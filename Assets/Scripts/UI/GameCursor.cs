using UnityEngine;
using UnityEngine.UI;

[LoadOnInit]
public class GameCursor : SingletonPersistent<GameCursor>
{
    private static readonly int INVERT_PROP = Shader.PropertyToID("_Invert");
    private static readonly int MSDF1_PROP = Shader.PropertyToID("_MSDF1");
    private static readonly int MSDF2_PROP = Shader.PropertyToID("_MSDF2");
    private static readonly int T_PROP = Shader.PropertyToID("_T");

    public State CurrentState
    {
        get => _currentState;
    }
    private State _currentState;

    public float RadialFillAmount
    {
        get => radialSlider.fillAmount;
        set => radialSlider.fillAmount = value;
    }

    [SerializeField] private Graphic graphic;
    private Material _mat;
    [SerializeField] private Image radialSlider;

    protected override void Awake()
    {
        base.Awake();

        _mat = new Material(graphic.material);
        graphic.material = _mat;

        _mat.SetFloat(INVERT_PROP, 1);

        //graphic.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        // TODO: Proper logic for this (as in this will replace the default cursor)
    }

    public enum State
    {
        Normal,
        Interactable,
    }
}
