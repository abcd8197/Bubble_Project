using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonBase))]
public class ButtonBaseEditor : UnityEditor.EventSystems.EventTriggerEditor
{
    SerializedProperty destScale;
    SerializedProperty pointerDown_Duration;
    SerializedProperty pointerDown_Ease;
    SerializedProperty pointerUp_Duration;
    SerializedProperty pointerUp_Ease;

    private bool m_Toggle = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        destScale = serializedObject.FindProperty("DestScale");

        pointerDown_Duration = serializedObject.FindProperty("PointerDown_Duration");
        pointerDown_Ease = serializedObject.FindProperty("PointerDown_Ease");

        pointerUp_Duration = serializedObject.FindProperty("PointerUp_Duration");
        pointerUp_Ease = serializedObject.FindProperty("PointerUp_Ease");

        m_Toggle = false;
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        m_Toggle = EditorGUILayout.Foldout(m_Toggle, "Button Base");

        if (m_Toggle)
        {
            EditorGUILayout.PropertyField(destScale);

            EditorGUILayout.PropertyField(pointerDown_Duration);
            EditorGUILayout.PropertyField(pointerDown_Ease);

            EditorGUILayout.PropertyField(pointerUp_Duration);
            EditorGUILayout.PropertyField(pointerUp_Ease);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Event Trigger", EditorStyles.boldLabel);
        base.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }

}
