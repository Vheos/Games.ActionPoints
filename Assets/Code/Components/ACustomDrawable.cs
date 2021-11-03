namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using System.Collections.Generic;
    using System.Linq;

    abstract public class ACustomDrawable : ABaseComponent
    {
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
        virtual protected void AssignInspectorMProps()
        { }
        virtual protected void InitializeRenderer(out Renderer renderer)
        => renderer = Get<Renderer>();

        private Renderer _renderer;
        private MaterialPropertyBlock _mprops;
        private bool _hasDirtyMProps;
        private void InitializeMProps()
        => _mprops = new MaterialPropertyBlock();
        private void UpdateDirtyMProps()
        {
            if (_hasDirtyMProps.Consume())
                _renderer.SetPropertyBlock(_mprops);
        }

        // Play
        protected override void AddToComponentCache()
        {
            base.AddToComponentCache();
            AddToCache<Renderer>();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdateLate += UpdateDirtyMProps;
        }
        override public void PlayAwake()
        {
            base.PlayAwake();
            InitializeRenderer(out _renderer);
            InitializeMProps();
            AssignInspectorMProps();
            UpdateDirtyMProps();
        }
    }
}

/*
// Editor
#if UNITY_EDITOR
override public void EditAwake()
{
    base.EditAwake();
    InitializeMProps();
    AssignInspectorMProps();
    UpdateDirtyMProps();
}
override public void EditInspect()
{
    base.EditInspect();
    AssignInspectorMProps();
    UpdateDirtyMProps();
}
#endif
*/