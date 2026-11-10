using UnityEngine;
using UnityEngine.UI;

public class PlaceHolderUI : MonoBehaviour
{
    // Points slider
    [SerializeField] private PlayerMeter stats;

    [SerializeField] private Slider warmthSlider;
    [SerializeField] private Slider healthSlider;


    // Compass
    [SerializeField] private RectTransform compassTransform;
    private Transform playerTransform;


    void Start()
    {
        warmthSlider.maxValue = stats.GetMaxWarmth();
        healthSlider.maxValue = stats.GetMaxHealth();

        playerTransform = Camera.main.transform;
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
}
