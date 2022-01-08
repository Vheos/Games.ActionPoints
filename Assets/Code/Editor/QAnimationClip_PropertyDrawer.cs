#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Tools.Extensions.General;
    using Tools.Extensions.Math;

    [CustomPropertyDrawer(typeof(QAnimationClip))]
    public class QAnimationClip_PropertyDrawer : PropertyDrawer
    {
        // Constants
        private const float PROPERTY_HEIGHT = 20f;
        private const float BOOL_WIDTH = 14f;
        private const string PROPERTY_USE_IDLE_POSTFIX = "UseIdle";
        static private readonly string[] TOGGLABLE_PROPERTIES = new[]
        {
            nameof(QAnimationClip.ArmRotation),
            nameof(QAnimationClip.ArmLength),
            nameof(QAnimationClip.HandRotation),
            nameof(QAnimationClip.HandScale),
            nameof(QAnimationClip.ForwardDistance),
            nameof(QAnimationClip.LookAt),
        };

        // Edit
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int setFlagCounts = property.FindPropertyRelative(nameof(QAnimationClip.Parameters)).intValue.SetFlagsCount();
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
                EditorGUI.LabelField(rect, $"{nameof(QAnimationClip.Duration)} / StayUp");
                EditorGUI.indentLevel--;
                rect.xMin = controlMinX;
                rect.width /= 2f;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(QAnimationClip.Duration)), GUIContent.none);
                rect.xMin += rect.width + 5f;
                rect.xMax = controlMaxX;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(QAnimationClip.StayUpTime)), GUIContent.none);
                rect.y += PROPERTY_HEIGHT;

                // Parameters & Style
                rect.xMin = labelMinX;
                EditorGUI.indentLevel++;
                EditorGUI.LabelField(rect, $"{nameof(QAnimationClip.Parameters)} / {nameof(QAnimationClip.Style)}");
                EditorGUI.indentLevel--;
                rect.xMin = controlMinX;
                rect.width /= 2f;
                SerializedProperty parametersProp = property.FindPropertyRelative(nameof(QAnimationClip.Parameters));
                EditorGUI.PropertyField(rect, parametersProp, GUIContent.none);
                rect.xMin += rect.width + 5f;
                rect.xMax = controlMaxX;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(QAnimationClip.Style)), GUIContent.none);
                rect.y += PROPERTY_HEIGHT;

                // Togglable properties                
                foreach (var name in TOGGLABLE_PROPERTIES)
                {
                    if (!System.Enum.TryParse<QAnimationClip.VisibleParameters>(name, out var enumValue)
                    || !parametersProp.intValue.Cast<QAnimationClip.VisibleParameters>().HasFlag(enumValue))
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