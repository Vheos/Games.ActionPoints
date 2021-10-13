namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    public interface IUIHierarchy
    {
        public UIBase UI
        { get; }
        public GameObject gameObject
        { get; }
    }
}