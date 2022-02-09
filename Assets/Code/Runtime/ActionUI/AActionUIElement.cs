namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using TMPro;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [DisallowMultipleComponent]
    abstract public class AActionUIElement<T> : ABaseComponent where T : ABaseComponent
    {
        // Publics
        public void SetIndex(int index)
        => _index = index;
        public void AnimateCreate(bool instantly = false)
        {
            transform.localScale = default;
            this.NewTween()
              .SetDuration(0.4f)
              .LocalScale(_originalScale)
              .FinishIf(instantly);
        }
        public void AnimateDestroy(bool instantly = false)
        {
            enabled = false;
            this.NewTween()
              .SetDuration(0.4f)
              .LocalScale(Vector3.zero)
              .AddEventsOnFinish(this.DestroyObject)
              .FinishIf(instantly);
        }
        public void AnimateMove(Vector3 targetLocalPosition, bool instantly = false)
        => this.NewTween(ConflictResolution.Interrupt)
            .SetDuration(0.4f)
            .LocalPosition(targetLocalPosition)
            .FinishIf(instantly);

        // Privates
        protected T _group;
        protected int _index;
        protected Vector3 _originalScale;

        // Play
        virtual public void Initialize(T group)
        {
            _group = group;
            _originalScale = transform.localScale;
            name = GetType().Name;
        }
    }
}