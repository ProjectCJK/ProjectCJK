using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Modules
{
    [RequireComponent(typeof(TilemapCollider2D))]
    public class InteractionTrade : MonoBehaviour
    {
        public event Action OnReceiveObject;
        public event Action OnSendObject;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            OnReceiveObject?.Invoke();
            OnSendObject?.Invoke();
        }
    }
}