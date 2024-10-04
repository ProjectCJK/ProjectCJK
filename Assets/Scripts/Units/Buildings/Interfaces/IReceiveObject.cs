using System;

namespace Units.Buildings.Interfaces
{
    public interface IReceiveObject
    {
        public event Action OnReceiveObject;
        public void ReceiveObject();
    }
}