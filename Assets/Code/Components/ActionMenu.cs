namespace Vheos.Games.ActionPoints.Test
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Sprite;
    public class ActionMenu : AMousable
    {
        // Inspector
        [SerializeField] protected UIActionMenu __UIActionMenu;

        // Mouse
        public override void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            foreach (var uiActionMenu in FindObjectsOfType<UIActionMenu>())
                if (uiActionMenu != __UIActionMenu)
                    uiActionMenu.Collapse();
            __UIActionMenu.Expand();     
        }
        public override bool IsHitValid(Vector3 location)
        {
            if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                return spriteRenderer.sprite.PositionToPixelAlpha(location, transform) >= 0.5f;
            return true;
        }

        // Mono
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            __UIActionMenu.transform.position = CameraManager.FirstActive.WorldToScreenPoint(transform.position);
        }
    }
}