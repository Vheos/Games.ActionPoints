#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Vheos.Tools.Extensions.General;

    [CustomPropertyDrawer(typeof(ActionEffectData))]
    public class ActionEffectData_PropertyDrawer : PropertyDrawer
    {
        private const float LINE_HEIGHT = 18;
        private const int LINES_COUNT = 1;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LINE_HEIGHT * LINES_COUNT;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Cache
            Rect originalRect = rect;
            var effectProperty = property.FindPropertyRelative(nameof(ActionEffectData.Effect));
            var valuesProperty = property.FindPropertyRelative(nameof(ActionEffectData.Values));
            float spacing = 5f;

            EditorGUI.BeginProperty(rect, label, property);
            {
                rect.height = LINE_HEIGHT;
                rect.x = originalRect.x;
                rect.width = originalRect.width / 2f - spacing;
                EditorGUI.PropertyField(rect, effectProperty, GUIContent.none);
                rect.x += originalRect.width / 2f;

                if (effectProperty.objectReferenceValue.TryAs(out ActionEffect actionEffect))
                {                    
                    rect.xMax = originalRect.xMax;
                    int valuesCount = actionEffect.RequiredValuesCount;
                    float valueWidth = rect.width / valuesCount;
                    rect.width  = valueWidth - spacing;
                    valuesProperty.arraySize = valuesCount;
                    for (int i = 0; i < valuesCount; i++)
                    {
                        EditorGUI.PropertyField(rect, valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);
                        rect.x += valueWidth;
                    }
                }
            }
        }
    }
}
#endif