using UnityEngine;

public class DayNightManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Settings")]
    [SerializeField] private float dayDuration = 60f;
    [SerializeField] private float nightDuration = 30f;
    [SerializeField] private float rotationDegrees = 200f;
    [Tooltip("The initial rotation of the directional light at the start of a new day.")]
    [SerializeField] private float lightStartRotation = 200f;
    [Tooltip("The threshold at which a new day is triggered. Negative values is extra time before the day ends.")]
    [SerializeField] private float newDayThreshold = -1f;
    [SerializeField] private float dayDivide = 4;
    [SerializeField] private float lowTempraturePreset = 5;
    [SerializeField] private float mediumTempraturePreset = 10;
    [SerializeField] private float highTempraturePreset = 40;
    [Header("References")]
    [SerializeField] private Light directionalLight;

    private float coldMultiplyer = 5;

    private bool paused = false;
    private bool isNight = false;
    private float dayTimer;


    //spawna resurser:
    //Vector3 pos = Random.insideUnitCircle;  //+deadzone runt huset och *radien 


    private void Awake()
    {
        //can be moved to a more general script when it is created
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 90;
    }

    void Start()
    {
        //Remove when resources are created in runtime, weird stacking issue with map creation
        Newday();
    }

    private void FixedUpdate()
    {
        if (paused) return;


        dayTimer -= Time.fixedDeltaTime;
        if (dayTimer <= newDayThreshold)        
            Newday();        

        CheckTempratureChange();

        directionalLight.transform.rotation = Quaternion.Euler(CalculateLightRotation());
    }

    private void Newday()
    {
        //ReferenceManager.Instance.mapManager.ClearMapObjects();//comment this out to test the day night cycle without clearing the map objects

        dayTimer = dayDuration;
        directionalLight.transform.Rotate(new Vector3(lightStartRotation, 0, 0));
        coldMultiplyer = 5;

        ReferenceManager.Instance.mapManager.CreateMap();//Maybe move this to pcg script so its called when the map is finished generating
    }

    private void CheckTempratureChange()
    {
        Debug.Log("coldMult: " + coldMultiplyer);
        if(isNight)
        {
            coldMultiplyer = highTempraturePreset;
            return;
        }

        if (dayTimer <= 3 * (dayDuration / dayDivide))
        {
            if (dayTimer <= dayDuration / dayDivide)
            {
                coldMultiplyer = lowTempraturePreset;
                return;
            }
            coldMultiplyer = mediumTempraturePreset;
        }
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
}
