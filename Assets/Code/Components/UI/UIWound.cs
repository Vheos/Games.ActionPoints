namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class UIWound : APlayable, IUIHierarchy
    {
        // Inspector
        [Range(0f, 1f)] public float _AnimDuration = 0.5f;
        public Vector2 _AngleRandomRange = new Vector2(+30f, +60f);
        [Range(0f, 2f)] public float _FadeDistance = 1f;

        // Pubilcs
        public void Show(int index)
        {
            this.GOActivate();
            float randFrom = index.IsEven() ? _AngleRandomRange.AvgComp() : _AngleRandomRange.x;
            float randTo = index.IsEven() ? _AngleRandomRange.y : _AngleRandomRange.AvgComp();
            transform.localRotation = Quaternion.Euler(0f, 0f, -Random.Range(randFrom, randTo));

            Vector2 fadeInPosition = Vector2.up.Mul(_FadeDistance).Rotate(transform.localRotation);
            using (AnimationManager.Group(this, null, _AnimDuration))
            {
                transform.GroupAnimateLocalPosition(fadeInPosition, Vector2.zero);
                _spriteRenderer.GroupAnimateColor(_spriteRenderer.color.NewA(1f));
            }
        }
        public void Hide()
        {
            Vector2 fadeOutPosition = Vector2.right.Mul(_FadeDistance).Rotate(transform.localRotation);
            using (AnimationManager.Group(this, null, _AnimDuration, this.GODeactivate))
            {
                transform.GroupAnimateLocalPosition(fadeOutPosition);
                _spriteRenderer.GroupAnimateColor(_spriteRenderer.color.NewA(0f));
            }
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