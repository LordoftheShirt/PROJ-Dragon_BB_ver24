using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

public class CustomButton : Button
{
    private ButtonSettings _settings;
    private EventSystem _eventSystem;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif
        onClick.AddListener(OnClick);
    }

    protected override void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif
        _settings = ButtonSettings.Instance;
    }

    private void OnClick()
    {
        RuntimeManager.PlayOneShot(_settings.ClickSFX);
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif
            /* Copy pasta from old project
            switch (state)
            {
                case SelectionState.Normal:
                    _buttonScaling.Highlighted = false;
                    break;

                case SelectionState.Highlighted:
                    _buttonScaling.Highlighted = true;

                    SoundManager.PlaySound("UI Select");
                    break;

                case SelectionState.Pressed:
                    _buttonScaling.Highlighted = true;
                    break;

                case SelectionState.Selected:
                    _buttonScaling.Highlighted = true;

                    SoundManager.PlaySound("UI Select");
                    break;

                case SelectionState.Disabled:
                    _buttonScaling.Highlighted = false;
                    break;
            }
            */
#if UNITY_EDITOR
        }
#endif

        base.DoStateTransition(state, instant);
    }

    private void Update()
    {
        /* Copy pasta from old project
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif
        if (_eventSystem == null)
        {
            _eventSystem = EventSystem.current;
        }

        bool thisIsSelected = _eventSystem.currentSelectedGameObject == gameObject;

        if (thisIsSelected && GameInput.Jump.Down && !_buttonScaling.Pressed)
        {
            SoundManager.PlaySound("UI Press Select", true);
            _buttonScaling.Pressed = true;
        }
        else if (!thisIsSelected && _buttonScaling.Pressed)
        {
            SoundManager.PlaySound("UI Press Unselect", true);
            _buttonScaling.Pressed = false;
        }

        if (_buttonScaling.Pressed && GameInput.Jump.Up)
        {
            onClick?.Invoke();
            _buttonScaling.Pressed = false;
        }
        */
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CustomButton), true)]
public class CustomButtonEditor : ButtonEditor
{
    private static readonly HashSet<string> _blacklistedProps = new()
    {
        "m_Script",
        "m_Navigation",
        "m_Transition",
        "m_Colors",
        "m_SpriteState",
        "m_AnimationTriggers",
        "m_Interactable",
        "m_TargetGraphic",
        "m_OnClick",
    };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            string propPath = prop.propertyPath;

            if (_blacklistedProps.Contains(propPath)) continue;

            EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif