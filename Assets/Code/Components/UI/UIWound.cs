namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class UIWound : ABaseComponent, IUIHierarchy
    {
        // Pubilcs
        public void Show(int index)
        {
            this.GOActivate();
            float randFrom = index.IsEven() ? UIManager.Settings._WoundAngleRandomRange.AvgComp() : UIManager.Settings._WoundAngleRandomRange.x;
            float randTo = index.IsEven() ? UIManager.Settings._WoundAngleRandomRange.y : UIManager.Settings._WoundAngleRandomRange.AvgComp();
            transform.localRotation = Quaternion.Euler(0f, 0f, -Random.Range(randFrom, randTo));

            Vector2 fadeInPosition = Vector2.up.Mul(UIManager.Settings._FadeDistance).Rotate(transform.localRotation);
            using (QAnimator.Group(this, null, UIManager.Settings._WoundAnimDuration))
            {
                transform.GroupAnimateLocalPosition(fadeInPosition, Vector2.zero);
                _spriteRenderer.GroupAnimateColor(_spriteRenderer.color.NewA(1f));
            }
        }
        public void Hide()
        {
            Vector2 fadeOutPosition = Vector2.right.Mul(UIManager.Settings._FadeDistance).Rotate(transform.localRotation);
            using (QAnimator.Group(this, null, UIManager.Settings._WoundAnimDuration, this.GODeactivate))
            {
                transform.GroupAnimateLocalPosition(fadeOutPosition);
                _spriteRenderer.GroupAnimateColor(_spriteRenderer.color.NewA(0f));
            }
        }

        // Privates
        private SpriteRenderer _spriteRenderer;

        public UIBase UI
        { get; private set; }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = UIManager.Settings._PointExhaustColor;
            Hide();
        }
    }
}