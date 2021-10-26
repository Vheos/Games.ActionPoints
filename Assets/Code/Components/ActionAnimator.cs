namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    [RequireComponent(typeof(Character))]
    public class ActionAnimator : ABaseComponent
    {
        // Const
        private const string ARM_NAME = "Arm";
        private const string HAND_NAME = "Hand";

        // Inspector
       [SerializeField]  protected GameObject _Parent;

        // Publics
        public void AnimateStateThenIdle(ActionAnimation.StateData state)
        {
            void returnToIdle() => AnimateState(_character.Tool.Idle, null, QAnimator.AnimationStyle.InvertedArc);
            void stayUpAfterRelease() => QAnimator.Wait(this, null, state._WaitTime, returnToIdle);

            AnimateState(state, returnToIdle);
        }
        public void AnimateState(ActionAnimation.StateData state, System.Action finalAction = null, QAnimator.AnimationStyle style = QAnimator.AnimationStyle.Normal)
        {
            using (QAnimator.Group(this, null, state._Duration, finalAction, style))
            {
                if (state._ForwardDistanceEnabled)
                    transform.GroupAnimatePosition(_character.CombatPosition + transform.right * state._ForwardDistance);
                if (state._ArmLengthEnabled)
                    QAnimator.GroupAnimate(v => _arm.Length = v, _arm.Length, state._ArmLength);
                if (state._ArmRotationEnabled)
                    QAnimator.GroupAnimate(AssignArmAngles, _armAngles, state._ArmRotation);
                if (state._HandRotationEnabled)
                    QAnimator.GroupAnimate(AssignHandAngles, _handAngles, state._HandRotation);
            }
        }

        // Privates
        private Character _character;
        private TransformArm _arm;
        private Vector3 _armAngles;
        private Vector3 _handAngles;
        public Transform HandTransform
        { get; private set; }
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
            _character = GetComponent<Character>();
            FindOrCreateArm();
            FindOrCreateHand();
        }
    }
}