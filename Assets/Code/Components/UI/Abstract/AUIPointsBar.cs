namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    abstract public class AUIPointsBar<T> : ABaseComponent, IUIHierarchy where T : AUIPoint
    {
        // Publics
        public UIBase UI
        { get; private set; }

        // Privates
        protected List<T> _points;
        protected void CreatePoints(int count, GameObject prefab)
        {
            for (int i = 0; i < count; i++)
            {
                T newPoint = this.CreateChildComponent<T>(prefab);
                _points.Add(newPoint);
                newPoint.Index = i;
            }
        }
        protected void AlignPoints()
        {
            int index = 0;
            foreach (var alignmentVector in GetAlignmentVectors())
            {
                _points[index].transform.localPosition = alignmentVector * _points[index].transform.localScale.XY();
                index++;
            }
        }
        virtual protected IEnumerable<Vector2> GetAlignmentVectors()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                float localX = (i - _points.Count.Sub(1) / 2f) * (1 + UIManager.Settings._PointsSpacing);
                yield return new Vector2(localX, 0);
            }
        }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _points = new List<T>();
        }
    }
}