namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    public class ActionPointDrawable : ADrawable<ActionPointMatProps>
    {
        // Inspector
        [SerializeField] private Color __BackgroundTint;
        [SerializeField] private Color __ProgressTint;
        [Range(0, 1)]
        [SerializeField] private float __Opacity;
        [Range(0, 1)]
        [SerializeField] private float __Progress;

        // Overrides
        override protected Material Material
        => Global.Settings.ActionPointMaterial;
        override protected void InitializeInspectorFields()
        {
            __BackgroundTint = Color.black;
            __ProgressTint = Color.white;
            __Opacity = 1f;
            __Progress = 0.5f;
        }
        override protected void AssignInspectorMProps()
        {
            _matProps.BackgroundTint = __BackgroundTint;
            _matProps.ProgressTint = __ProgressTint;
            _matProps.Opacity = __Opacity;
            _matProps.Progress = __Progress;
        }
    }
}