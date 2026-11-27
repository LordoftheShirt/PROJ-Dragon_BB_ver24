#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugTools
{
    /// <summary>
    /// Press F9 to pause the Unity Editor.
    /// <summary>
    public class ForcePauseEditorKeybind : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            DontDestroyOnLoad(new GameObject(nameof(ForcePauseEditorKeybind), typeof(ForcePauseEditorKeybind)));
        }

        private void Update()
        {
            if (Keyboard.current == null)
            {
                return;
            }

            if (!Keyboard.current.f9Key.wasPressedThisFrame)
            {
                return;
            }

            EditorApplication.isPaused = true;
        }
    }
}
#endif