#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttribute_PropertyDrawer : PropertyDrawer
    {
        // Edit
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousGUIEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = previousGUIEnabled;
        }
    }
}
#endif