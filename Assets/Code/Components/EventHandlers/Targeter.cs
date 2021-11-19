namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [DisallowMultipleComponent]
    public class Targeter : AEventSubscriber
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _LookAtAnimDuration;

        // Events
        public Event<Targetable, Targetable> OnChangeTarget
        { get; } = new Event<Targetable, Targetable>();

        // Publics
        private Targetable _target;
        public Targetable Target
        {
            get => _target;
            set
            {
                Targetable previousTarget = _target;
                _target = value;

                if (previousTarget != _target)
                {
                    OnChangeTarget?.Invoke(previousTarget, _target);
                    if (previousTarget != null)
                        previousTarget.LoseTargetingFrom(this);
                    if (_target != null)
                        _target.GainTargetingFrom(this);
                }
            }
        }
        public void TryLookAtTarget(bool allAxes = false)
        {
            if (Target == null)
                return;

            Vector3 targetAngles = Quaternion.LookRotation(this.DirectionTowards(Target)).eulerAngles;
            targetAngles.y -= 90f;
            if (!allAxes)
            {
                targetAngles.x = transform.position.x;
                targetAngles.z = transform.position.z;
            }           

            transform.AnimateRotation(this, Quaternion.Euler(targetAngles), _LookAtAnimDuration);
        }
    }
}