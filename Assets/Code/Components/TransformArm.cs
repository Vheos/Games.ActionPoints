namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class TransformArm : AUpdatable
    {
        // Inspector
        public Color _Color;
        [Range(0f, 1f)] public float _Length;

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

        // Privates


        // Mono
        public override void EditUpdate()
        {
            base.EditUpdate();
            Length = _Length;
        }

    }
}