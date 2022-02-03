namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ComponentScriptReference
    {
        // Publics
        public string AssetPath;
        public string AssemblyQualifiedName;
        public Type Type
        => Type.GetType(AssemblyQualifiedName);
    }
}