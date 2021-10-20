namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    abstract public class AActionScript : ScriptableObject
    {
        // Public
        abstract public void Invoke(Character user, Character target, params float[] values);

        // Private
        abstract protected int RequiredValuesCount
        { get; }

        // Defines
        [System.Serializable]
        public struct Data
        {
            // Inspector
            [SerializeField] private AActionScript _Script;
            [SerializeField] private float[] _Values;
            [SerializeField] private bool _SwapUserAndTarget;

            // Privates
            private bool IsValid
            => _Script != null
            && _Values.Length >= _Script.RequiredValuesCount;

            // Publics
            public void Invoke(Character user, Character target)
            {
                if (!IsValid)
                    return;

                if (_SwapUserAndTarget)
                    _Script.Invoke(target, user, _Values);
                else
                    _Script.Invoke(user, target, _Values);
            }
        }
    }
}