namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using UnityEditor;
    [CustomEditor(typeof(AExtendedMono), true)]
    [CanEditMultipleObjects]
    public class AExtendedMono_Editor : MonoBehaviour_Editor
    {
        // CONST
        static private readonly GUILayoutOption BUTTON_WIDTH = GUILayout.Width(75f);

        // Editor
        override public void OnInspectorGUI()
        {
            AExtendedMono castTarget = target as AExtendedMono;

            // Debug
            if (castTarget.ShowMonoDebug)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Build", BUTTON_WIDTH))
                    foreach (AExtendedMono multiTarget in targets)
                        multiTarget.OnAdd();
                if (GUILayout.Button("Rebuild", BUTTON_WIDTH))
                    foreach (AExtendedMono multiTarget in targets)
                        multiTarget.OnBuild();
                if (GUILayout.Button("Reinspect", BUTTON_WIDTH))
                    foreach (AExtendedMono multiTarget in targets)
                        multiTarget.OnInspect();
                if (GUILayout.Button("Repaint", BUTTON_WIDTH))
                    foreach (AExtendedMono multiTarget in targets)
                        multiTarget.OnRepaint();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            base.OnInspectorGUI();

            // Reinspect
            if (GUI.changed)
                foreach (AExtendedMono multiTarget in targets)
                    multiTarget.OnInspect();
        }
    }
}