namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;
    using System;

    [RequireComponent(typeof(LineRenderer))]
    public class UITargetingLine : ABaseComponent, IUIHierarchy
    {
        // Cache
        protected override Type[] ComponentsTypesToCache => new[]
        {
            typeof(LineRenderer),
        };

        // Publics
        public UIBase UI
        { get; private set; }
        public void Activate(Transform from, Transform to)
        {
            _from = from;
            _to = to;
            _isDeactivating = false;
            UpdatePositionsAndTiling();
            this.GOActivate();
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, Settings.StartWidth, Settings.WidthAnimDuration);
        }
        public void Deactivate()
        {
            _isDeactivating = true;
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, 0f, Settings.WidthAnimDuration, this.GODeactivate);
        }
        public void UpdatePositionsAndTiling()
        {
            Get<LineRenderer>().SetPosition(0, _from.position);
            Get<LineRenderer>().SetPosition(1, _to.position);
            Get<LineRenderer>().sharedMaterial.mainTextureScale = new Vector2(_from.DistanceTo(_to) * Settings.Tiling, 1);
        }
        public bool TryGetCursorCharacter(out Character target)
        {
            if (CursorManager.CursorMousable.TryNonNull(out var mousable)
            && mousable.TryGetComponent<Character>(out var character))
            {
                target = character;
                return true;
            }
            target = null;
            return false;
        }

        // Privates
        private Transform _from;
        private Transform _to;
        private bool _isDeactivating;
        private UISettings.TargetingLineSettings Settings
        => UIManager.Settings.TargetingLine;
        private void SetWidth(float width)
        {
            Get<LineRenderer>().startWidth = width;
            Get<LineRenderer>().endWidth = width * Settings.EndWidthRatio;
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            Get<LineRenderer>().positionCount = 2;
            Get<LineRenderer>().startColor = UI.Character.Color.NewA(Settings.StartOpacity);
            this.GODeactivate();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                if (!_isDeactivating)
                    UpdatePositionsAndTiling();
            };
        }
    }
}