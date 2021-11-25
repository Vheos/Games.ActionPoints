namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using System.Linq;
    using System.Collections.Generic;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Tool : AEventSubscriber
    {
        // Inspector
        [SerializeField] protected Vector3 _LocalPositionOffset = Vector3.zero;
        [SerializeField] protected Vector3 _LocalRotationOffset = Vector3.zero;
        [SerializeField] [Range(0f, 1f)] protected float _AnimDuration;
        [SerializeField] protected ToolAnimationSet _AnimationSet;

        // Publics
        public IReadOnlyList<ActionAnimation.Clip> Idle
        => _AnimationSet.Idle;
        public Equipable.Slot EquipSlot
        => Get<Equipable>().EquipSlot;
        public Equiper Equiper
        => Get<Equipable>().Equiper;
        public bool IsUnsheathed
        { get; private set; }
        public void TrySheathe()
        {
            if (!IsUnsheathed)
                return;

            IsUnsheathed = false;
            Equiper.Get<ActionAnimator>().AnimateClips(_AnimationSet.Sheathe);
        }
        public void TryUnsheathe()
        {
            if (IsUnsheathed)
                return;

            IsUnsheathed = true;
            Equiper.Get<ActionAnimator>().SetClip(_AnimationSet.Sheathe.Last());
            Equiper.Get<ActionAnimator>().AnimateClips(_AnimationSet.Idle);
        }

        // Privates
        private void AttachToEquiper(System.Action nextAction = null)
        {
            Equiper.Get<ActionAnimator>().SetClip(Idle.Last());
            transform.BecomeChildOf(Equiper.AttachmentTransform[EquipSlot], true);
            IsUnsheathed = true;
            using (QAnimator.Group(this, null, _AnimDuration, nextAction))
            {
                transform.GroupAnimateLocalPosition(_LocalPositionOffset);
                transform.GroupAnimateLocalRotation(_LocalRotationOffset);
            }
        }
        private void OnChangeEquiper(Equiper from, Equiper to)
        {
            if (to != null)
                AttachToEquiper(TrySheathe);
            else
                ;
        }
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
            SubscribeTo(Get<Equipable>().OnChangeEquiper, OnChangeEquiper);
            SubscribeTo(Get<Targetable>().OnGainTargeting, OnGainTargeting);
            SubscribeTo(Get<Targetable>().OnLoseTargeting, OnLoseTargeting);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
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