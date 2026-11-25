using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class PlayerUI : Singleton<PlayerUI>
{
    public const string STORM_TIME_FORMAT = "mm':'ss";

    [Header("Top bar")]
    [SerializeField] private TMP_Text worldTempText;
    private string _worldTempFormat;

    [Space]
    [SerializeField] private Slider hpBar;
    [SerializeField] private TMP_Text hpText;
    private string _hpFormat;

    [Space]
    [SerializeField] private Slider warmthBar;
    [SerializeField] private TMP_Text warmthText;
    private string _warmthFormat;
    private PlayerMeter _meter;

    [Space]
    [SerializeField] private TMP_Text stormTimeText;
    private string _stormTimeFormat;

    [Space]
    [SerializeField] private ResourceUISlot resourceUISlotTemplate;
    private ResourceUISlot[] _resourceUISlots;

    [Header("Compass")]
    [SerializeField] private Item compassItem;
    [SerializeField] private GameObject compass;
    [SerializeField] private RectTransform compassNeedle;
    private Transform _playerTransform;

    [Header("Day Counter")]
    [SerializeField] private TMP_Text dayCounter;
    private string _dayCounterFormat;

    private DayNightManager _dayNightManager;

    private void Start()
    {
        ReferenceManager refManager = ReferenceManager.Instance;
        GameObject player = refManager.player;
        _dayNightManager = refManager.dayNightManager;

        _playerTransform = player.transform;
        _meter = player.GetComponent<PlayerMeter>();

        _worldTempFormat = worldTempText.text;
        _hpFormat = hpText.text;
        _warmthFormat = warmthText.text;
        _stormTimeFormat = stormTimeText.text;

        ResourceType[] allResourceTypes = ResourceTypeUtil.GetAllResourceTypes();
        int length = allResourceTypes.Length;
        _resourceUISlots = new ResourceUISlot[length];

        for (int i = 0; i < length; i++)
        {
            var newSlot = Instantiate(resourceUISlotTemplate, resourceUISlotTemplate.transform.parent);
            newSlot.Type = allResourceTypes[i];

            _resourceUISlots[i] = newSlot;
        }
        resourceUISlotTemplate.gameObject.SetActive(false);

        hpBar.maxValue = _meter.GetMaxHealth();
        warmthBar.maxValue = _meter.GetMaxWarmth();

        UpdateHP();
        UpdateWarmth();

        compass.SetActive(Inventory.Instance.HasItem(compassItem));

        Inventory.OnItemChanged += OnItemChanged;

        _dayCounterFormat = dayCounter.text;
        UpdateDayCounter();
        SleepNewWorld.OnSleep += OnSleep;
    }

    private void OnDestroy()
    {
        Inventory.OnItemChanged -= OnItemChanged;
    }

    private void OnItemChanged(Item item, int count, int change)
    {
        if (item == compassItem)
        {
            compass.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        UpdateCompass();
        UpdateWorldTemp();
        UpdateHP();
        UpdateWarmth();
        UpdateStormTime();
    }

    void UpdateCompass()
    {
        Vector3 playerForward = _playerTransform.forward;
        playerForward.y = 0;
        float angleToNorth = Vector3.SignedAngle(Vector3.right, playerForward, Vector3.up);
        compassNeedle.localRotation = Quaternion.Euler(0, 0, -angleToNorth);
    }

    private void UpdateWorldTemp()
    {
        worldTempText.text = string.Format(_worldTempFormat, _dayNightManager.GetCurrentTemrpature());
    }

    private void UpdateHP()
    {
        float hp = _meter.GetCurrentHealth();
        hpBar.value = hp;
        hpText.text = string.Format(_hpFormat, hp);
    }

    private void UpdateWarmth()
    {
        float warmth = _meter.GetCurrentWarmth();
        warmthBar.value = warmth;
        warmthText.text = string.Format(_warmthFormat, warmth);
    }

    private void UpdateStormTime()
    {
        float time = Mathf.Max(_dayNightManager.DayTimer, 0);
        TimeSpan span = TimeSpan.FromSeconds(time);
        stormTimeText.text = string.Format(_stormTimeFormat, span.ToString(STORM_TIME_FORMAT));
    }

    private void OnSleep(int day)
    {
        UpdateDayCounter();
    }

    private void UpdateDayCounter()
    {
        dayCounter.text = string.Format(_dayCounterFormat, SleepNewWorld.DayNumber);
    }
}
