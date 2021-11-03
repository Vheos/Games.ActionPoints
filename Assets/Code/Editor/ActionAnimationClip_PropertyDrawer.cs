#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Tools.Extensions.General;
    using Tools.Extensions.Math;

    [CustomPropertyDrawer(typeof(ActionAnimation.Clip))]
    public class ActionAnimationClip_PropertyDrawer : PropertyDrawer
    {
        // Constants
        private const float PROPERTY_HEIGHT = 20f;
        private const float BOOL_WIDTH = 14f;
        private const string PROPERTY_USE_IDLE_POSTFIX = "UseIdle";
        static private readonly string[] TOGGLABLE_PROPERTIES = new[]
        {
            nameof(ActionAnimation.Clip.ArmRotation),
            nameof(ActionAnimation.Clip.ArmLength),
            nameof(ActionAnimation.Clip.HandRotation),
            nameof(ActionAnimation.Clip.HandScale),
            nameof(ActionAnimation.Clip.ForwardDistance),
        };

        // Edit
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int setFlagCounts = property.FindPropertyRelative(nameof(ActionAnimation.Clip.Parameters)).intValue.SetFlagsCount();
            return (3 + setFlagCounts.ClampMax(TOGGLABLE_PROPERTIES.Length)) * PROPERTY_HEIGHT;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            {
                rect.height = PROPERTY_HEIGHT;
                Rect controlRect = EditorGUI.PrefixLabel(rect, new GUIContent(" "));
                float labelMinX = rect.xMin;
                float controlMinX = controlRect.xMin;
                float controlMaxX = controlRect.xMax;

                // Label
                rect.xMin = labelMinX;
                EditorGUI.LabelField(rect, label);
                rect.y += PROPERTY_HEIGHT;

                // Duration & WaitTime
                rect.xMin = labelMinX;
                EditorGUI.indentLevel++;
                EditorGUI.LabelField(rect, $"{nameof(ActionAnimation.Clip.Duration)} / StayUp");
                EditorGUI.indentLevel--;
                rect.xMin = controlMinX;
                rect.width /= 2f;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ActionAnimation.Clip.Duration)), GUIContent.none);
                rect.xMin += rect.width + 5f;
                rect.xMax = controlMaxX;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ActionAnimation.Clip.StayUpTime)), GUIContent.none);
                rect.y += PROPERTY_HEIGHT;

                // Parameters & Style
                rect.xMin = labelMinX;
                EditorGUI.indentLevel++;
                EditorGUI.LabelField(rect, $"{nameof(ActionAnimation.Clip.Parameters)} / {nameof(ActionAnimation.Clip.Style)}");
                EditorGUI.indentLevel--;
                rect.xMin = controlMinX;
                rect.width /= 2f;
                SerializedProperty parametersProp = property.FindPropertyRelative(nameof(ActionAnimation.Clip.Parameters));
                EditorGUI.PropertyField(rect, parametersProp, GUIContent.none);
                rect.xMin += rect.width + 5f;
                rect.xMax = controlMaxX;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(ActionAnimation.Clip.Style)), GUIContent.none);
                rect.y += PROPERTY_HEIGHT;

                // Togglable properties                
                foreach (var name in TOGGLABLE_PROPERTIES)
                {
                    if (!System.Enum.TryParse<ActionAnimation.Clip.VisibleParameters>(name, out var enumValue)
                    || !parametersProp.intValue.As<ActionAnimation.Clip.VisibleParameters>().HasFlag(enumValue))
                        continue;

                    GUI.enabled = true;
                    rect.xMin = controlMinX;
                    SerializedProperty useIdleProp = property.FindPropertyRelative(name + PROPERTY_USE_IDLE_POSTFIX);
                    if (useIdleProp != null)
                    {

                        rect.width = BOOL_WIDTH;
                        EditorGUI.PropertyField(rect, useIdleProp, GUIContent.none);
                        rect.xMin += rect.width + 5f;
                        rect.xMax = controlMaxX;
                        GUI.enabled = !useIdleProp.boolValue;
                    }

                    EditorGUI.PropertyField(rect, property.FindPropertyRelative(name), GUIContent.none);

                    rect.xMin = labelMinX;
                    EditorGUI.indentLevel++;
                    EditorGUI.LabelField(rect, name);
                    EditorGUI.indentLevel--;
                    rect.y += PROPERTY_HEIGHT;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
#endif