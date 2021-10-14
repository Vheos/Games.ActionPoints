namespace Vheos.Games.ActionPoints
{
    using Tools.UnityCore;
    using UnityEngine;
    abstract public class AFollowTarget : AUpdatable
    {
        // Inspector
        public Transform _Target = null;
        public Vector3 _Offset;
        public Axes _LockedAxes = 0;
        [Range(0f, 1f)] public float _HalfTime = 0.25f;

        // Publics
        abstract public void Follow(Transform target, float lerpAlpha);
        [System.Flags]
        public enum Axes
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2,
        }

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