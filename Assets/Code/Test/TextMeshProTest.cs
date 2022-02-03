namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using TMPro;
    using UnityEditor;
    using UnityEngine;
    using Vheos.Games.Core;
    using Vheos.Tools.Extensions.General;

    public class TextMeshProTest : MonoBehaviour
    {
        public TextMeshPro TextMeshPro;
        public bool Log;

        private void Update()
        {
            if (!Log.Consume())
                return;

            Debug.Log($"{TextMeshPro.mesh.bounds}");
           // Debug.Log($"{TextMeshPro.meshFilter.sharedMesh.bounds}");
            Debug.Log($"{TextMeshPro.meshFilter.mesh.bounds}");
            Debug.Log($"{TextMeshPro.renderer.localBounds}");
            Debug.Log($"{TextMeshPro.renderedWidth} / {TextMeshPro.renderedHeight}");
            Debug.Log($"{TextMeshPro.preferredWidth} / {TextMeshPro.preferredHeight}");
        }
    }
}