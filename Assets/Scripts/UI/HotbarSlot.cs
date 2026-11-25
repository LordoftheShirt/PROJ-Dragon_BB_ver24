using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public bool SlotEnabled { get; private set; } = false;

    [SerializeField] private Graphic sdfGraphic;
    private Material _material;
    private bool _madeMaterials;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color defaultBackgroundColor = Color.black;

    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private Graphic[] mainColorGraphics;
    [SerializeField] private Graphic[] backgroundColorGraphics;

    [Space]
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private TMP_Text countText;

    [Space]
    [SerializeField] private Graphic selectionOutline;
    [SerializeField] private float selectionOutlineTweenDuration;
    private Material _selectionOutlineMaterial;
    private Vector2 _selectedOutlineThickness;
    private Vector2 _unselectedOutlineThickness;
    private Item _item;
    private bool _oldIsDisabled;

    private void Start()
    {
        TryMakeMaterials();
    }

    private void TryMakeMaterials()
    {
        if (_madeMaterials) return;
        _madeMaterials = true;

        sdfGraphic.material = new Material(sdfGraphic.material);
        _material = sdfGraphic.material;

        // Selection outline
        selectionOutline.material = new Material(selectionOutline.material);
        _selectionOutlineMaterial = selectionOutline.material;

        _selectedOutlineThickness = GetSelectionOutineThickness();
        _unselectedOutlineThickness = _selectedOutlineThickness;

        _unselectedOutlineThickness.x = _selectedOutlineThickness.y;
        SetSelectionOutlineThickness(_unselectedOutlineThickness);

        selectionOutline.gameObject.SetActive(false);
    }

    #region Selection Outline Thickness
    private Vector2 GetSelectionOutineThickness()
    {
        Vector4 outlineThickness = _selectionOutlineMaterial.GetVector(SdfShaderProperties.OUTLINE_THICKNESS);
        return new(outlineThickness.x, outlineThickness.y);
    }

    private void SetSelectionOutlineThickness(Vector2 vector)
    {
        _selectionOutlineMaterial.SetVector(SdfShaderProperties.OUTLINE_THICKNESS, new(vector.x, vector.y, 0,0));
    }

    private Tween TweenSelectionOutlineThickness(Vector2 to, float duration)
    {
        return DOTween.To(GetSelectionOutineThickness, SetSelectionOutlineThickness, to, duration).SetTarget(selectionOutline);
    }
    #endregion

    public void Select()
    {
        selectionOutline.DOKill();
        selectionOutline.gameObject.SetActive(true);
        TweenSelectionOutlineThickness(_selectedOutlineThickness, selectionOutlineTweenDuration);
    }

    public void Unselect()
    {
        selectionOutline.DOKill();
        TweenSelectionOutlineThickness(_unselectedOutlineThickness, selectionOutlineTweenDuration).onComplete = () =>
        {
            selectionOutline.gameObject.SetActive(false);
        };
    }

    public void SetColor(Item item)
    {
        TryMakeMaterials();

        _item = item;
        UpdateColor(true);
    }

    public void UpdateColor(bool forced = false)
    {
        int count = Inventory.Instance.GetItemAmount(_item);

        bool isDisabled = count <= 0;

        if (!forced && isDisabled == _oldIsDisabled) return;
        _oldIsDisabled = isDisabled;

        Color color = isDisabled ? EnumStyle.DisabledColor : (_item == null || _item.Color.a <= 0 ? defaultColor : _item.Color);
        Color backgroundColor = isDisabled ? EnumStyle.DisabledBackgroundColor : (_item == null || _item.BackgroundColor.a <= 0 ? defaultBackgroundColor : _item.BackgroundColor);

        if (_item != null)
        {
            icon.sprite = _item.Icon;
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }

        foreach (var graphic in mainColorGraphics)
        {
            graphic.color = color;
        }

        foreach (var graphic in backgroundColorGraphics)
        {
            graphic.color = backgroundColor;
        }

        _material.SetColor(SdfShaderProperties.COLOR, backgroundColor);
        _material.SetColor(SdfShaderProperties.OUTLINE_COLOR, color);
    }

    public void SetKeyText(string text)
    {
        keyText.text = text;
    }

    public void SetCountText(string text)
    {
        countText.text = text;
    }

    public void Enable() => Toggle(true);
    public void Disable() => Toggle(false);
    public void Toggle(bool enable)
    {
        SlotEnabled = enable;
        gameObject.SetActive(enable);
    }
}
