namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    public class ActionAnimator : APlayable
    {
        // Const
        private const string ARM_NAME = "Arm";
        private const string HAND_NAME = "Hand";

        // Inspector
        public GameObject _Parent;

        // Publics
        public void AnimateState(ActionAnimation.StateData state)
        {
            using (AnimationManager.Group(this, null, state._Duration))
            {
                if (state._ForwardDistanceEnabled)
                    transform.GroupAnimatePosition(GetComponent<Character>().CombatPosition + transform.right * state._ForwardDistance);
                if (state._ArmLengthEnabled)
                    AnimationManager.GroupAnimate(v => _arm.Length = v, _arm.Length, state._ArmLength);
                if (state._ArmRotationEnabled)
                    _arm.transform.GroupAnimateLocalRotation(state._ArmRotation);
                if (state._HandRotationEnabled)
                    _handTransform.GroupAnimateLocalRotation(state._HandRotation);
            }
        }

        // Privates
        private TransformArm _arm;
        private Transform _handTransform;
        private void FindOrCreateArm()
        {
            if (!_Parent.FindChild<TransformArm>(ARM_NAME).TryNonNull(out _arm))
                _arm = _Parent.CreateChildComponent<TransformArm>(ARM_NAME);
        }
        private void FindOrCreateHand()
        {
            if (!_arm.FindChild<Transform>(HAND_NAME).TryNonNull(out _handTransform))
                _handTransform = _arm.CreateChildGameObject(HAND_NAME).transform;
        }

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            FindOrCreateArm();
            FindOrCreateHand();
        }
    }
}