namespace Vheos.Games.ActionPoints
{
    using Tools.UnityCore;
    using UnityEngine;
    abstract public class AFollowTarget : AUpdatable
    {
        // Inspector
        public Transform _Target = null;
        [Range(0f, 1f)] public float _HalfTime = 0.25f;

        // Publics
        abstract public void Follow(Transform target, float lerpAlpha);

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_Target == null)
                return;

            Follow(_Target, NewUtility.LerpHalfTimeToAlpha(_HalfTime));
        }
    }
}