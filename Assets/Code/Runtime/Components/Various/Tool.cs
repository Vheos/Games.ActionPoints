namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using System.Linq;
    using System.Collections.Generic;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Tool : AAutoSubscriber
    {
        // Inspector
        [SerializeField] protected Vector3 _LocalPositionOffset = Vector3.zero;
        [SerializeField] protected Vector3 _LocalRotationOffset = Vector3.zero;
        [SerializeField] [Range(0f, 1f)] protected float _AnimDuration;

        // Publics
        public Equipable.Slot EquipSlot
        => Get<Equipable>().EquipSlot;
        public Equiper Equiper
        => Get<Equipable>().Equiper;
        public void AttachTo(Transform target)
        {
            transform.BecomeChildOf(target, true);
            this.NewTween()
                .SetDuration(_AnimDuration)
                .LocalPosition(_LocalPositionOffset)
                .LocalRotation(Quaternion.Euler(_LocalRotationOffset))
                .LocalScale(_originalScale);
        }
        public void DetachTo(Transform target)
        {
            transform.BecomeSiblingOf(target, true);

            this.NewTween()
                .SetDuration(_AnimDuration)
                .LocalPosition(target.localPosition)
                .LocalRotation(target.localRotation)
                .LocalScale(target.localScale);
        }

        // Privates
        private Vector3 _originalScale;
        private void OnGainTargeting(Targeter targeter, bool isFirst)
        {
            if (isFirst && Equiper == null || Equiper.SameGOAs(targeter))
                Get<SpriteOutline>().Show();
        }
        private void OnLoseTargeting(Targeter targeter, bool isLast)
        {
            if (isLast)
                Get<SpriteOutline>().Hide();
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Targetable>().OnGainTargeting, OnGainTargeting);
            SubscribeTo(Get<Targetable>().OnLoseTargeting, OnLoseTargeting);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _originalScale = transform.localScale;
            Get<Equipable>().EquipSlot.Set(() => Equipable.Slot.Hand);
        }
    }



    static public class Tool_Extensions
    {
        static public bool TryGetTool(this ABaseComponent t, out Tool r)
        {
            if (t.TryGet<Equiper>(out var equiper)
            && equiper.TryGetEquiped(Equipable.Slot.Hand, out var handEquipable)
            && handEquipable.TryGet(out r))
                return true;

            r = null;
            return false;
        }
    }
}