using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hotbar : Singleton<Hotbar>
{
    public static Item[] Items => _items;
    private static Item[] _items;

    public static int? SelectedSlot
    {
        get => Instance._selectedSlot;
        set => Instance.SetSelectedSlot(value);
    }

    [SerializeField] private int maxSlots = 9;

    [Space]
    [SerializeField] private HotbarSlot slotTemplate;
    private int? _selectedSlot;
    private int _previouslySelectedSlot;

    [Space]
    [SerializeField] private Item[] addToHotbarInAwake;

    [Header("Inputs")]
    [SerializeField] private InputActionReference cycleInput;
    [SerializeField] private InputActionReference useInput;
    [SerializeField] private InputAction[] numberInputs;
    private GameCursor _gameCursor;
    private BuildingPlacer _buildingPlacer;
    private Inventory _inventory;

    private HotbarSlot[] _slots;

    protected override void Awake()
    {
        base.Awake();

        _items = new Item[maxSlots];
        _slots = new HotbarSlot[maxSlots];

        slotTemplate.gameObject.SetActive(false);

        for (int i = 0; i < maxSlots; i++)
        {
            int number = i + 1;
            HotbarSlot slot = Instantiate(slotTemplate, slotTemplate.transform.parent);
            slot.gameObject.name = slotTemplate.name + " (" + number + ")";
            slot.Disable();

            slot.SetKeyText(number.ToString());

            _slots[i] = slot;
        }

        cycleInput.action.performed += OnCycleInputPerformed;
        useInput.action.performed += OnUseInputPerformed;

        for (int i = 0; i < maxSlots; i++)
        {
            int index = i;
            InputAction action = numberInputs[index];
            action.Enable();
            action.performed += (ctx) =>
            {
                if (!_slots[index].SlotEnabled || !cycleInput.action.enabled || !useInput.action.enabled) return;

                if (SelectedSlot.HasValue && SelectedSlot.Value == index)
                {
                    SetSelectedSlot(null);
                }
                else
                {
                    SetSelectedSlot(index);
                }
            };
        }

        Inventory.OnItemChanged += OnItemChanged;
    }

    private void OnDestroy()
    {
        cycleInput.action.performed -= OnCycleInputPerformed;
        useInput.action.performed -= OnUseInputPerformed;

        Inventory.OnItemChanged -= OnItemChanged;
    }

    private void Start()
    {
        _buildingPlacer = BuildingPlacer.Instance;
        _gameCursor = GameCursor.Instance;
        _inventory = Inventory.Instance;

        int length = addToHotbarInAwake.Length;
        for (int i = 0; i < length; i++)
        {
            SetItem(i, addToHotbarInAwake[i]);
        }
    }

    private void OnCycleInputPerformed(InputAction.CallbackContext ctx)
    {
        int input = Mathf.RoundToInt(ctx.ReadValue<float>());

        if (_selectedSlot.HasValue)
        {
            int failsafe = 100;
            int selectedSlot = _selectedSlot.Value;
            do
            {
                failsafe--;
                selectedSlot += input;

                if (selectedSlot < 0) selectedSlot = maxSlots - 1;
                else if (selectedSlot >= maxSlots) selectedSlot = 0;

            } while (!_slots[selectedSlot].SlotEnabled && failsafe > 0);

            if (failsafe <= 0)
            {
                Debug.LogError("INFINITE LOOP DETECTED");
            }

            SetSelectedSlot(selectedSlot, false);
        }
        else
        {
            SetSelectedSlot(_previouslySelectedSlot, false);
        }
    }

    private void OnUseInputPerformed(InputAction.CallbackContext ctx)
    {
        if (!_selectedSlot.HasValue) return;

        Item item = _items[_selectedSlot.Value];

        if (item != null)
        {
            item.Use();
        }
    }

    public void SetSelectedSlot(int? i, bool clamp = true)
    {
        if (clamp && i.HasValue) i = Mathf.Clamp(i.Value, 0, maxSlots - 1);

        if (i == _selectedSlot) return;

        if (SelectedSlot.HasValue)
        {
            _previouslySelectedSlot = SelectedSlot.Value;
        }

        if (_selectedSlot.HasValue)
        {
            _slots[_selectedSlot.Value].Unselect();
            _items[_selectedSlot.Value].Unselect();
        }

        _selectedSlot = i;

        if (i.HasValue)
        {
            _slots[i.Value].Select();
            _items[i.Value].Select();
        }
    }

    public void SetItem(int index, Item item)
    {
        _items[index] = item;
        HotbarSlot slot = _slots[index];

        if (item == null)
        {
            slot.Disable();
        }
        else
        {
            slot.Enable();
            slot.SetColor(item);
            slot.SetCountText(_inventory.GetItemAmount(item).ToString());
        }
    }

    public void RemoveItem(int index)
    {
        _items[index] = null;
        _slots[index].Disable();
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (_slots[i].SlotEnabled) continue;

            SetItem(i, item);

            return true;
        }

        // NO OPEN SLOT
        return false;
    }

    private void LateUpdate()
    {
        _gameCursor.RadialFillAmount = _buildingPlacer.GetHoldTimer() / _buildingPlacer.GetHoldToPlaceSeconds();
    }

    private void OnItemChanged(Item item, int count, int change)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            // UPDATE ITEM!
            if (_items[i] == item)
            {
                HotbarSlot slot = _slots[i];
                slot.SetCountText(count.ToString());
                slot.UpdateColor();
                return;
            }
        }
    }
}
