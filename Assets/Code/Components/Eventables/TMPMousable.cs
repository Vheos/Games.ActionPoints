namespace Vheos.Games.ActionPoints
{
    using TMPro;
    using UnityEngine;
    using Vheos.Tools.Extensions.General;
    using Vheos.Tools.Extensions.UnityObjects;

    sealed public class TMPMousable : Mousable
    {
        // Privates
        private TextMeshPro _tmp;
        private void TryFitBoxColliderOnTextChanged(Object tmp)
        {
            if (tmp == _tmp)
                TryFitBoxColliderToMesh();
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _tmp = GetComponent<TextMeshPro>();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(TryFitBoxColliderOnTextChanged);
        }
        protected override void PlayDestroy()
        {
            base.PlayDestroy();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(TryFitBoxColliderOnTextChanged);
        }
    }
}