namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using UnityEditor;
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public class MonoBehaviour_Editor : Editor
    {
        override public void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}