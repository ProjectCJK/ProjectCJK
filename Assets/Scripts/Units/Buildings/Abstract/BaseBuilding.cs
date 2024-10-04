using UnityEngine;

namespace Units.Buildings.Abstract
{
    public abstract class BaseBuilding : MonoBehaviour
    {
        public bool isSendable;
        public bool isReceivable;
        
        public abstract void RegisterReference();
        public abstract void Initialize();
    }
}