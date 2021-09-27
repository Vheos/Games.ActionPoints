namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using Tools.Extensions.Math;
    [RequireComponent(typeof(Image))]
    public class UIActionButton : Button
    {
        // Inspector
        public float __HighlightedScale;
        public QAnimVector3 __ScaleAnimation;

        // Privates
        private Image _image;
        private Vector3 _originalScale;

        // Mouse
        override public void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
                __ScaleAnimation.Start(transform.localScale, __HighlightedScale.ToVector3());
        }
        override public void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            __ScaleAnimation.Start(transform.localScale, _originalScale);
        }
        override public void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Debug.Log($"Press");
        }
        override public void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            Debug.Log($"Release");
        }
        override public void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            Debug.Log($"Click");
        }

        // Mono
        protected override void Awake()
        {
            _image = GetComponent<Image>();
            _image.alphaHitTestMinimumThreshold = 0.5f;
            _image.SetA(0f);
            _originalScale = transform.localScale;
        }
        private void Update()
        {
            if (__ScaleAnimation.IsActive)
                transform.localScale = __ScaleAnimation.Value;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            targetGraphic = GetComponent<Image>();
            __HighlightedScale = 1.1f;
        }
#endif
    }
}