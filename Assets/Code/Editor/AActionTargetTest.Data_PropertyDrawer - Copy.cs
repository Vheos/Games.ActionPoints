#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Tools.Extensions.General;
    using Tools.Extensions.Math;

    [CustomPropertyDrawer(typeof(AActionTargetTest.Data))]
    public class AActionTargetTestData_PropertyDrawer : PropertyDrawer
    {
        // Constants
        private const float PROPERTY_HEIGHT = 20f;
        private const float BOOL_WIDTH = 14f;

        // Edit
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => PROPERTY_HEIGHT;        
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

                rect.xMin = controlMinX;
                SerializedProperty isInverted = property.FindPropertyRelative(nameof(AActionTargetTest.Data.TestForTrue));
                if (isInverted != null)
                {                    
                    rect.width = BOOL_WIDTH;
                    EditorGUI.PropertyField(rect, isInverted, GUIContent.none);
                    rect.xMin += rect.width + 5f;
                    rect.xMax = controlMaxX;
                }

                EditorGUI.PropertyField(rect, property.FindPropertyRelative(nameof(AActionTargetTest.Data.TargetTest)), GUIContent.none);
            }
            EditorGUI.EndProperty();
        }
    }
}
#endif