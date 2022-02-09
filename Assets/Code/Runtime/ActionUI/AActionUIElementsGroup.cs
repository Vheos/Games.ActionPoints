namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [RequireComponent(typeof(Expandable))]
    [DisallowMultipleComponent]
    abstract public class AActionUIElementsGroup<T> : ABaseComponent
    {
        // Publics
        public ActionUI UI
        { get; private set; }

        // Privates
        protected readonly List<T> _elements = new();
        protected readonly HashSet<T> _newElements = new();

        // Play
        virtual public void Initialize(ActionUI ui)
        {
            UI = ui;
            name = GetType().Name;
            BindEnableDisable(ui);

            Get<Expandable>().OnStartExpanding.SubDestroy(this, Activate);
            Get<Expandable>().OnFinishCollapsing.SubDestroy(this, Deactivate);
            Get<Expandable>().ExpandTween.Set(() => this.NewTween().SetDuration(this.Settings().ExpandGroupDuration).LocalScale(Vector3.one));
            Get<Expandable>().CollapseTween.Set(() => this.NewTween().SetDuration(this.Settings().CollapseGroupDuration).LocalScale(Vector3.zero));
            Get<Expandable>().TryExpand(true);
        }
    }
}