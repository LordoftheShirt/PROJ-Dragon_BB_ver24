using TMPro;
using UnityEngine;

public class CraftingTab : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private CraftingUI _craftingUI;

    private void Awake()
    {
        _craftingUI = GetComponentInParent<CraftingUI>(true);

    }
}
