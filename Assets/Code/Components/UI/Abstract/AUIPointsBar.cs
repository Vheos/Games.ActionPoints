namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    abstract public class AUIPointsBar<T> : AUpdatable, IUIHierarchy where T : AUIPoint
    {
        // Publics
        public UIBase UI
        { get; private set; }

        // Privates
        protected List<T> _points;
        protected void CreatePoints(int count, GameObject prefab)
        {
            for (int i = 0; i < count; i++)
                _points.Add(this.CreateChild<T>(prefab));
        }
        protected void AlignPoints()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                float localX = (i - _points.Count.Sub(1) / 2f) * (1 + UI._PointsSpacing) * _points[i].transform.localScale.x;
                _points[i].transform.SetLocalX(localX);
            }
        }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _points = new List<T>();
        }
    }
}