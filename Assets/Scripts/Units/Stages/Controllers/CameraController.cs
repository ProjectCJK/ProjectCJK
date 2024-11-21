using System;
using Interfaces;
using Managers;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public class CameraController : MonoBehaviour, IRegisterReference
    {
        public Transform Transform;
        private Action OnArrived;
        
        [SerializeField] private float smoothTime = 0.3f;

        private Transform _target;
        private Vector3 _tempTarget;
        
        private Vector3 velocity = Vector3.zero;
        private bool tempCamera;
        private bool isInitialized;
        private float tempSmoothTime;

        public void RegisterReference()
        {
            Transform = transform;
        }

        public void FollowTarget(Transform target)
        {
            _target = target;
        }
        
        public void FollowTempTarget(Vector3 tempTarget, Action onArrived = null)
        {
            tempCamera = true;
            _tempTarget = tempTarget;
            tempSmoothTime = 1.3f;
            OnArrived = onArrived;
        }
        
        private void LateUpdate()
        {
            if (tempCamera)
            {
                var targetPosition = new Vector3(_tempTarget.x, _tempTarget.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, _tempTarget, ref velocity, tempSmoothTime);
                
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    OnArrived?.Invoke();
                    OnArrived = null;
                }
            }
            else
            {
                if (_target == null) return;

                var targetPosition = new Vector3(_target.position.x, _target.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
        
        public void UnregisterReference()
        {
            tempCamera = false;
        }
    }
}