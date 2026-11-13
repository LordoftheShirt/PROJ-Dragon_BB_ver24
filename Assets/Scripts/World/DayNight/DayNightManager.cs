using UnityEngine;
using System.Collections;

public class DayNightManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Settings")]
    [SerializeField] private float dayDuration = 60f;
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

    private float coldMultiplyer;

    private bool paused = false;
    private bool isNight = false;
    private float dayTimer;


    bool createdMap = false;
    float trimer = 1;


    void Start()
    {
        //Remove when resources are created in runtime, weird stacking issue with map creation'
        Newday();
    }

    private void FixedUpdate()
    {
        if (paused) return;

        if(!createdMap)
        {
            if(trimer < 0)
            {
                ReferenceManager.Instance.mapManager.CreateMap();
                createdMap = true;
            }
            trimer -= Time.fixedDeltaTime;                
        }



        dayTimer -= Time.fixedDeltaTime;
        if (dayTimer <= newDayThreshold)        
            Newday();        

        CheckTempratureChange();

        directionalLight.transform.rotation = Quaternion.Euler(CalculateLightRotation());
    }

    public void Newday()
    {
        //ReferenceManager.Instance.mapManager.ClearMapObjects();//comment this out to test the day night cycle without clearing the map objects

        dayTimer = dayDuration;
        directionalLight.transform.Rotate(new Vector3(lightStartRotation, 0, 0));

        ChangeTemprature(coldMultiplyer, mediumTempraturePreset);
        coldMultiplyer = mediumTempraturePreset;
    }

    private void CheckTempratureChange()
    {
        if(isNight)
        {
            ChangeTemprature(coldMultiplyer, highTempraturePreset);
            coldMultiplyer = highTempraturePreset;
            return;
        }

        if (dayTimer <= 3 * (dayDuration / dayDivide))
        {
            if (dayTimer <= dayDuration / dayDivide)
            {
                ChangeTemprature(coldMultiplyer, mediumTempraturePreset);
                coldMultiplyer = mediumTempraturePreset;
                return;
            }
            ChangeTemprature(coldMultiplyer, lowTempraturePreset);
            coldMultiplyer = lowTempraturePreset;
        }
    }

    private void ChangeTemprature(float oldChange, float newChange)
    {
        if (oldChange != newChange)        
            ReferenceManager.Instance.playerMeter.ChangeWarmthDecayrate(newChange);        
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
