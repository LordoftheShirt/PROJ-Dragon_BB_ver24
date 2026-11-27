using DebugTools;
using UnityEngine;

public class PlayerMeter : MonoBehaviour
{
    public static bool GodMode { get; set; } = false;

    private float maxWarmth = 100f;
    private float maxHealth = 100f;
    private float currentWarmth;
    private float currentHealth;

    [Header("Health loss per second - MAX HP: 100")]
    [SerializeField] private float HealthDecayRate = 2f;
    private float HealthRegenRate = 4;

    //[Header("Warmth loss per second - MAX W: 100")]
    //[SerializeField] private float DefultWarmthDecayRate = 1f;

    private float warmthDecayRate;

    private PlayerUpgrades _upgrades;

    void Start()
    {
        _upgrades = PlayerUpgrades.Instance;
        //ResetMeter();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GodMode)
        {
            Decay();

            if(currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void IncreaseWarmth(float amount)
    {
        currentWarmth += amount;
        if (currentWarmth > maxWarmth)
            currentWarmth = maxWarmth;
    }

    void Decay()
    {
        float resistance = 1 + _upgrades.ColdResistBoost;

        // when DecayRate is 1, it takes 100 seconds to go from 100 to 0. when DecayRate is 2, it takes 50 seconds. when DecayRate is 0.5, it takes 200 seconds.
        currentWarmth -= warmthDecayRate / resistance * Time.deltaTime;

        // Frost damage
        if (currentWarmth <= 0)
        {
            currentWarmth = 0;
            currentHealth -= HealthDecayRate * Time.deltaTime;
        } else if (currentHealth < maxHealth)
        {
            currentHealth += HealthRegenRate * Time.deltaTime;
        }
    }



    public void ChangeWarmthDecayrate(float newRate)
    {
        //resetts decayrate
        warmthDecayRate = newRate;
    }

    public void Die()
    {
        if (GodMode) return;

        ReferenceManager.Instance.gameManager.ResetWorld();
        ReferenceManager.Instance.gameManager.ResetTemporaryResources();
    }

    public void ResetMeter()
    {
        currentWarmth = maxWarmth;
        currentHealth = maxHealth;
        
    }

    public float GetMaxWarmth() { return maxWarmth; }
    public float GetMaxHealth() { return maxHealth; }
    public float GetCurrentWarmth() { return currentWarmth; }
    public float GetCurrentHealth() { return currentHealth; }

    [DebugCommand("Toggles God Mode (invincibility)", DisplayName = "GodMode")]
    public static void ToggleGodMode()
    {
        GodMode = !GodMode;
        Debug.Log("Turned GodMode " + (GodMode ? "on" : "off"));
    }
}