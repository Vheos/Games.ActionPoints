namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    [ExecuteAlways]
    abstract public class AExtendedMono : MonoBehaviour
    {
        #region PLAY & EDITOR
        // Publics
        virtual public void OnAwake()
        { }
        virtual public void OnStart()
        { }
        virtual public void OnUpdate()
        { }
        virtual public void OnFixedUpdate()
        { }

        // Wrappers
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            OnAwake();
        }                     // OnAwake
        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            OnStart();
        }                     // OnStart
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!_isBuilt)
                {
                    OnBuild();
                    _isBuilt = true;
                }
                OnRepaint();
                return;
            }
#endif
            OnUpdate();
        }                    // OnUpdate, Onbuild, OnRepaint
        private void FixedUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            OnFixedUpdate();
        }               // OnFixedUpdate
        #endregion
        #region EDITOR-ONLY
#if UNITY_EDITOR
        // Publics
        virtual public void OnAdd()
        { }            // when adding the component, before any Rebuild
        virtual public void OnBuild()
        { }          // when loading scene, entering edit mode, recompiling scripts
        virtual public void OnInspect()
        { }        // when changing anything in this object's inspector
        virtual public void OnRepaint()
        { }        // when changing anything in the scene

        // Privates
        private bool _isBuilt;

        // Wrappers
        private void Reset()
        {
            if (!Application.isPlaying)
                OnAdd();
        }                     // OnAdd
        [UnityEditor.Callbacks.DidReloadScripts]
        static private void DidScriptsReload()
        {
            foreach (var extendedEditor in FindObjectsOfType<AExtendedMono>())
                extendedEditor.OnBuild();
        }   // OnBuild

        // Debug
        [HideInInspector] public bool ShowMonoDebug;
        [ContextMenu("Toggle Mono Debug")]
        public void ToggleMonoDebug()
        => ShowMonoDebug = !ShowMonoDebug;
#endif
        #endregion
    }
}


/*
 *         private void Refresh()
        {
            OnRebuild();
            OnReinspect();
            OnRepaint();
        }
*/