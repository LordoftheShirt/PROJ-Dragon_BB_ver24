using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceUISlot : MonoBehaviour
{
    public ResourceType Type
    {
        get => _type.HasValue ? _type.Value : default;
        set
        {
            TryMakeMaterials();

            _type = value;
            icon.sprite = EnumStyle.GetResourceIcon(value);

            int count = GetResource(value);

            UpdateCountText(count);

            if (nameText != null) nameText.text = Type.GetDisplayString(true);

            UpdateColor(count, true);
        }
    }
    private ResourceType? _type;

    private Canvas _canvas;

    [SerializeField] private SlotMode mode;
    [SerializeField] private Graphic sdfGraphic;
    private Material _material;
    private bool _madeMaterials;
    [SerializeField] private Graphic[] colorGraphics;
    [SerializeField] private Graphic[] backgroundColorGraphics;

    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;

    [Header("Tween")]
    [SerializeField] private UIEffect uiEffect;
    [SerializeField] private float tweenShineDuration;
    [SerializeField] private Ease tweenShineEase;
    private Tween _uiEffectShineTween;
    [Space]
    [SerializeField] private float tweenOverlayDuration;
    [SerializeField] private Ease tweenOverlayEase;
    private Tween _uiEffectOverlayTween;

    [Space]
    [SerializeField] private float tweenSize;
    [SerializeField] private float tweenSizeDuration;
    [SerializeField] private Ease tweenSizeEase;

    private bool _isDisabledColor;
    private Color _currentColor;
    private Color _currentBackgroundColor;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>(true);
    }

    private void OnEnable()
    {
        TryMakeMaterials();

        switch (mode)
        {
            case SlotMode.Temporary:
                Inventory.OnResourceTemporaryChanged += OnResourceChanged;
                break;

            case SlotMode.Permanent:
                Inventory.OnResourcePermanentChanged += OnResourceChanged;
                break;

            default:
                Inventory.OnResourceChanged += OnResourceChanged;
                break;
        }

        if (_type.HasValue)
        {
            UpdateCountText(GetResource(_type.Value), true);
        }
    }

    private void TryMakeMaterials()
    {
        if (_madeMaterials) return;
        _madeMaterials = true;

        if (sdfGraphic != null)
        {
            sdfGraphic.material = new Material(sdfGraphic.material);
            _material = sdfGraphic.material;
        }
    }

    private void OnDisable()
    {
        if (_uiEffectShineTween != null) _uiEffectShineTween.Kill();
        if (_uiEffectOverlayTween != null) _uiEffectOverlayTween.Kill();
        if (uiEffect != null) uiEffect.DOKill();

        transform.DOKill();

        switch (mode)
        {
            case SlotMode.Temporary:
                Inventory.OnResourceTemporaryChanged -= OnResourceChanged;
                break;

            case SlotMode.Permanent:
                Inventory.OnResourcePermanentChanged -= OnResourceChanged;
                break;

            default:
                Inventory.OnResourceChanged -= OnResourceChanged;
                break;
        }
    }

    private int GetResource(ResourceType type)
    {
        Inventory inv = Inventory.Instance;
        return mode switch
        {
            SlotMode.Temporary => inv.GetResourceTemporary(type),
            SlotMode.Permanent => inv.GetResourcePermanent(type),
            _ => inv.GetResource(type),
        };
    }

    private void OnResourceChanged(ResourceType type, int count, int change)
    {
        if (type == Type)
        {
            if (change > 0)
            {
                // Do anim
                if (tweenSizeDuration > 0)
                {
                    if (uiEffect != null)
                    {
                        if (_uiEffectShineTween == null)
                        {
                            uiEffect.transitionRate = 0;
                            _uiEffectShineTween = uiEffect.DOTransitionRate(1, tweenSizeDuration).SetEase(tweenShineEase);
                            _uiEffectShineTween.onComplete = () =>
                            {
                                _uiEffectShineTween = null;
                            };
                        }

                        if (_uiEffectOverlayTween != null) _uiEffectOverlayTween.Kill();

                        uiEffect.colorIntensity = 1;
                        _uiEffectOverlayTween = uiEffect.DOColorIntensity(0, tweenOverlayDuration).SetEase(tweenOverlayEase);
                    }

                    transform.DOKill();
                    transform.localScale = Vector2.one * tweenSize;
                    transform.DOScale(Vector2.one, tweenSizeDuration).SetEase(tweenSizeEase);
                }
            }

            UpdateCountText(count);
        }
    }

    private void UpdateCountText(int count, bool forced = false)
    {
        if (countText != null) countText.text = count.ToString();

        UpdateColor(count, forced);
    }

    private void UpdateColor(int count, bool forced = false)
    {
        bool disabledColor = count <= 0;

        if (!forced && disabledColor == _isDisabledColor) return;
        _isDisabledColor = disabledColor;

        Color col = disabledColor ? EnumStyle.DisabledColor : EnumStyle.GetResourceColor(Type);
        Color bgCol = disabledColor ? EnumStyle.DisabledBackgroundColor : EnumStyle.GetResourceBackgroundColor(Type);

        _currentColor = col;
        _currentBackgroundColor = bgCol;

        icon.color = col;
        countText.color = col;

        if (_material != null)
        {
            _material.SetColor(SdfShaderProperties.OUTLINE_COLOR, col);
            _material.SetColor(SdfShaderProperties.COLOR, bgCol);
        }

        if (colorGraphics != null)
        {
            foreach (var graphic in colorGraphics)
            {
                graphic.color = col;
            }
        }

        if (backgroundColorGraphics != null)
        {
            foreach (var graphic in backgroundColorGraphics)
            {
                graphic.color = bgCol;
            }
        }
    }

    public RectTransform CreateCloneWithAnim()
    {
        RectTransform clone = CreateClone();

        Vector3 endPos = clone.localPosition + (Vector3)(Random.insideUnitCircle * Random.Range(25f, 50f));
        clone.DOLocalMove(endPos, 0.7f).SetEase(Ease.OutExpo);
        clone.DORotate(new Vector3(0, 0, Random.Range(-20f, 20f)), 0.7f).SetEase(Ease.OutExpo);

        return clone;
    }

    public RectTransform CreateClone()
    {
        GameObject obj = new GameObject(gameObject.name + " (Clone)", typeof(RectTransform));
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.SetParent(_canvas.transform, false);

        RectTransform thisRect = transform as RectTransform;

        rect.pivot = thisRect.pivot;
        rect.sizeDelta = thisRect.sizeDelta;
        rect.position = thisRect.position;
        rect.localScale = thisRect.lossyScale;

        if (sdfGraphic != null)
        {
            GameObject imgObj = new GameObject("sdf", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform imgRect = imgObj.GetComponent<RectTransform>();
            imgRect.SetParent(rect, false);
            imgRect.localPosition = Vector2.zero;
            imgRect.anchorMin = Vector2.zero;
            imgRect.anchorMax = Vector2.one;
            imgRect.anchoredPosition = Vector2.zero;
            imgRect.sizeDelta = Vector2.zero;

            Image img = imgObj.GetComponent<Image>();
            Material material = new Material(sdfGraphic.material);
            material.SetColor(SdfShaderProperties.OUTLINE_COLOR, _currentColor);
            material.SetColor(SdfShaderProperties.COLOR, _currentBackgroundColor);
            img.material = material;
        }

        void CopyRect(RectTransform from, RectTransform to)
        {
            to.pivot = from.pivot;
            to.anchorMin = from.anchorMin;
            to.anchorMax = from.anchorMax;

            to.localPosition = from.localPosition;
            to.anchoredPosition = from.anchoredPosition;
            to.sizeDelta = from.sizeDelta;
        }

        if (icon != null)
        {
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.SetParent(rect, false);
            CopyRect(icon.GetComponent<RectTransform>(), iconRect);

            Image img = iconObj.GetComponent<Image>();
            img.sprite = icon.sprite;
            img.material = icon.material;
            img.color = _currentColor;
        }

        if (countText != null)
        {
            GameObject textObj = new GameObject("Count Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.SetParent(rect, false);
            CopyRect(countText.GetComponent<RectTransform>(), textRect);

            TMP_Text text = textObj.GetComponent<TMP_Text>();
            text.text = countText.text;
            text.font = countText.font;
            text.fontSize = countText.fontSize;
            text.color = _currentColor;
            text.alignment = countText.alignment;
        }

        return rect;
    }

    public enum SlotMode
    {
        Temporary,
        Permanent,
        Any,
    }
}
