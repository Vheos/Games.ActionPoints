namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(UIPrefabs), menuName = nameof(UIPrefabs))]
    public class UIPrefabs : ScriptableObject
    {
        // Inspector
        [SerializeField] protected GameObject _Base = null;
        [SerializeField] protected GameObject _TargetingLine = null;
        [SerializeField] protected GameObject _ActionPointsBar = null;
        [SerializeField] protected GameObject _ActionPoint = null;
        [SerializeField] protected GameObject _Wound = null;
        [SerializeField] protected GameObject _Wheel = null;
        [SerializeField] protected GameObject _Button = null;
        [SerializeField] protected GameObject _CostPointsBar = null;
        [SerializeField] protected GameObject _CostPoint = null;
        [SerializeField] protected GameObject _PopupHandler = null;
        [SerializeField] protected GameObject _DamagePopup = null;

        // Getters
        public GameObject Base
        => _Base;
        public GameObject TargetingLine
        => _TargetingLine;
        public GameObject ActionPointsBar
        => _ActionPointsBar;
        public GameObject ActionPoint
        => _ActionPoint;
        public GameObject Wound
        => _Wound;
        public GameObject Wheel
        => _Wheel;
        public GameObject Button
        => _Button;
        public GameObject CostPointsBar
        => _CostPointsBar;
        public GameObject CostPoint
        => _CostPoint;
        public GameObject PopupHandler
        => _PopupHandler;
        public GameObject DamagePopup
        => _DamagePopup;
    }
}