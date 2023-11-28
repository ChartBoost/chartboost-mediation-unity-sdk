#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Attribute utilized to disable GUI interaction within the Unity Editor. Specially useful if you want to be able to see information on the Inspector, but don't want anyone to be able to modify it's content.
/// </summary>
public class GrayOut : PropertyAttribute
{
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(GrayOut))]
    public class GrayOutDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
    #endif
}
