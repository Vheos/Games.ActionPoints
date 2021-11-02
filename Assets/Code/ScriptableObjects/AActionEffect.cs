namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    abstract public class AActionEffect : ScriptableObject
    {
        // Constants
        protected const string CONTEXT_MENU_PATH = "ActionEffects/";

        // Public
        abstract public void Invoke(Character user, Character target, params float[] values);

        // Private
        abstract protected int RequiredValuesCount
        { get; }

        // Defines
        [System.Serializable]
        public class Data
        {
            // Inspector     
            [SerializeField] protected AActionEffect _Effect = null;
            [SerializeField] protected Direction _Direction = Direction.FromTargetToUser;
            [SerializeField] protected float[] _Values = new float[1]; 

            // Privates
            private bool IsValid
            => _Effect != null
            && _Values.Length >= _Effect.RequiredValuesCount;

            // Publics
            public void Invoke(Character user, Character target)
            {
                if (!IsValid)
                    return;

                switch (_Direction)
                {
                    case Direction.FromUserToTarget: _Effect.Invoke(user, target, _Values); break;
                    case Direction.FromTargetToUser: _Effect.Invoke(target, user, _Values); break;
                }
            }

            // Defines
            protected enum Direction
            {
                FromUserToTarget,
                FromTargetToUser,
            }
        }
    }
}