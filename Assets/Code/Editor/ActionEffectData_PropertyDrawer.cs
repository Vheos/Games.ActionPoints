#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Vheos.Tools.Extensions.General;

    [CustomPropertyDrawer(typeof(ActionEffectData))]
    public class ActionEffectData_PropertyDrawer : PropertyDrawer
    {
        private const int LINES_COUNT = 1;
        private const int SPACING_X = 4;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        => EditorGUIUtility.singleLineHeight * LINES_COUNT;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            // Cache
            var propEffects = property.FindPropertyRelative(nameof(ActionEffectData.Effect));
            var propValues = property.FindPropertyRelative(nameof(ActionEffectData.Values));
            var propSubject = property.FindPropertyRelative(nameof(ActionEffectData.Subject));
            var propObject = property.FindPropertyRelative(nameof(ActionEffectData.Object));
            float originalWidth = rect.width;
            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(rect.ConsumeX(originalWidth * 1 / 3f, false, SPACING_X), propEffects, GUIContent.none);
            if (!propEffects.objectReferenceValue.TryAs(out ActionEffect actionEffect))
                return;

            EditorGUI.PropertyField(rect.ConsumeX(originalWidth * 1 / 6f, false, SPACING_X), propSubject, GUIContent.none);
            EditorGUI.PropertyField(rect.ConsumeX(originalWidth * 1 / 6f, false, SPACING_X), propObject, GUIContent.none);

            int valuesCount = actionEffect.RequiredValuesCount;
            float valueWidth = originalWidth * 1 / 3f / valuesCount;
            propValues.arraySize = valuesCount;
            for (int i = 0; i < valuesCount; i++)
                EditorGUI.PropertyField(rect.ConsumeX(valueWidth, false, SPACING_X), propValues.GetArrayElementAtIndex(i), GUIContent.none);
        }
    }
}
#endif