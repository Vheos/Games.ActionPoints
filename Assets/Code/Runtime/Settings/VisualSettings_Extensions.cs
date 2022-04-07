namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    static public class VisualSettings_Extensions
    {
        static public VisualSettings.UICursorSettings Settings(this UICursor _)
        => SettingsManager.Instance.VisualSettings.UICursor;
        static public VisualSettings.UITargetingLineSettings Settings(this UITargetingLine _)
        => SettingsManager.Instance.VisualSettings.UITargetingLine;
        static public VisualSettings.SpriteOutlineSettings Settings(this SpriteOutline _)
        => SettingsManager.Instance.VisualSettings.SpriteOutline;
        static public VisualSettings.ActionUIElementSettings Settings<T>(this AActionUIElementsGroup<T> _)
        => SettingsManager.Instance.VisualSettings.ActionUIElement;
        static public VisualSettings.ActionUIElementSettings Settings<T>(this AActionUIElement<T> _)
        => SettingsManager.Instance.VisualSettings.ActionUIElement;
        static public VisualSettings.ActionPointSettings Settings(this ActionPointsBar _)
        => SettingsManager.Instance.VisualSettings.ActionPoint;
        static public VisualSettings.ActionPointSettings Settings(this ActionPoint _)
        => SettingsManager.Instance.VisualSettings.ActionPoint;
        static public VisualSettings.ActionButtonSettings Settings(this ActionButtonsWheel _)
        => SettingsManager.Instance.VisualSettings.ActionButton;
        static public VisualSettings.ActionButtonSettings Settings(this ActionButton _)
        => SettingsManager.Instance.VisualSettings.ActionButton;
        static public VisualSettings.PopupSettings Settings(this Popup _)
        => SettingsManager.Instance.VisualSettings.Popup;

    }
}