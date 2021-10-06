namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    public class Character : AMousableSprite
    {
        // Inspector
        public GameObject _ActionUIPrefab = null;
        public Color _Color = Color.white;

        // Private
        private ActionUI _actionUI;

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
            base.PlayUpdate();
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