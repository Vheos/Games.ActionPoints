namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class UIWound : AUpdatable, IUIHierarchy
    {
        // Inspector
        [Range(0f, 1f)] public float _AnimDuration = 0.5f;
        public Vector2 _AngleRandomRange = new Vector2(+30f, +60f);
        [Range(0f, 2f)] public float _FadeDistance = 1f;

        // Pubilcs
        public void Show()
        {
            this.GOActivate();
            transform.localRotation = Quaternion.Euler(0f, 0f, -Random.Range(_AngleRandomRange.x, _AngleRandomRange.y));
            Vector2 fadeInPosition = Vector2.up.Mul(_FadeDistance).Rotate(transform.localRotation);
            transform.AnimateLocalPosition(this, fadeInPosition, Vector2.zero, _AnimDuration);
            _spriteRenderer.AnimateColor(this, _spriteRenderer.color.NewA(1f), _AnimDuration);
        }
        public void Hide()
        {
            Vector2 fadeOutPosition = Vector2.right.Mul(_FadeDistance).Rotate(transform.localRotation);
            transform.AnimateLocalPosition(this, fadeOutPosition, _AnimDuration);
            _spriteRenderer.AnimateColor(this, _spriteRenderer.color.NewA(0f), _AnimDuration, false, this.GODeactivate);
        }

        // Privates
        private SpriteRenderer _spriteRenderer;

        public UIBase UI
        { get; private set; }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = UI._PointExhaustColor;
            Hide();
        }
    }
}