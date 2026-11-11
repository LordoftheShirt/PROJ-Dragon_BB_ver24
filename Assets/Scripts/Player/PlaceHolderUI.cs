using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceHolderUI : MonoBehaviour
{
    // Points slider
    [Header("Health bar, Warmth bar")]
    [SerializeField] private PlayerMeter stats;

    [SerializeField] private Slider warmthSlider;
    [SerializeField] private Slider healthSlider;


    // Compass
    [Header("Compass")]
    [SerializeField] private RectTransform compassTransform;
    private Transform playerTransform;

    // Consumable Counters
    [Header("Consumable Counters")]
    [SerializeField] private TextMeshProUGUI flagCounter;
    [SerializeField] private TextMeshProUGUI fireKitCounter;
    [SerializeField] private TextMeshProUGUI tentCounter;

    [Header("Place Timer")]
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private Slider buildTimeSlider;

    void Start()
    {
        playerTransform = Camera.main.transform;

        UpdateFlagCounter(ConsumablesManager.Instance.GetCurrentFlags());
        UpdateTentCounter(ConsumablesManager.Instance.GetCurrentTents());
        UpdateFirekitCounter(ConsumablesManager.Instance.GetCurrentFirekits());

        if (stats != null)
        {
            warmthSlider.maxValue = stats.GetMaxWarmth();
            healthSlider.maxValue = stats.GetMaxHealth();
        }

        if (buildTimeSlider != null && buildingPlacer != null)
        {
            buildTimeSlider.maxValue = buildingPlacer.GetHoldToPlaceSeconds();
            buildTimeSlider.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (compassTransform != null)
        {
            UpdateCompass();
        }

        if (stats != null)
        {
            UpdateSliders();
        }

        if (buildingPlacer != null && buildTimeSlider != null && buildTimeSlider.IsActive())
        {
            buildTimeSlider.value = buildingPlacer.GetHoldTimer();
        }
    }

    void UpdateCompass()
    {
        Vector3 playerForward = playerTransform.forward;
        playerForward.y = 0;
        float angleToNorth = Vector3.SignedAngle(Vector3.forward, playerForward, Vector3.up);
        compassTransform.localRotation = Quaternion.Euler(0, 0, angleToNorth);
    }

    void UpdateSliders()
    {
        warmthSlider.value = stats.GetCurrentWarmth();
        healthSlider.value = stats.GetCurrentHealth();
    }

    private void OnEnable()
    {
        if (ConsumablesManager.Instance != null)
        {
            ConsumablesManager.Instance.OnFlagsChanged += UpdateFlagCounter;
            ConsumablesManager.Instance.OnTentsChanged += UpdateTentCounter;
            ConsumablesManager.Instance.OnFirekitsChanged += UpdateFirekitCounter;

            buildingPlacer.OnGhostPresent += ActivateBuildSlider;

        }
    }

    private void OnDisable()
    {
        if (ConsumablesManager.Instance != null)
        {
            ConsumablesManager.Instance.OnFlagsChanged -= UpdateFlagCounter;
            ConsumablesManager.Instance.OnTentsChanged -= UpdateTentCounter;
            ConsumablesManager.Instance.OnFirekitsChanged -= UpdateFirekitCounter;

            buildingPlacer.OnGhostPresent -= ActivateBuildSlider;
        }
    }

    void UpdateFlagCounter(int flagCount)
    {
        if (flagCounter != null)
        {
            flagCounter.text = flagCount.ToString() + "/" + ConsumablesManager.Instance.GetMaxFlags().ToString();
        }
    }

    void UpdateTentCounter(int tentCount)
    {
        if (tentCounter != null)
        {
            tentCounter.text = tentCount.ToString() + "/" + ConsumablesManager.Instance.GetMaxTents().ToString();
        }
    }

    void UpdateFirekitCounter(int firekitCount)
    {
        if (fireKitCounter != null)
        {
            fireKitCounter.text = firekitCount.ToString() + "/" + ConsumablesManager.Instance.GetMaxFirekits().ToString();
        }
    }

    void ActivateBuildSlider(bool activate)
    {
        if (buildTimeSlider != null)
            buildTimeSlider.gameObject.SetActive(activate);
    }
}
