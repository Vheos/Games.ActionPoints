namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    [RequireComponent(typeof(SpriteRenderer))]
    public class ActionButton : AMousableSprite
    {
        // Inspector
        [SerializeField] [Range(1f, 2f)] protected float _HighlightScaleMultiplier = 1.25f;
        [SerializeField] protected QAnimVector2 _HighlightScaleAnim = default;
        [SerializeField] protected QAnimVector2 _ExpandScaleAnim = default;
        [SerializeField] protected QAnimVector2 _ExpandMoveAnim = default;

        // Publics
        public void ExpandTo(Vector3 targetPosition)
        {
            _ExpandMoveAnim.Start(transform.localPosition, targetPosition);
            _ExpandScaleAnim.Start(transform.localScale, _originalScale);
            RecieveMouseEvents = true;
        }
        public void Collapse()
        {
            _ExpandMoveAnim.Start(transform.localPosition, Vector2.zero);
            _ExpandScaleAnim.Start(transform.localScale, Vector2.zero);
            _HighlightScaleAnim.Stop();
            RecieveMouseEvents = false;
        }

        // Privates
        private Vector2 _originalScale;

        // Mouse
        override public void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            _HighlightScaleAnim.Start(transform.localScale, _originalScale * _HighlightScaleMultiplier);
        }
        override public void MouseLoseHighlight()
        {
            base.MouseLoseHighlight();
            _HighlightScaleAnim.Start(transform.localScale, _originalScale);
        }
        public override void MousePress(MouseManager.Button button, Vector3 location)
        {
            base.MousePress(button, location);
            Debug.Log($"Pressed {button}");
        }
        public override void MouseRelease(MouseManager.Button button)
        {
            base.MouseRelease(button);
            Debug.Log($"Released {button}");
        }

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _originalScale = transform.localScale;
            _HighlightScaleAnim.SetMutuallyExclusiveWith(_ExpandScaleAnim);
        }
        private void Update()
        {
            if (_HighlightScaleAnim.IsActive)
                transform.localScale = _HighlightScaleAnim.Value;
            else if (_ExpandScaleAnim.IsActive)
                transform.localScale = _ExpandScaleAnim.Value;

            if (_ExpandMoveAnim.IsActive)
                transform.localPosition = _ExpandMoveAnim.Value;
        }
    }
}