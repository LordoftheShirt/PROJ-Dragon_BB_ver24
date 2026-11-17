using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles changing the color of the skybox.
/// </summary>
[LoadOnInit]
public class DayNightVisuals : SingletonPersistent<DayNightVisuals>
{
    private static readonly int SKY_TINT_PROP = Shader.PropertyToID("_SkyTint");
    private static readonly int EXPOSURE_PROP = Shader.PropertyToID("_Exposure");

    //[Range(0, 1)] [SerializeField] private float t;
    [SerializeField] private Material mat;
    [Header("Skybox Color")]
    [SerializeField] private AnimationCurve colorCurve;
    [SerializeField] private Color[] colors;
    private int _colorsLength;

    [Header("Skybox Exposure")]
    [SerializeField] private AnimationCurve exposureCurve;
    [SerializeField] private float[] exposureValues;
    private int _exposureLength;

    [Header("Light Intensity")]
    [SerializeField] private AnimationCurve lightIntensityCurve;
    [SerializeField] private float[] lightIntensityValues;
    private int _lightIntensityLength;

    [Header("Fog")]
    [SerializeField] private AnimationCurve fogCurve;
    [SerializeField] private Color[] fogColors;
    private int _fogLength;

    private DayNightManager _dayNight;

    private float _oldT = -999;

    private void Start()
    {
        _colorsLength = colors.Length;
        _exposureLength = exposureValues.Length;
        _lightIntensityLength = lightIntensityValues.Length;
        _fogLength = fogColors.Length;

        SetDayNight();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        SetDayNight();
    }

    private void SetDayNight()
    {
        var refManager = ReferenceManager.Instance;
        if (refManager == null) return;

        _dayNight = refManager.dayNightManager;
        UpdateVisuals(GetDayNightT(), true);
    }

    private void LateUpdate()
    {
        float t;
        if (_dayNight != null) t = GetDayNightT();
        else t = 0.5f;

        UpdateVisuals(t);
    }

    private float GetDayNightT() => 1 - _dayNight.NormalizedDayTimer;

    private void UpdateVisuals(float t, bool forced = false)
    {
        t = Mathf.Clamp01(t);

        if (!forced && t == _oldT) return;

        _oldT = t;

        Color color = GetValue(colors, _colorsLength, t, colorCurve, Color.Lerp);
        float exposure = GetValue(exposureValues, _exposureLength, t, exposureCurve, Mathf.Lerp);
        float lightIntensity = GetValue(lightIntensityValues, _lightIntensityLength, t, lightIntensityCurve, Mathf.Lerp);
        Color fogColor = GetValue(fogColors, _fogLength, t, fogCurve, Color.Lerp);

        mat.SetColor(SKY_TINT_PROP, color);
        mat.SetFloat(EXPOSURE_PROP, exposure);
        _dayNight.DirectionalLight.intensity = lightIntensity;
        RenderSettings.fogColor = fogColor;
    }

    private T GetValue<T>(T[] array, int length, float t, AnimationCurve curve, Func<T, T, float, T> lerp)
    {
        float curveT = curve.Evaluate(t);

        int aIndex = Mathf.Max(0, Mathf.FloorToInt(curveT));
        int bIndex = Mathf.Min(length - 1, Mathf.CeilToInt(curveT));

        curveT %= 1;

        if (curveT <= 0) return array[aIndex];
        else if (curveT >= 1) return array[bIndex];

        return lerp.Invoke(array[aIndex], array[bIndex], curveT);
    }
}
