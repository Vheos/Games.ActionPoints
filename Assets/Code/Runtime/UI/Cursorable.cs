namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using TMPro;
    using Tools.Extensions.General;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class Cursorable : ABaseComponent
    {
        // Eventsd
        public readonly AutoEvent<UICursor, bool> OnGainHighlight = new();
        public readonly AutoEvent<UICursor, bool> OnLoseHighlight = new();
        public readonly AutoEvent<UICursor> OnPress = new();
        public readonly AutoEvent<UICursor> OnHold = new();
        public readonly AutoEvent<UICursor, bool> OnRelease = new();
        internal void TryInvokeOnGainHighlight(UICursor cursor)
        {
            _highlighters.Add(cursor);
            OnGainHighlight.Invoke(cursor, _highlighters.Count == 1);
        }
        internal void TryInvokeOnLoseHighlight(UICursor cursor)
        {
            _highlighters.Remove(cursor);
            OnLoseHighlight.Invoke(cursor, _highlighters.Count == 0);
        }
        internal void TryInvokeOnPress(UICursor cursor)
        {
            if (IsHeld)
                return;

            _holder = cursor;
            OnPress.Invoke(cursor);
        }
        internal void TryInvokeOnRelease(UICursor cursor, bool withinTrigger)
        {
            if (!IsHeldBy(cursor))
                return;

            _holder = null;
            OnRelease.Invoke(cursor, withinTrigger);
        }
        internal bool TryInvokeOnHold(UICursor cursor)
        {
            if (!IsHeldBy(cursor))
                return false;

            OnHold.Invoke(cursor);
            return true;
        }

        // Publics
        public Collider Trigger
        { get; private set; }
        public bool PerformRaycastTests(Vector3 position)
        {
            if (_raycastTests != null)
                foreach (Func<Vector3, bool> test in _raycastTests)
                    if (!test(position))
                        return false;
            return true;
        }
        public void AddRaycastTest(Func<Vector3, bool> test)
        => _raycastTests.Add(test);
        public bool IsHeld
        => _holder != null;
        public bool IsHeldBy(object holder)
        => ReferenceEquals(_holder, holder);

        // Privates
        private UICursor _holder;
        private HashSet<UICursor> _highlighters;
        private HashSet<Func<Vector3, bool>> _raycastTests;
        private RaycastTargetType _raycastTargetType;
        private RaycastTargetType FindRaycastTargetType()
        {
            foreach (var component in GetComponents<Component>())
                switch (component)
                {
                    case Collider _: return RaycastTargetType.Collider;
                    case SpriteRenderer _: return RaycastTargetType.Sprite;
                    case TextMeshPro _: return RaycastTargetType.TextMeshPro;
                }
            return RaycastTargetType.Collider;
        }
        private protected void TryFitBoxColliderToMesh()
        {
            if (Trigger.TryAs(out BoxCollider boxCollider)
            && TryGet(out MeshFilter meshFilter))
                boxCollider.size = meshFilter.mesh.bounds.size;
        }
        private bool SpriteRaycastTest(Vector3 position)
        {
            if (Get<SpriteRenderer>().sprite.TryNonNull(out var sprite)
            && sprite.texture.isReadable)
                return sprite.PositionToPixelAlpha(position, transform) >= 0.5f;
            return true;
        }
        private void TMPTryFitBoxColliderOnTextChanged(UnityEngine.Object tmp)
        {
            if (tmp == Get<TextMeshPro>())
                TryFitBoxColliderToMesh();
        }

        // Defines
        private enum RaycastTargetType
        {
            Collider,
            Sprite,
            TextMeshPro,
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _highlighters = new HashSet<UICursor>();
            _raycastTests = new HashSet<Func<Vector3, bool>>();
            _raycastTargetType = FindRaycastTargetType();
            Trigger = Get<Collider>();
            TryFitBoxColliderToMesh();

            switch (_raycastTargetType)
            {
                case RaycastTargetType.Sprite: AddRaycastTest(SpriteRaycastTest); break;
                case RaycastTargetType.TextMeshPro: TMPro_EventManager.TEXT_CHANGED_EVENT.Add(TMPTryFitBoxColliderOnTextChanged); break;
            }
        }
        protected override void PlayDestroy()
        {
            base.PlayDestroy();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(TMPTryFitBoxColliderOnTextChanged);
        }
    }
}