namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Tools.Extensions.Collections;
    using System;

    [RequireComponent(typeof(Character))]
    public class ActionAnimator : ABaseComponent
    {
        // Const
        private const string ARM_NAME = "Arm";
        private const string HAND_NAME = "Hand";

        // Inspector
        [SerializeField] protected GameObject _Parent;

        // CacheComponents
        protected override Type[] ComponentsTypesToCache => new[]
        {
            typeof(Character),
        };

        // Publics
        public Transform HandTransform
        { get; private set; }
        public void Animate(ActionAnimation.Clip clip)
        {
            ActionAnimation.Clip idle = Get<Character>().Idle;
            using (QAnimator.Group(this, null, clip.Duration, null, clip.Style))
            {                
                if (clip.ForwardDistanceEnabled)
                    transform.GroupAnimatePosition(Get<Character>().CombatPosition + transform.right * clip.GetForwardDistance(idle));
                if (clip.ArmLengthEnabled)
                    QAnimator.GroupAnimate(v => _arm.Length = v, _arm.Length, clip.GetArmLength(idle));
                if (clip.ArmRotationEnabled)
                    QAnimator.GroupAnimate(AssignArmAngles, _armAngles, clip.GetArmRotation(idle));
                if (clip.HandRotationEnabled)
                    QAnimator.GroupAnimate(AssignHandAngles, _handAngles, clip.GetHandRotation(idle));
            }
        }
        public void Animate(ActionAnimation.Clip[] clips, int clipIndex = 0)
        {
            if (clipIndex >= clips.Length)
                return;

            Animate(clips[clipIndex]);
            QAnimator.Delay(this, null, clips[clipIndex].TotalTime, () => Animate(clips, ++clipIndex));
        }

        // Privates
        private TransformArm _arm;
        private Vector3 _armAngles;
        private Vector3 _handAngles;
        private void FindOrCreateArm()
        {
            if (!_Parent.FindChild<TransformArm>(ARM_NAME).TryNonNull(out _arm))
                _arm = _Parent.CreateChildComponent<TransformArm>(ARM_NAME);
        }
        private void FindOrCreateHand()
        {
            HandTransform = _arm.FindChild<Transform>(HAND_NAME).TryNonNull(out var foundHandTransform)
                          ? foundHandTransform
                          : _arm.CreateChildGameObject(HAND_NAME).transform;
        }
        private void AssignArmAngles(Vector3 v)
        {
            _armAngles = v;
            _arm.transform.localRotation = Quaternion.Euler(_armAngles);
        }
        private void AssignHandAngles(Vector3 v)
        {
            _handAngles = v;
            HandTransform.localRotation = Quaternion.Euler(_handAngles);
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            FindOrCreateArm();
            FindOrCreateHand();
        }
    }
}