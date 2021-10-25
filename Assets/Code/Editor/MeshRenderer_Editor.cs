#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

    [CustomEditor(typeof(MeshRenderer))]
    public class MeshRendererSortingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            MeshRenderer renderer = target as MeshRenderer;

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int newId = DrawSortingLayersPopup(renderer.sortingLayerID);
            if (EditorGUI.EndChangeCheck())
                renderer.sortingLayerID = newId;            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            int order = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);
            if (EditorGUI.EndChangeCheck())
                renderer.sortingOrder = order;         
            EditorGUILayout.EndHorizontal();
        }

        int DrawSortingLayersPopup(int layerID)
        {
            SortingLayer[] layers = SortingLayer.layers;
            string[] names = layers.Select(l => l.name).ToArray();
            if (!SortingLayer.IsValid(layerID))
                layerID = layers[0].id;
            
            int layerValue = SortingLayer.GetLayerValueFromID(layerID);
            int newLayerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, names);
            return layers[newLayerValue].id;
        }
    }
}
#endif