namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
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
        [SerializeField] protected GameObject _ArmAttachmentTransform;
        [SerializeField] protected Clip[] _Idle = new Clip[1];

        // Events
        public Event OnFinishAnimation
        { get; } = new Event();

        // Publics
        public bool IsPlaying
        { get; private set; }
        public Transform HandTransform
        { get; private set; }
        public IReadOnlyList<Clip> CurrentIdle
        => this.TryGetTool(out var tool) ? tool.Idle : _Idle;
        public void AnimateAction(Action action, ActionAnimation.Type type)
        {
            if (action.Animation == null)
                return;

            AnimateClips(AnimationTypeToClips(action, type));
        }
        public void AnimateClips(IReadOnlyList<Clip> clips)
        {
            IsPlaying = true;
            AnimateClipsRecursivelyFrom(clips, 0);
        }
        public void SetClip(Clip clip)
        {
            IsPlaying = false;
            AnimateClip(clip, true);
        }
        public void Stop()
        => QAnimator.Stop(this, null);

        // Privates
        private TransformArm _arm;
        private Vector3 _armAngles;
        private Vector3 _handAngles;
        private void FindOrCreateArm()
        {
            if (!_ArmAttachmentTransform.FindChild<TransformArm>(ARM_NAME).TryNonNull(out _arm))
                _arm = _ArmAttachmentTransform.CreateChildComponent<TransformArm>(ARM_NAME);
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
        private void AnimateClip(Clip clip, bool isInstant = false)
        {
            Clip idle = CurrentIdle.Last();
            using (QAnimator.Group(this, null, isInstant ? 0f : clip.Duration, null, clip.Style))
            {
                if (clip.ArmRotationEnabled)
                    QAnimator.GroupAnimate(AssignArmAngles, _armAngles, clip.ChooseArmRotation(idle));
                if (clip.ArmLengthEnabled)
                    QAnimator.GroupAnimate(v => _arm.Length = v, _arm.Length, clip.ChooseArmLength(idle));
                if (clip.HandRotationEnabled)
                    QAnimator.GroupAnimate(AssignHandAngles, _handAngles, clip.ChooseHandRotation(idle));
                if (clip.HandScaleEnabled)
                    HandTransform.GroupAnimateLocalScale(clip.ChooseHandScale(idle).ToVector3());
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

                    Get<RotateTowards>().SetTarget(targetPosition, true);
                }
                if (clip.ForwardDistanceEnabled)
                {
                    Vector3 startingPosition = TryGet<Combatable>(out var combatable) && combatable.IsInCombat
                                             ? combatable.AnchorPosition : transform.position;
                    transform.GroupAnimatePosition(startingPosition + transform.right * clip.ChooseForwardDistance(idle));
                }
            }
        }
        private void AnimateClipsRecursivelyFrom(IReadOnlyList<Clip> clips, int clipIndex)
        {
            if (clipIndex >= clips.Count)
            {
                OnFinishAnimation?.Invoke();
                IsPlaying = false;
                return;
            }

            AnimateClip(clips[clipIndex]);
            QAnimator.Delay(this, null, clips[clipIndex].TotalTime, () => AnimateClipsRecursivelyFrom(clips, ++clipIndex));
        }
        private IReadOnlyList<Clip> AnimationTypeToClips(Action action, ActionAnimation.Type type)
        => type switch
        {
            ActionAnimation.Type.Target => action.Animation.Target,
            ActionAnimation.Type.Use => action.Animation.Use,
            ActionAnimation.Type.Idle => CurrentIdle,
            ActionAnimation.Type.UseThenIdle => action.Animation.Use.Concat(CurrentIdle).ToArray(),
            _ => Array.Empty<Clip>(),
        };

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            FindOrCreateArm();
            FindOrCreateHand();
        }
    }
}