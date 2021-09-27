namespace Vheos.Games.ActionPoints.Test
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    public class UIActionMenu : UIBehaviour
    {
        // Inspector
        [SerializeField] private UIActionButton[] __Buttons;
        [SerializeField] private QAnimFloat __TransparencyAnimation;

        // Privates
        public void Expand()
        {
            foreach (var button in __Buttons)
                __TransparencyAnimation.Start(button.image.color.a, 1f);
        }

        public void Collapse()
        {
            foreach (var button in __Buttons)
                __TransparencyAnimation.Start(button.image.color.a, 0f);
        }

        // Mono
        protected override void Awake()
        {
            base.Awake();
            __Buttons = GetComponentsInChildren<UIActionButton>();
        }
        public void Update()
        {
            if (__TransparencyAnimation.IsActive)
                foreach (var button in __Buttons)
                    button.image.SetA(__TransparencyAnimation.Value);
            // make buttons independent of menu
        }
    }
}