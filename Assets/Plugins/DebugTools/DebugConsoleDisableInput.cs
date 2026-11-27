using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugTools
{
    /// <summary>
    /// Script responsible for disabling <see cref="GameInput"/> when the <see cref="DebugConsole"/> is opened.
    /// <summary>
    public static class DebugConsoleDisableInput
    {
        private static bool _startCursorVisible;
        private static CursorLockMode _startCursorLockState;

        private static bool _actionsState;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Init()
        {
            //GameInputManager.OnLoadAsset += OnLoadGameInput;
            OnLoadGameInput();
        }

        public static void OnLoadGameInput()
        {
            DebugConsole.OnOpen += OnOpen;
            DebugConsole.OnClose += OnClose;
        }

        public static void OnOpen()
        {
            _startCursorVisible = Cursor.visible;
            _startCursorLockState = Cursor.lockState;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _actionsState = InputSystem.actions.enabled;
            InputSystem.actions.Disable();
        }

        public static void OnClose()
        {
            Cursor.visible = _startCursorVisible;
            Cursor.lockState = _startCursorLockState;

            if (_actionsState)
            {
                InputSystem.actions.Enable();
            }
            else
            {
                InputSystem.actions.Disable();
            }
        }
    }
}
