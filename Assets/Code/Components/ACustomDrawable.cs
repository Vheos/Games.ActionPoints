namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    abstract public class ACustomDrawable : AUpdatable
    {
        // Virtuals
        virtual protected void AssignInspectorMProps()
        { }

        // MProps
        protected float GetFloat(string name)
        => _mprops.GetFloat(name);
        protected Color GetColor(string name)
        => _mprops.GetColor(name);
        protected Texture GetTexture(string name)
        => _mprops.GetTexture(name);
        protected void SetFloat(string name, float value)
        {
            if (_mprops.GetFloat(name) == value)
                return;

            _mprops.SetFloat(name, value);
            _hasDirtyMProps = true;
        }
        protected void SetColor(string name, Color value)
        {
            if (_mprops.GetColor(name) == value)
                return;

            _mprops.SetColor(name, value);
            _hasDirtyMProps = true;
        }
        protected void SetTexture(string name, Texture value)
        {
            if (_mprops.GetTexture(name) == value)
                return;

            _mprops.SetTexture(name, value);
            _hasDirtyMProps = true;
        }

        // Privates
        protected MeshFilter _meshFilter;
        protected MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _mprops;
        private bool _hasDirtyMProps;
        private void InitializeMProps()
        => _mprops = new MaterialPropertyBlock();
        private void CacheMeshComponents()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }
        private void UpdateDirtyMProps()
        {
            if (_hasDirtyMProps.Consume())
                _meshRenderer.SetPropertyBlock(_mprops);
        }

        // Mono
        override public void PlayAwake()
        {
            base.PlayAwake();
            CacheMeshComponents();
            InitializeMProps();
            AssignInspectorMProps();
            UpdateDirtyMProps();
        }
        override public void PlayUpdate()
        {
            base.PlayUpdate();
            UpdateDirtyMProps();
        }

        // Editor
#if UNITY_EDITOR
        override public void EditAwake()
        {
            base.EditAwake();
            PlayAwake();
        }
        override public void EditInspect()
        {
            base.EditInspect();
            AssignInspectorMProps();
            UpdateDirtyMProps();
        }
#endif
    }
}