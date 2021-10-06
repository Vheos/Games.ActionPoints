namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class ActionButton : AMousableSprite
    {
        // Inspector
        [Range(1f, 2f)] public float _HighlightScaleMultiplier = 1.25f;
        public QAnimVector2 _HighlightScaleAnim = new QAnimVector2();
        public QAnimVector2 _AlignMoveAnim = new QAnimVector2();

        // Publics
        static public ActionButton Create(GameObject prefab, ActionWheel wheel, AAction action)
        {
            ActionButton newButton = Instantiate(prefab).GetComponent<ActionButton>();
            newButton.name = nameof(ActionButton);
            newButton.BecomeChildOf(wheel);
            newButton.Wheel = wheel;
            newButton._action = action;

            newButton.Initialize();
            return newButton;
        }
        public ActionWheel Wheel
        { get; private set; }
        public void MoveTo(Vector2 targetLocalPosition)
        => _AlignMoveAnim.Start(transform.localPosition.XY(), targetLocalPosition);

        // Privates
        private AAction _action;
        private Vector2 _originalScale;
        public void Initialize()
        {
            _spriteRenderer.sprite = _action.Sprite;
            _spriteRenderer.color = Wheel.UI.Character._Color;

            _originalScale = transform.localScale;
            _spriteRenderer.sortingOrder++;
        }

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
            if (_action != null)
                _action.Invoke(Wheel.UI.Character);
        }
        public override void MouseRelease(MouseManager.Button button)
        {
            base.MouseRelease(button);
        }

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_HighlightScaleAnim.IsActive)
                transform.localScale = _HighlightScaleAnim.Value;
            if (_AlignMoveAnim.IsActive)
                transform.localPosition = _AlignMoveAnim.Value;
        }
    }
}