namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;

    public class Character : AMousableSprite
    {
        // Inspector
        public GameObject _ActionUIPrefab = null;
        public Color _Color = Color.white;
        public List<AAction> _Actions = new List<AAction>();
        public int _MaxPoints = 5;
        public float _ActionSpeed = 1f;
        public float _FocusSpeed = 0.5f;
        public float _ExhaustSpeed = 0.5f;
        public float _ActionProgress = 0f;
        public float _FocusProgress = 0f;

        // Private
        private ActionUI _actionUI;
        private void UpdateProgresses(float deltaTime)
        {
            _ActionProgress += deltaTime * _ActionSpeed;
            if(_ActionProgress > _MaxPoints)
            {
                deltaTime = (_ActionProgress - _MaxPoints) / _ActionSpeed;
                _ActionProgress = _MaxPoints;
                _FocusProgress += deltaTime * _FocusSpeed;
                _FocusProgress = _FocusProgress.ClampMax(_ActionProgress);
            }
        }

        // Mouse
        public override void MousePress(MouseManager.Button button, Vector3 location)
        {
            base.MousePress(button, location);
            _actionUI.ToggleWheel();
        }
        public override void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            _actionUI.CollapseOtherWheels();
            _actionUI.ExpandWheel();
        }

        // Mono
        public override void PlayUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                _ActionProgress -= _MaxPoints / 2f;

            base.PlayUpdate();
            UpdateProgresses(Time.deltaTime);
        }
        public override void PlayAwake()
        {
            base.PlayAwake();
            _actionUI = ActionUI.Create(_ActionUIPrefab, this);
            _spriteRenderer.color = _Color;
        }

#if UNITY_EDITOR
        public override void EditAwake()
        {
            base.EditAwake();
            GetComponent<SpriteRenderer>().color = _Color;
        }
#endif
    }
}