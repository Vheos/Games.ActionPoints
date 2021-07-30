namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;
    public class CSpriteDrawable : ADrawable<CSpriteMatProps>
    {
        // Inspector
        [SerializeField] private Sprite __Sprite;
        [SerializeField] private Color __WhiteTint;
        [SerializeField] private Color __BlackTint;

        // Privates
        private Mesh _mesh;
        private void UpdateMeshGeometry()
        {
            // Cache
            Vector3[] originalVertices = Global.Settings.SpriteMesh.vertices;
            Vector3[] scaledVertices = new Vector3[originalVertices.Length];
            Vector3 scale = __Sprite.SizePixels() * Global.Settings.PixelSize;

            // Execute
            for (var i = 0; i < originalVertices.Length; i++)
                scaledVertices[i] = originalVertices[i].Mul(scale);
            _mesh = Instantiate(Global.Settings.SpriteMesh);
            _mesh.vertices = scaledVertices;
            _mesh.RecalculateBounds();
        }

        // Overrides
        override protected Mesh Mesh
        => _mesh;
        override protected void InitializeInspectorFields()
        {
            __Sprite = Global.Settings.WhitePixel;
            __WhiteTint = Color.white;
            __BlackTint = Color.black;
        }
        override protected void AssignInspectorMProps()
        {
            _matProps.Texture = __Sprite.Texture();
            _matProps.TextureST = __Sprite.SizeOffset01();
            _matProps.WhiteTint = __WhiteTint;
            _matProps.BlackTint = __BlackTint;
        }

        // Mono
        override public void OnAwake()
        {
            base.OnAwake();
            UpdateMeshGeometry();
        }

        #region EDITOR
#if UNITY_EDITOR
        // Privates
        private Sprite _previousSprite;
        private bool HasSpriteSizeChanged
        => __Sprite != _previousSprite
        && __Sprite.SizePixels() != _previousSprite.SizePixels();

        // Mono
        override public void OnBuild()
        {
            base.OnBuild();
            UpdateMeshGeometry();
            _previousSprite = Global.Settings.WhitePixel;
        }
        override public void OnInspect()
        {
            if (__Sprite == null)
                __Sprite = _previousSprite;

            AssignInspectorMProps();
            if (HasSpriteSizeChanged)
                UpdateMeshGeometry();

            _previousSprite = __Sprite;
        }
#endif
        #endregion
    }
}

