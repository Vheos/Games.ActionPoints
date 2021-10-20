namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;

    public class AnimEvent_AllowDestroySelf : MonoBehaviour
    {
        public void DestroySelf()
        => this.DestroyObject();
    }
}