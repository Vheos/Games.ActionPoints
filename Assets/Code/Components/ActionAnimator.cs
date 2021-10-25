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
        private const string ARM_LENGTH_UID = "ArmLength";
        private const string ARM_ROTATION_UID = "ArmRotation";
        private const string HAND_ROTATION_UID = "HandRotation";

        // Inspector
        public GameObject _Parent;

        // Publics
        public void AnimateState(ActionAnimation.StateData state)
        {
            if (state._ForwardDistanceEnabled)
                transform.AnimatePosition(this, GetComponent<Character>().CombatPosition + transform.right * state._ForwardDistance, state._Duration);

            if (state._ArmLengthEnabled)
                AnimationManager.Animate(this, ARM_LENGTH_UID, v => _arm.Length = v, _arm.Length, state._ArmLength, state._Duration);

            if (state._ArmRotationEnabled)
                _arm.transform.AnimateLocalRotation(this, ARM_ROTATION_UID, state._ArmRotation, state._Duration);

            if (state._HandRotationEnabled)
                _handTransform.AnimateLocalRotation(this, HAND_ROTATION_UID, state._HandRotation, state._Duration);
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