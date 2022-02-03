#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Vheos.Tools.Extensions.General;

    [CustomPropertyDrawer(typeof(ComponentScriptReference))]
    public class ComponentScriptReference_PropertyDrawer : PropertyDrawer
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
            var assetPathProperty = property.FindPropertyRelative(nameof(ComponentScriptReference.AssetPath));
            var assemblyQualifiedNameProperty = property.FindPropertyRelative(nameof(ComponentScriptReference.AssemblyQualifiedName));
            var currentScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPathProperty.stringValue);
            var newScript = EditorGUI.ObjectField(rect, label, currentScript, typeof(MonoScript), false);

            if (newScript == null)
            {
                assetPathProperty.stringValue = "";
                assemblyQualifiedNameProperty.stringValue = "";
            }
            if (newScript != currentScript
            && newScript.TryAs(out MonoScript monoScript)
            && monoScript.GetClass().TryNonNull(out var monoScriptClass)
            && monoScriptClass.IsAssignableTo<Component>())
            {
                assetPathProperty.stringValue = AssetDatabase.GetAssetPath(newScript);
                assemblyQualifiedNameProperty.stringValue = monoScriptClass.AssemblyQualifiedName;
            }
        }

    }
}
#endif