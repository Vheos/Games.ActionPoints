namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    abstract public class ACustomDrawable : ABaseComponent
    {
        // Cache
        protected override Type[] ComponentsTypesToCache => new[]
        {
            typeof(MeshRenderer),
        };

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
        private MaterialPropertyBlock _mprops;
        private bool _hasDirtyMProps;
        private void InitializeMProps()
        => _mprops = new MaterialPropertyBlock();
        private void UpdateDirtyMProps()
        {
            if (_hasDirtyMProps.Consume())
                Get<MeshRenderer>().SetPropertyBlock(_mprops);
        }

        // Play
        override public void PlayAwake()
        {
            base.PlayAwake();
            InitializeMProps();
            AssignInspectorMProps();
            UpdateDirtyMProps();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                UpdateDirtyMProps();
            };
        }

        // Editor
#if UNITY_EDITOR
        override public void EditAwake()
        {
            base.EditAwake();
            // Cache components
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
    }
}