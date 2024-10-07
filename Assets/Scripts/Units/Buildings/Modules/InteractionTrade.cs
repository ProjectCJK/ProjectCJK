using System;
using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Units.Buildings.Modules
{
    [RequireComponent(typeof(TilemapCollider2D))]
    public class InteractionTrade : MonoBehaviour, IInitializable<Action, Action> 
    {
        public event Action OnSendObject;
        public event Action OnReceiveObject;
        
        public void Initialize(Action handle1, Action handle2)
        {
            OnSendObject += handle1;
            OnReceiveObject += handle2;
        }
                
        public void SendObject()
        {
            OnSendObject?.Invoke();
        }

        public void ReceiveObject()
        {
            OnReceiveObject?.Invoke();
        }
    }
}