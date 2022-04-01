namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.UnityObjects;

    [DisallowMultipleComponent]
    abstract public class AActionUIElement<TElementGroup> : ABaseComponent
    {
        // Publics
        public void SetIndex(int index)
        => _index = index;
        public void AnimateCreate(bool instantly = false)
        {
            transform.localScale = default;
            this.NewTween()
              .SetDuration(this.Settings().CreateElementDuration)
              .LocalScale(_originalScale)
              .FinishIf(instantly);
        }
        public void AnimateDestroy(bool instantly = false)
        {
            IsEnabled = false;
            this.NewTween()
              .SetDuration(this.Settings().DestroyElementDuration)
              .LocalScale(Vector3.zero)
              .AlphaRatio(ColorComponent.Any, 0f)
              .AddEventsOnFinish(this.DestroyObject)
              .FinishIf(instantly);
        }
        public void AnimateMove(Vector3 targetLocalPosition, bool instantly = false)
        => this.NewTween(ConflictResolution.Interrupt)
            .SetDuration(this.Settings().MoveElementDuration)
            .LocalPosition(targetLocalPosition)
            .FinishIf(instantly);

        // Privates
        protected TElementGroup _group;
        protected int _index;
        protected Vector3 _originalScale;

        // Play
        virtual public void Initialize(TElementGroup group)
        {
            _group = group;
            _originalScale = transform.localScale;
        }
    }
}