namespace Vheos.Games.ActionPoints.Test
{
    using System.Collections.Generic;
    using TMPro;
    using Tools.UnityCore;
    using UnityEngine;
    using UnityEngine.Events;

    public class TMPTest : AAutoSubscriber
    {
        TextMeshPro _previousTMP;
        Mesh _previousTMPMesh;
        Mesh _previousFilterMesh;
        string _previousText;

        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, Updatable_OnPlayUpdate);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _previousTMP = Get<TextMeshPro>();
            _previousText = Get<TextMeshPro>().text;
        }
        private void Updatable_OnPlayUpdate()
        {
            //Debug.Log($"{Time.frameCount} - {Get<MeshFilter>().mesh.bounds.size}");

            if (_previousTMP != Get<TextMeshPro>())
                Debug.Log($"\tTMP");

            if (_previousTMPMesh != Get<TextMeshPro>().mesh)
                Debug.Log($"\ttmp mesh");

            if (_previousFilterMesh != Get<MeshFilter>().mesh)
                Debug.Log($"\tfilter mesh");

            if (_previousText != Get<TextMeshPro>().text)
                Debug.Log($"\ttext");
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add((tmp) => { if (tmp == Get<TextMeshPro>()) Debug.Log($"text changed"); });

            _previousTMP = Get<TextMeshPro>();
            _previousTMPMesh = Get<TextMeshPro>().mesh;
            _previousFilterMesh = Get<MeshFilter>().mesh;
            _previousText = Get<TextMeshPro>().text;
        }
    }
}