namespace Vheos.Tools.UnityCore
{
    abstract public class ABaseComponent :
#if UNITY_EDITOR
        AEditable
#else
        APlayable
#endif
    { }
}

