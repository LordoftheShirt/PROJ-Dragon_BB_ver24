using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUIRequirement : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color defaultBackgroundColor = Color.white;

    [Space]
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private Color inactiveBackgroundColor = Color.white;

    [Space]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text countText;
    private string _countTextFormat = null;

    [Space]
    [SerializeField] private GameObject check;
    [SerializeField] private GameObject x;

    [Space]
    [SerializeField] private Graphic[] colorGraphics;
    [SerializeField] private Graphic[] backgroundColorGraphics;

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }
    public int RequiredAmount { get; set; }
    public ResourceType Type { get; set; }
    public Item Item { get; set; }
    public Sprite Icon { get => icon.sprite; set => icon.sprite = value; }

    private void SetColor(Color color, Color backgroundColor)
    {
        foreach (var graphic in colorGraphics)
        {
            graphic.color = color;
        }

        foreach (var graphic in backgroundColorGraphics)
        {
            graphic.color = backgroundColor;
        }
    }

    public void UpdateRequirement()
    {
        if (_countTextFormat == null) _countTextFormat = countText.text;

        Inventory inv = Inventory.Instance;
        int count = Item != null ? inv.GetItemAmount(Item) : inv.GetResource(Type);
        countText.text = string.Format(_countTextFormat, count, RequiredAmount);

        bool hasEnough = count >= RequiredAmount;
        check.SetActive(hasEnough);
        //x.SetActive(!hasEnough);

        if (!hasEnough)
        {
            SetColor(inactiveColor, inactiveBackgroundColor);
        }
        else if (Item != null)
        {
            if (Item.Color.a > 0 && Item.BackgroundColor.a > 0)
            {
                SetColor(Item.Color, Item.BackgroundColor);
            }
            else
            {
                SetColor(defaultColor, defaultBackgroundColor);
            }
        }
        else
        {
            SetColor(EnumStyle.GetResourceColor(Type), EnumStyle.GetResourceBackgroundColor(Type));
        }
    }
}
