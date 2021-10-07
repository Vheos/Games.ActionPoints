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
        public QAnimColor _ActivityColorAnim = new QAnimColor();

        // Publics
        static public ActionButton Create(GameObject prefab, ActionWheel wheel, Action action)
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
        private Action _action;
        private Vector2 _originalScale;
        private void Initialize()
        {
            _spriteRenderer.sprite = _action.Sprite;
            _spriteRenderer.color = Wheel.UI.Character._Color;

            _originalScale = transform.localScale;
            _spriteRenderer.sortingOrder++;

            UpdateActiveStatus(Wheel.UI.Character.ActionPointsCount, Wheel.UI.Character.FocusPointsCount);
            Wheel.UI.Character.OnPointsCountChanged += UpdateActiveStatus;            
        }
        private void UpdateActiveStatus(int actionPointsCount, int focusPointsCount)
        {
            Color targetColor = actionPointsCount >= _action.ActionPointsCost ? Wheel.UI.Character._Color : Wheel._InactiveColor;
            _ActivityColorAnim.Start(_spriteRenderer.color, targetColor);
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
            if (_ActivityColorAnim.IsActive)
                _spriteRenderer.color = _ActivityColorAnim.Value;
        }
    }
}