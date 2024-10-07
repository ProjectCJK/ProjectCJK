using System;
using Interfaces;
using Units.Creatures.Units.Players;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour, IReferenceRegisterable<Transform>
    {
        [SerializeField] private float smoothTime = 0.3f;

        private Transform _player;
        private Vector3 velocity = Vector3.zero;

        public void RegisterReference(Transform player)
        {
            _player = player;
        }
        
        private void LateUpdate()
        {
            if (_player == null) return;
            
            var targetPosition = new Vector3(_player.position.x, _player.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}