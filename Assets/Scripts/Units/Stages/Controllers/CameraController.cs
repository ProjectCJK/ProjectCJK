using Interfaces;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public class CameraController : MonoBehaviour, IRegisterReference<Transform>
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