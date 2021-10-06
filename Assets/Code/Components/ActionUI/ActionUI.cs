namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    public class ActionUI : AUpdatable
    {
        // Inspector
        public GameObject _WheelPrefab = null;
        public GameObject _ButtonPrefab = null;
        public GameObject _PointsBarPrefab = null;

        // Publics
        static public ActionUI Create(GameObject prefab, Character character)
        {
            ActionUI newUI = Instantiate(prefab).GetComponent<ActionUI>();
            newUI.name = nameof(ActionUI);
            newUI.BecomeChildOf(GameObject.Find("UI"));
            newUI.Character = character;

            newUI.Initialize();
            return newUI;
        }
        public Character Character
        { get; private set; }
        public void ToggleWheel()
        => _wheel.Toggle();
        public void CollapseOtherWheels()
        {
            foreach (var wheel in FindObjectsOfType<ActionWheel>())
                if (wheel.IsExpanded && wheel != _wheel)
                    wheel.CollapseButtons();
        }
        public void ExpandWheel()
        {
            if (!_wheel.IsExpanded)
                _wheel.ExpandButtons();
        }

        // Privates
        private ActionWheel _wheel;
        private ActionPointsBar _pointsBar;
        private void Initialize()
        {
            _wheel = ActionWheel.Create(_WheelPrefab, this);
            _pointsBar = ActionPointsBar.Create(_PointsBarPrefab, this);
        }
    }
}