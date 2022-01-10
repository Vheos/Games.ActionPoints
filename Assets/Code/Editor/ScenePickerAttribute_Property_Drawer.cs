#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [CustomPropertyDrawer(typeof(ScenePickerAttribute))]
    public class ScenePickerAttribute_PropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
                return;

            var previousSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
            var currentSceneAsset = EditorGUI.ObjectField(rect, label, previousSceneAsset, typeof(SceneAsset), true);

            if (currentSceneAsset == null)
                property.stringValue = "";
            else if (currentSceneAsset != previousSceneAsset)
                property.stringValue = AssetDatabase.GetAssetPath(currentSceneAsset);
        }
    }
}
#endif