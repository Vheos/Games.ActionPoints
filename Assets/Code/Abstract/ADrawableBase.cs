namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    abstract public class ADrawableBase : AExtendedMono
    {
        // Publics
        public Vector3 LocalSize
        => Mesh.bounds.size;

        // Virtuals
        abstract protected void InitializeInspectorFields();
        abstract protected void InitializeMatProps();
        abstract protected void AssignInspectorMProps();
        virtual protected Material Material
        => Global.Settings.SpriteMaterial;
        virtual protected Mesh Mesh
        => Global.Settings.SpriteMesh;

        // Privates
        protected AMatProps _matPropsBase;
        protected void Draw()
        {
            Graphics.DrawMesh(Mesh, transform.localToWorldMatrix,
                              Material, 0, null, 0, _matPropsBase.MPBlock);
        }

        // Mono
        override public void OnAwake()
        {
            InitializeMatProps();
            AssignInspectorMProps();
        }
        override public void OnUpdate()
        {
            Draw();
        }

        #region EDITOR
#if UNITY_EDITOR
        // Mono
        override public void OnAdd()
        {
            InitializeInspectorFields();
        }
        override public void OnBuild()
        {
            InitializeMatProps();
            AssignInspectorMProps();
        }
        override public void OnInspect()
        {
            AssignInspectorMProps();
        }
        override public void OnRepaint()
        {
            Draw();
        }
#endif
        #endregion
    }
}