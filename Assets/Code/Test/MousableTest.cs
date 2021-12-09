namespace Vheos.Games.ActionPoints.Test
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class MousableTest : AAutoSubscriber
    {
        // Inspector
        [Range(1f, 2f)] public float _HighlightedScale = 1.5f;
        [Range(0f, 1f)] public float _AnimDuration = 0.5f;

        // Privates
        private Vector3 _originalScale;
        private void OnGainHighlight()
        {
            Debug.Log($"{name} - GainHighlight");
            transform.AnimateLocalScaleRatio(_HighlightedScale.ToVector3(), _AnimDuration);
        }
        private void OnPress(CursorManager.MouseButton button, Vector3 location)
        {
            Debug.Log($"{name} - Pressed {button} at {location}");
        }
        private void OnHold(CursorManager.MouseButton button, Vector3 location)
        {
            // Debug.Log($"{name} - Held {button} at {location}");
        }
        private void OnRelease(CursorManager.MouseButton button, bool isClick)
        {
            Debug.Log($"{name} - {(isClick ? "Clicked" : "Released")} {button}");
        }
        private void OnLoseHighlight()
        {
            Debug.Log($"{name} - LoseHighlight");
            transform.AnimateLocalScaleRatio(_HighlightedScale.Inv().ToVector3(), _AnimDuration);
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            Mousable mousable = Get<Mousable>();
            SubscribeTo(mousable.OnGainHighlight, OnGainHighlight);
            SubscribeTo(mousable.OnPress, OnPress);
            SubscribeTo(mousable.OnHold, OnHold);
            SubscribeTo(mousable.OnRelease, OnRelease);
            SubscribeTo(mousable.OnLoseHighlight, OnLoseHighlight);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _originalScale = transform.localScale;
        }
    }
}