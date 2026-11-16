using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraModifier : CinemachineComponentBase
{
    private const float FULL_ROTATION_RADIANS = Mathf.PI * 2;

    [Header("Bobbing")]
    [SerializeField] private bool canDoBobbing = true;
    [SerializeField] private float bobbingIntensity;
    [SerializeField] private float bobbingFrequency;
    [SerializeField] private float bobbingModifierMultiplier = 1;

    [Space]
    [SerializeField] private float bobbingRevertSpeed;

    public bool CanDoBobbing
    {
        get => canDoBobbing;
        set => canDoBobbing = value;
    }

    public bool RevertBobbing { get; set; } = false;

    public float BobbingModifier { get; set; } = 1;

    public float BobbingTimer
    {
        get => _bobbingTimer;
        set => _bobbingTimer = value;
    }
    private float _bobbingTimer;

    [Header("Sway")]
    [SerializeField] private bool canDoSway = true;

    [Space]
    [SerializeField] private Vector3 swayIntensity;
    [SerializeField] private Vector3 swayFrequency;
    [SerializeField] private float swayModifierMultiplier = 1;

    [Space]
    [SerializeField] private float swayFrequencyMoveMultiplier;
    [SerializeField] private Vector3 swayIntensityMoveMultiplier;

    [Space]
    [SerializeField] private float swayRevertSpeed;

    public bool CanDoSway
    {
        get => canDoSway;
        set => canDoSway = value;
    }

    public bool RevertSway { get; set; } = false;

    public float SwayModifier { get; set; } = 1;

    public Vector3 SwayTimer
    {
        get => _swayTimer;
        set => _swayTimer = value;
    }

    public override bool IsValid => enabled;

    public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Finalize;

    private Vector3 _swayTimer;

    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
        if (!IsValid)
            return;

        if (deltaTime >= 0)
        {
            curState.PositionCorrection = Bobbing(deltaTime);
            curState.OrientationCorrection = Quaternion.Euler(Sway(deltaTime));
        }
    }

    public Vector3 Bobbing(float deltaTime)
    {
        if (!canDoBobbing)
        {
            return Vector3.zero;
        }

        // Do head bobbing only when we are not reverting
        if (!RevertBobbing)
        {
            _bobbingTimer += deltaTime * bobbingFrequency * BobbingModifier * bobbingModifierMultiplier;

            // Prevent a possible timerift from being created due to an impossibly high value (It will take 1364871638 years before that happens)
            _bobbingTimer %= FULL_ROTATION_RADIANS;
        }
        // Revert the bobbing value back to 0
        else if (_bobbingTimer != 0)
        {
            _bobbingTimer = MoveTowardsZero(_bobbingTimer, bobbingRevertSpeed * deltaTime);
        }

        // Displace the local Y position based on our bobbing timer
        if (_bobbingTimer != 0)
        {
            Vector3 result = new Vector3(0, Mathf.Sin(_bobbingTimer) * bobbingIntensity, 0);

            return result;
        }
        // No bobbing, means no displacement
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 Sway(float deltaTime)
    {
        if (!canDoSway)
        {
            return Vector3.zero;
        }

        // Do swaying only when we are not reverting
        if (!RevertSway)
        {
            _swayTimer += deltaTime * swayFrequency * (SwayModifier * swayModifierMultiplier * swayFrequencyMoveMultiplier + 1);

            // Prevent a possible timerift from being created due to an impossibly high value (It will take 1364871638 years before that happens)
            _swayTimer.x %= FULL_ROTATION_RADIANS;
            _swayTimer.y %= FULL_ROTATION_RADIANS;
            _swayTimer.z %= FULL_ROTATION_RADIANS;
        }
        // Revert the sway value back to 0
        else
        {
            float maxDelta = swayRevertSpeed * deltaTime;

            _swayTimer.x = MoveTowardsZero(_swayTimer.x, maxDelta);
            _swayTimer.y = MoveTowardsZero(_swayTimer.y, maxDelta);
            _swayTimer.z = MoveTowardsZero(_swayTimer.z, maxDelta);
        }

        // ApplySettings rotation

        Vector3 result = new Vector3(
            Mathf.Sin(_swayTimer.x),
            Mathf.Sin(_swayTimer.y),
            Mathf.Sin(_swayTimer.z)
            );

        result.Scale(Vector3.Scale(swayIntensity, (SwayModifier * swayModifierMultiplier * swayIntensityMoveMultiplier) + Vector3.one));

        return result;
    }

    private float MoveTowardsZero(float timer, float maxDelta)
    {
        if (timer > Mathf.PI)
        {
            return Mathf.MoveTowards(timer, FULL_ROTATION_RADIANS, maxDelta);
        }
        else
        {
            return Mathf.MoveTowards(timer, 0, maxDelta);
        }
    }
}
