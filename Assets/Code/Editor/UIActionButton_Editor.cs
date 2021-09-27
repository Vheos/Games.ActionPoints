#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEditor;
    using UnityEditor.UI;
    [CustomEditor(typeof(UIActionButton), true)]
    [CanEditMultipleObjects]
    public class UIActionButton_Editor : ButtonEditor
    {
        SerializedProperty __HighlightedScale, __ScaleAnimation;

        override protected void OnEnable()
        {
            base.OnEnable();
            __HighlightedScale = serializedObject.FindProperty(nameof(__HighlightedScale));
            __ScaleAnimation = serializedObject.FindProperty(nameof(__ScaleAnimation));
        }

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(__HighlightedScale);
            EditorGUILayout.PropertyField(__ScaleAnimation);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
#endif