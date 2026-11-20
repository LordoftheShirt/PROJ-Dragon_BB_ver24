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

    [Header("Compass")]
    [SerializeField] private RectTransform compassNeedle;
    private Transform _playerTransform;

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

        hpBar.maxValue = _meter.GetMaxHealth();
        warmthBar.maxValue = _meter.GetMaxWarmth();

        UpdateHP();
        UpdateWarmth();
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
        float angleToNorth = Vector3.SignedAngle(Vector3.forward, playerForward, Vector3.up);
        compassNeedle.localRotation = Quaternion.Euler(0, 0, -angleToNorth);
    }

    private void UpdateWorldTemp()
    {
        // TODO
        worldTempText.text = "TODO!";
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
}
