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
        private TextMeshPro _textMeshPro;

        private void Awake()
        {
            _textMeshPro = gameObject.AddComponent<TextMeshPro>();
            _textMeshPro.ForceMeshUpdate();
            Debug.Log($"Awake 1: {_textMeshPro.renderer.localBounds.size}");
            _textMeshPro.text = "SWORD CIACH";
            _textMeshPro.rectTransform.sizeDelta = Vector2.one;
            _textMeshPro.fontStyle = FontStyles.Bold;
            _textMeshPro.enableAutoSizing = true;
            _textMeshPro.fontSizeMin = 0f;
            _textMeshPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
            _textMeshPro.verticalAlignment = VerticalAlignmentOptions.Middle;
            _textMeshPro.ForceMeshUpdate();
            Debug.Log($"Awake 2: {_textMeshPro.renderer.localBounds.size}");
        }

        private void OnEnable()
        {
            Debug.Log($"OnEnable: {_textMeshPro.renderer.localBounds.size}");
        }
        private void Start()
        {
            Debug.Log($"Start: {_textMeshPro.renderer.localBounds.size}");
        }
        private void Update()
        {
            Debug.Log($"Update: {_textMeshPro.renderer.localBounds.size}");
        }
        private void LateUpdate()
        {
            Debug.Log($"LateUpdate: {_textMeshPro.renderer.localBounds.size}");
        }
    }
}