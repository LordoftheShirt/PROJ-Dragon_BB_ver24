using FMOD;
using FMODUnity;
using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        private Vector2 _moveInput;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        public bool _oldGrounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        [Header("Input Action Names")]
        public string MoveActionName = "Player/Move";
        public string LookActionName = "Player/Look";
        public string JumpActionName = "Player/Jump";
        public string SprintActionName = "Player/Sprint";
        public string OpenMapActionName = "Player/OpenMap";

        [Header("Visuals/VFX")]
        [SerializeField] private PlayerCameraModifier camModifier;

        [Header("SFX")]
        [SerializeField] private EventReference footstepSFX;
        [SerializeField] private float footstepWalkFrequency = 1;
        [SerializeField] private float footstepRunFrequency = 1;
        private float _footstepTimer = 1;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = -53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // project-wide input actions
#if ENABLE_INPUT_SYSTEM
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _openMapAction;
#endif

        private CharacterController _controller;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
        

#if ENABLE_INPUT_SYSTEM
            if (InputSystem.actions != null)
            {
                _moveAction = InputSystem.actions.FindAction(MoveActionName);
                _lookAction = InputSystem.actions.FindAction(LookActionName);
                _jumpAction = InputSystem.actions.FindAction(JumpActionName);
                _sprintAction = InputSystem.actions.FindAction(SprintActionName);
                _openMapAction= InputSystem.actions.FindAction(OpenMapActionName);
                _openMapAction.performed += ReferenceManager.Instance.mapManager.ActiveteOrDeactivateMap;
            }
            else
            {
                //Debug.Log("No Project-wide Input Actions asset is assigned in Project Settings > Input System Package.");
            }
#else
			Debug.LogError("New Input System is not enabled in Player Settings.");
#endif

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

		private void Update()
		{
            _moveInput = _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;

            JumpAndGravity();
			GroundedCheck();
			Move();
            CamModifier();
			SFX();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
#if ENABLE_INPUT_SYSTEM
            Vector2 lookInput = _lookAction != null ? _lookAction.ReadValue<Vector2>() : Vector2.zero;

            if (lookInput.sqrMagnitude >= _threshold)
            {
                // Check if current input comes from a gamepad/stick vs mouse
                bool isGamepad = _lookAction.activeControl?.device is Gamepad;
                float deltaTimeMultiplier = isGamepad ? Time.deltaTime : 1.0f;

                _cinemachineTargetPitch -= lookInput.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = lookInput.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
#endif
        }

        private void Move()
        {
#if ENABLE_INPUT_SYSTEM
            bool isSprinting = _sprintAction != null && _sprintAction.IsPressed();

            float targetSpeed = isSprinting ? SprintSpeed : MoveSpeed;

            if (_moveInput == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _moveInput.magnitude > 1f ? 1f : _moveInput.magnitude;
            if (inputMagnitude == 0f) inputMagnitude = 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_moveInput.x, 0.0f, _moveInput.y).normalized;

            if (_moveInput != Vector2.zero)
            {
                inputDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            }

            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
#endif
        }


        private void CamModifier()
        {
            if (camModifier == null) return;

            camModifier.RevertBobbing = _moveInput == Vector2.zero || !Grounded;
            camModifier.RevertSway = !Grounded;

            // Our current move speed is our bobbing modifier
            // Divide by walk speed to make the walk speed the regular 1 modifier value
            float modifier = _speed / MoveSpeed;
            camModifier.BobbingModifier = modifier;
            camModifier.SwayModifier = modifier;
        }

        private void SFX()
        {
			if (_oldGrounded != Grounded)
			{
				_oldGrounded = Grounded;

				if (Grounded)
                {
					// LAND SFX HERE INSTEAD
                    _footstepTimer = 0;
                }
			}

            if (!Grounded)
            {
                _footstepTimer = 1;
                return;
            }

            float moveDelta = _speed / MoveSpeed;

            // Determine how frequently the footsteps should play
            float frequency;

            if (moveDelta > 1)
            {
                moveDelta = _speed / SprintSpeed;

                frequency = Mathf.LerpUnclamped(footstepWalkFrequency, footstepRunFrequency, moveDelta);
            }
            else
            {
                frequency = footstepWalkFrequency * moveDelta;
            }

            _footstepTimer -= Time.deltaTime * frequency;

            if (_footstepTimer <= 0)
            {
                _footstepTimer = 1;

				PlayFootstepSFX();
            }
        }

		private void PlayFootstepSFX()
		{
            var instance = RuntimeManager.CreateInstance(footstepSFX);

            // TODO: Different ground materials should have different footstep sounds. This is a placeholder for now.
            instance.setParameterByName("GroundType", 1);

            instance.start();
            instance.release();
            instance.clearHandle();
        }

		private void JumpAndGravity()
        {
#if ENABLE_INPUT_SYSTEM
            bool jumpPressed = _jumpAction != null && _jumpAction.WasPressedThisFrame();
            if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (jumpPressed && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
            }

            if (_verticalVelocity > _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
#endif
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
