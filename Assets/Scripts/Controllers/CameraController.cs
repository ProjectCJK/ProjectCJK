using System;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float smoothTime = 0.3f;

        private Vector3 velocity = Vector3.zero;

        private void LateUpdate()
        {
            var targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}