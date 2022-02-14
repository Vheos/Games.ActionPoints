namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.General;
    using Vheos.Tools.Extensions.Math;

    static public class PopupManager
    {
        // Publics
        static public void PopText(Vector3 position, string text, Color color, float scale = 1f, float pulseRate = 0f, float pulseScale = 1f)
        => GameObject.Instantiate<Popup>(SettingsManager.Prefabs.Popup).Initialize(position, text, color, scale, pulseRate, pulseScale, _isOddPopup.Toggle());
        static public void PopDamage(Vector3 position, float amount, bool hasDealtWound)
        => PopDamageOrHealing(false, position, amount, hasDealtWound);
        static public void PopHealing(Vector3 position, float amount, bool hasHealedWound)
        => PopDamageOrHealing(true, position, amount, hasHealedWound);

        // Privates
        static private bool _isOddPopup;
        static private void PopDamageOrHealing(bool isHealing, Vector3 position, float amount, bool hasAffectedWound)
        {
            var settings = isHealing ? SettingsManager.Visual.HealingPopup : SettingsManager.Visual.DamagePopup ;
            var lerpAlpha = hasAffectedWound ? 1f : amount / 100f;
            var text = (isHealing ? "+" : "") + amount.RoundDown().ToString();
            var color = settings.ColorCurve.Evaluate(lerpAlpha).NewA(0f);
            var scale = settings.SizeCurve.Evaluate(lerpAlpha);
            var pulseRate = hasAffectedWound ? settings.WoundPulseRate : 0f;
            var pulseScale = hasAffectedWound ? settings.WoundPulseScale : 1f;
            PopText(position, text, color, scale, pulseRate, pulseScale);
        }       

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            _isOddPopup = false;
        }
    }
}