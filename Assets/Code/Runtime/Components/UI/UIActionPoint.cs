namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIActionPoint : AUIPoint
    {
        // Publics
        public void Initialize()
        {
            _uiWound = this.CreateChildComponent<UIWound>(UIManager.Settings.Prefab.Wound);
            _uiWound.Initialize();
            _drawable.Opacity = Settings.PartialProgressOpacity;
        }
        public void UpdateLocalProgresses(float action, float focus)
        {
            _drawable.ActionProgress = action.Abs().Sub(Index).Clamp01();
            _drawable.ActionColor = action >= 0 ? Settings.ActionColor : Settings.ExhaustColor;
            _drawable.FocusProgress = focus.Sub(Index).Clamp01();
        }
        public void ResetVisuals()
        {
            _drawable.Opacity = Settings.PartialProgressOpacity;
            _uiWound.Hide(true);
            UpdateOpacity(0, Character.Get<Actionable>().ActionPoints);
            UpdateWoundVisibility(0, Character.Get<Woundable>().WoundsCount);
        }

        // Privates
        private UIWound _uiWound;
        private void UpdateOpacity(int previous, int current)
        {
            previous = previous.Abs();
            current = current.Abs();
            if (Index >= previous && Index < current)
                _drawable.AnimateOpacity(1f, Settings.AnimDuration);
            else if (Index >= current && Index < previous)
                _drawable.AnimateOpacity(Settings.PartialProgressOpacity, Settings.AnimDuration);
        }
        private void UpdateWoundVisibility(int previous, int current)
        {
            int indexFromLast = Character.Get<Actionable>().MaxActionPoints - Index - 1;
            if (indexFromLast >= previous && indexFromLast < current)
                _uiWound.Show(Index);
            else if (indexFromLast >= current && indexFromLast < previous)
                _uiWound.Hide();
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            SubscribeTo(Character.Get<Actionable>().OnChangeActionPoints, UpdateOpacity);
            SubscribeTo(Character.Get<Woundable>().OnChangeWoundsCount, UpdateWoundVisibility);
        }
    }
}