namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class UIWound : ABaseComponent, IUIHierarchy
    {
        // Pubilcs
        public UIBase UI
        { get; private set; }
        public void Show(int index)
        {
            this.GOActivate();
            float randFrom = index.IsEven() ? Settings.WoundAngleRandomRange.AvgComp() : Settings.WoundAngleRandomRange.x;
            float randTo = index.IsEven() ? Settings.WoundAngleRandomRange.y : Settings.WoundAngleRandomRange.AvgComp();
            transform.localRotation = Quaternion.Euler(0f, 0f, -Random.Range(randFrom, randTo));

            Vector2 fadeInPosition = Vector2.up.Mul(Settings.FadeDistance).Rotate(transform.localRotation);
            using (QAnimator.Group(this, null, Settings.WoundAnimDuration))
            {
                transform.GroupAnimateLocalPosition(fadeInPosition, Vector2.zero);
                _spriteRenderer.GroupAnimateColor(_spriteRenderer.color.NewA(1f));
            }
        }
        public void Hide()
        {
            Vector2 fadeOutPosition = Vector2.right.Mul(Settings.FadeDistance).Rotate(transform.localRotation);
            using (QAnimator.Group(this, null, Settings.WoundAnimDuration, this.GODeactivate))
            {
                transform.GroupAnimateLocalPosition(fadeOutPosition);
                _spriteRenderer.GroupAnimateColor(_spriteRenderer.color.NewA(0f));
            }
        }

        // Privates
        private SpriteRenderer _spriteRenderer;
        private UISettings.WoundSettings Settings
        => UIManager.Settings.Wound;

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.color = UIManager.Settings.ActionPoint.ExhaustColor;
            Hide();
        }
    }
}