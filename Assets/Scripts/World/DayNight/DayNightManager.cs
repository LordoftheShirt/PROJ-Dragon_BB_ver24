using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    public float NormalizedDayTimer => Mathf.Clamp01(dayTimer / dayDuration);
    public float DayTimer => dayTimer;
    public float DayDuration => dayDuration;
    public Light DirectionalLight => directionalLight;

    [Header("Settings")]
    [SerializeField] private float dayDuration = 60f;
    [SerializeField] private float rotationDegrees = 200f;
    [SerializeField] private AnimationCurve coldChange;

    [Tooltip("The initial rotation of the directional light at the start of a new day.")]
    [SerializeField] private float lightStartRotation = 200f;
    [Tooltip("The threshold at which a new day is triggered. Negative values is extra time before the day ends.")]
    [SerializeField] private float newDayThreshold = -1f;
    [SerializeField] private float dayDivide = 4;
    [Tooltip("Degrees outside in magnitude of 10. So a value of -1 is -10 degrees")]
    [SerializeField] private float nightTempraturePreset = 4;
    [Header("References")]
    [SerializeField] private Light directionalLight;


    private float currentTemprature;
    private float coldMultiplyer;

    private bool paused = false;
    private bool isNight = false;
    private float dayTimer;

    private void FixedUpdate()
    {
        if (paused || isNight) return;

        if (dayTimer <= newDayThreshold && !isNight)
            isNight = true;
        else
            dayTimer -= Time.fixedDeltaTime;

        CheckTempratureChange();

        if(!isNight)
            directionalLight.transform.rotation = Quaternion.Euler(CalculateLightRotation());
    }

    public void Newday()
    {
        dayTimer = dayDuration;
        directionalLight.transform.Rotate(new Vector3(lightStartRotation, 0, 0));

        CheckTempratureChange();
        isNight = false;
    }

    private void CheckTempratureChange()
    {
        if (isNight)
        {
            ChangeTemprature(coldMultiplyer, UpdateTemprature(nightTempraturePreset));
            return;
        }
        ChangeTemprature(coldMultiplyer, UpdateTemprature(coldChange.Evaluate(dayTimer / dayDuration) * 10));
    }

    private void ChangeTemprature(float oldChange, float newChange)
    {
        if (oldChange != newChange)        
            ReferenceManager.Instance.playerMeter.ChangeWarmthDecayrate(newChange);
        coldMultiplyer = newChange;
    }

    private float UpdateTemprature(float newTemp)
    {
        currentTemprature = newTemp * 10;
        return (newTemp * newTemp);
    }

    private Vector3 CalculateLightRotation()
    {
        float rotationAngle = (dayTimer / dayDuration) * rotationDegrees;
        return new Vector3(rotationAngle, 0, 0);
    }

    public void PauseDayCycle()
    {
        paused = true;
    }

    public void ResumeDayCycle()
    {
        paused = false;
    }
    public void PauseDayCycle(Vector3 newSunRotation)
    {
        paused = true;
        directionalLight.transform.rotation = Quaternion.Euler(newSunRotation);
    }

    public void ResumeDayCycle(Vector3 newSunRotation)
    {
        paused = false;
        directionalLight.transform.rotation = Quaternion.Euler(newSunRotation);
    }

    public int GetCurrentTemrpature()
    {
        return (int)currentTemprature;
    }

}
