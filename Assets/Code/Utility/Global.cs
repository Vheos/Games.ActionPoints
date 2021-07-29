namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    static public class Global
    {
        // Publics 
        static public Settings Settings
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return GameObject.FindObjectOfType<Settings>();
#endif
                return _cachedSettings;
            }
        }

        // Privates
        static private Cached<Settings> _cachedSettings;

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void Initialize()
        {
            _cachedSettings = new Cached<Settings>(GameObject.FindObjectOfType<Settings>);
        }
    }
}