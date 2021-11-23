namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;
    using Tools.Extensions.Math;
    using Event = Tools.UnityCore.Event;
    using static ActionAnimation;

    [RequireComponent(typeof(Character))]
    public class ActionAnimator : ABaseComponent
    {
        // Const
        private const string ARM_NAME = "Arm";
        private const string HAND_NAME = "Hand";

        // Inspector
        [SerializeField] protected GameObject _Parent;

        // Events
        public Event OnAnimationStopped
        { get; } = new Event();

        public bool IsPlaying
        { get; private set; }
        public Transform HandTransform
        { get; private set; }
        public void Animate(Clip clip)
        => Animate(new Clip[] { clip });
        public void Animate(IList<Clip> clips)
        {
            IsPlaying = true;
            AnimateClipsFrom(clips, 0);
        }
        public void Stop()
        => QAnimator.Stop(this, null);

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
        private void AnimateClip(Clip clip)
        {
            Clip idle = Get<Character>().Idle;
            using (QAnimator.Group(this, null, clip.Duration, null, clip.Style))
            {
                if (clip.ArmRotationEnabled)
                    QAnimator.GroupAnimate(AssignArmAngles, _armAngles, clip.ChooseArmRotation(idle));
                if (clip.ArmLengthEnabled)
                    QAnimator.GroupAnimate(v => _arm.Length = v, _arm.Length, clip.ChooseArmLength(idle));
                if (clip.HandRotationEnabled)
                    QAnimator.GroupAnimate(AssignHandAngles, _handAngles, clip.ChooseHandRotation(idle));
                if (clip.HandScaleEnabled)
                    HandTransform.GroupAnimateLocalScale(clip.ChooseHandScale(idle).ToVector3());
                if (clip.ForwardDistanceEnabled)
                    transform.GroupAnimatePosition(Get<Character>().CombatPosition + transform.right * clip.ChooseForwardDistance(idle));
                if (clip.LookAtEnabled)
                {
                    if (!TryGet<Combatable>(out var combatable)
                    || !combatable.IsInCombat)
                        return;

                    Vector3 targetPosition = clip.ChooseLookAt(idle) switch
                    {
                        Clip.LookAtTarget.AllyMidpoint => combatable.AllyMidpoint,
                        Clip.LookAtTarget.EnemyMidpoint => combatable.EnemyMidpoint,
                        Clip.LookAtTarget.CombatMidpoint => combatable.Combat.Midpoint,
                        _ => float.NaN.ToVector3(),
                    };

                    transform.GroupAnimateRotation(GetComponent<Character>().LookAtRotation(targetPosition));
                    QAnimator.Stop(GetComponent<Character>(), QAnimator.GetUID(QAnimator.ComponentProperty.TransformRotation));
                }
            }
        }
        private void AnimateClipsFrom(IList<Clip> clips, int clipIndex)
        {
            if (clipIndex >= clips.Count)
            {
                OnAnimationStopped?.Invoke();
                IsPlaying = false;
                return;
            }

            AnimateClip(clips[clipIndex]);
            QAnimator.Delay(this, null, clips[clipIndex].TotalTime, () => AnimateClipsFrom(clips, ++clipIndex));
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            FindOrCreateArm();
            FindOrCreateHand();
        }
    }
}