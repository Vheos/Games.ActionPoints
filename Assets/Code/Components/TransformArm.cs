namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class TransformArm : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _Length;

        // Publics 
        public float Length
        {
            get => _Length;
            set
            {
                _Length = value;
                foreach (var childTransform in this.GetChildTransforms())
                    childTransform.localPosition = _Length.Append(childTransform.localPosition.YZ());
            }
        }

#if UNITY_EDITOR
        // Play
        [SerializeField] protected Color _GizmoColor;
        public Color GizmoColor
        => _GizmoColor;
        public override void EditUpdate()
        {
            base.EditUpdate();
            Length = _Length;
        }
#endif
    }
}