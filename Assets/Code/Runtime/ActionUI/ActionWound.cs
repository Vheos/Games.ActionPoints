namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [RequireComponent(typeof(Expandable))]
    [DisallowMultipleComponent]
    abstract public class ActionWound : ABaseComponent
    {
        // Publics
        public ActionPoint Point
        { get; private set; }

        // Privates
        private Tween GetExpandTween()
        => this.NewTween()
            .SetDuration(1f);
        private Tween GetCollapseTween()
        => this.NewTween()
            .SetDuration(1f);

        // Play
        virtual public void Initialize(ActionPoint point)
        {
            Point = point;

            Get<Expandable>().OnStartExpanding.SubDestroy(this, Activate);
            Get<Expandable>().OnFinishCollapsing.SubDestroy(this, Deactivate);
            Get<Expandable>().ExpandTween.Set(GetExpandTween);
            Get<Expandable>().CollapseTween.Set(GetCollapseTween);
            Get<Expandable>().TryCollapse(true);
        }
    }
}