#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ActionAnimation.StateData))]
    public class ActionAnimationStateData_PropertyDrawer : PropertyDrawer
    {
        private const float PROPERTY_HEIGHT = 20f;
        private const float BOOL_WIDTH = 20f;
        private const string PROPERTY_ENABLED_POSTFIX = "Enabled";
        static private readonly string[] TOGGLABLE_PROPERTIES = new[]
        {
            nameof(ActionAnimation.StateData._ForwardDistance),
            nameof(ActionAnimation.StateData._ArmLength),
            nameof(ActionAnimation.StateData._ArmRotation),
            nameof(ActionAnimation.StateData._HandRotation),
        };

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            {
                Rect originalRect = rect;
                rect.height = PROPERTY_HEIGHT;

                // Label
                EditorGUI.LabelField(rect, label);
                rect.y += PROPERTY_HEIGHT;

                EditorGUI.indentLevel++;
                EditorGUI.PrefixLabel(rect, new GUIContent(nameof(ActionAnimation.StateData._Duration)));
                rect.xMin = EditorGUIUtility.labelWidth;
                rect.width /= 2f;
                
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ActionAnimation.StateData._Duration)), GUIContent.none);
                rect.x += rect.width;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ActionAnimation.StateData._WaitTime)), GUIContent.none);

                rect.y += PROPERTY_HEIGHT;

                // SubProperties
                
                foreach (var name in TOGGLABLE_PROPERTIES)
                {
                    GUI.enabled = true;
                    rect.xMin = EditorGUIUtility.labelWidth;
                    rect.width = BOOL_WIDTH;
                    SerializedProperty enabledProp = property.FindPropertyRelative(name + PROPERTY_ENABLED_POSTFIX);
                    if (enabledProp != null)
                    {
                        EditorGUI.PropertyField(rect, enabledProp, GUIContent.none);
                        GUI.enabled = enabledProp.boolValue;
                        rect.xMin += BOOL_WIDTH;
                    }
                    rect.xMax = originalRect.xMax;
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative(name), GUIContent.none);

                    rect.xMin = originalRect.xMin;
                    EditorGUI.LabelField(rect, name);

                    rect.y += PROPERTY_HEIGHT;
                }
                EditorGUI.indentLevel--;

            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => (TOGGLABLE_PROPERTIES.Length + 2) * PROPERTY_HEIGHT;
    }
}
#endif