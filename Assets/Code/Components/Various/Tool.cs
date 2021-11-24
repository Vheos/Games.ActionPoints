namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using System.Linq;

    [RequireComponent(typeof(SpriteRenderer))]
    public class Tool : AEventSubscriber
    {
        // Inspector
        [SerializeField] protected Vector3 _LocalPositionOffset = Vector3.zero;
        [SerializeField] protected Vector3 _LocalRotationOffset = Vector3.zero;
        [SerializeField] [Range(0f, 1f)] protected float _AnimDuration;
        [SerializeField] protected ActionAnimation.Clip[] _Unsheathe = new ActionAnimation.Clip[1];
        [SerializeField] protected ActionAnimation.Clip[] _Sheathe = new ActionAnimation.Clip[1];

        // Publics
        public ActionAnimation.Clip Idle
        => _Unsheathe.Last();
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
            Equiper.Get<ActionAnimator>().Animate(_Sheathe);
        }
        public void TryUnsheathe()
        {
            if (IsUnsheathed)
                return;

            IsUnsheathed = true;
            Equiper.Get<ActionAnimator>().SetClip(_Sheathe.Last());
            Equiper.Get<ActionAnimator>().Animate(_Unsheathe);
        }

        // Privates
        private void AttachToEquiper(System.Action nextAction = null)
        {
            Equiper.Get<ActionAnimator>().SetClip(Idle);
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
}