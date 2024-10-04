using System;
using Unity.VisualScripting;

namespace Units.Buildings.Interfaces
{
    public interface ISendObject
    {
        public event Action OnSendObject;
        public void SendObject();
    }
}