#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Vheos.Tools.Extensions.General;

    [CustomPropertyDrawer(typeof(ActionButtonVisuals))]
    public class ActionButtonVisuals_PropertyDrawer : PropertyDrawer
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
            var propSprite = property.FindPropertyRelative(nameof(ActionButtonVisuals.Sprite));
            var propText = property.FindPropertyRelative(nameof(ActionButtonVisuals.Text));
            var propColor = property.FindPropertyRelative(nameof(ActionButtonVisuals.Color));

            rect = EditorGUI.PrefixLabel(rect, label);
            bool isUsingSprite = propSprite.objectReferenceValue != null;

            float propWidth = rect.width / (isUsingSprite ? 2f : 4f);
            rect.width = propWidth - 5f;

            EditorGUI.PropertyField(rect, propSprite, GUIContent.none);
            rect.x += propWidth;
            if (!isUsingSprite)
            {
                rect.width = 2 * propWidth - 5f;
                EditorGUI.PropertyField(rect, propText, GUIContent.none);                
                rect.x += 2 * propWidth;
                rect.width = propWidth - 5f;
            }
            EditorGUI.PropertyField(rect, propColor, GUIContent.none);
            rect.x += propWidth;

        }

    }
}
#endif