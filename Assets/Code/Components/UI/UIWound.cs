namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(SpriteRenderer))]
    public class UIWound : ABaseComponent, IUIHierarchy
    {
        // Publics
        public UIBase UI
        { get; private set; }
        public void Show(int index)
        {
            this.GOActivate();
            float randFrom = index.IsEven() ? Settings.WoundAngleRandomRange.AvgComp() : Settings.WoundAngleRandomRange.x;
            float randTo = index.IsEven() ? Settings.WoundAngleRandomRange.y : Settings.WoundAngleRandomRange.AvgComp();
            transform.localRotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(randFrom, randTo).Neg());

            Vector2 fadeInPosition = Vector2.up.Mul(Settings.FadeDistance).Rotate(transform.localRotation);
            using (QAnimator.Group(this, null, Settings.WoundAnimDuration))
            {
                transform.GroupAnimateLocalPosition(fadeInPosition, Vector2.zero);
                Get<SpriteRenderer>().GroupAnimateAlpha(1f);
            }
        }
        public void Hide(bool instantly = false)
        {
            Vector2 fadeOutPosition = Vector2.right.Mul(Settings.FadeDistance).Rotate(transform.localRotation);
            using (QAnimator.Group(this, null, instantly ? 0f : Settings.WoundAnimDuration, this.GODeactivate))
            {
                transform.GroupAnimateLocalPosition(fadeOutPosition);
                Get<SpriteRenderer>().GroupAnimateAlpha(0f);
            }
        }

        // Privates
        private UISettings.WoundSettings Settings
        => UIManager.Settings.Wound;

        // Play
        protected override void AddToComponentCache()
        {
            base.AddToComponentCache();
            AddToCache<SpriteRenderer>();
        }
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            Get<SpriteRenderer>().color = UIManager.Settings.ActionPoint.ExhaustColor;
            Hide(true);
        }
    }
}