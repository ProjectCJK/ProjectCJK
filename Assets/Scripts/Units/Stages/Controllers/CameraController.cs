using System;
using System.Collections;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private const float defaultSmoothTime = 0.3f;
        private const float specificSmoothTime = 1.3f;
        private const float specificZoomInDepth = 1f;
        private const float specificZoomInZoomSpeed = 0.2f;

        private Transform _playerTransform;

        private Vector3 _velocity = Vector3.zero;
        private bool _specificCameraOnline;
        private bool _isInitialized;
        private float _originalCameraSize;
        private Camera _camera;

        public void RegisterReference(Transform playerTransform)
        {
            _playerTransform = playerTransform;

            if (Camera.main != null)
            {
                _camera = Camera.main;
                _originalCameraSize = _camera.orthographicSize;
            }
        }

        public void ActivateFollowCamera(Vector3 targetPosition, bool zoomIn = false, Action onArrived = null)
        {
            StartCoroutine(FollowCamera(targetPosition, zoomIn, onArrived));
        }

        public void ActivateFollowCamera(Vector3 startPosition, Vector3 targetPosition, Action onArrived = null)
        {
            StartCoroutine(FollowCamera(startPosition, targetPosition, onArrived));
        }

        private void LateUpdate()
        {
            if (!_specificCameraOnline)
            {
                if (_playerTransform == null) return;

                var targetPosition = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, defaultSmoothTime);
                
                // 줌인 상태라면 크기를 원래대로 되돌리기
                if (Math.Abs(_camera.orthographicSize - _originalCameraSize) > 0.01f)
                {
                    _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _originalCameraSize, Time.deltaTime * 2);
                }
            }
        }

        private IEnumerator FollowCamera(Vector3 targetPosition, bool zoomIn = false, Action onArrived = null)
        {
            _specificCameraOnline = true;

            var originalSize = _camera.orthographicSize;
            var targetSize = zoomIn ? originalSize - specificZoomInDepth : originalSize; // 줌인 여부에 따라 크기 설정

            var newTargetPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

            while (Vector3.Distance(transform.position, newTargetPosition) > 0.1f || (zoomIn && Math.Abs(_camera.orthographicSize - targetSize) > 0.01f))
            {
                transform.position = Vector3.SmoothDamp(transform.position, newTargetPosition, ref _velocity, specificSmoothTime);

                if (zoomIn)
                {
                    _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetSize, specificZoomInZoomSpeed * Time.deltaTime);
                }

                yield return null;
            }

            if (zoomIn)
            {
                _camera.orthographicSize = targetSize;
            }

            onArrived?.Invoke();
            _specificCameraOnline = false;
        }
        
        private IEnumerator FollowCamera(Vector3 startPosition, Vector3 targetPosition, Action onArrived = null)
        {
            _specificCameraOnline = true;
            
            transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);
            var newTargetPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

            while (Vector3.Distance(transform.position, newTargetPosition) > 0.1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, newTargetPosition, ref _velocity, specificSmoothTime);
                yield return null;
            }

            onArrived?.Invoke();

            _specificCameraOnline = false;
        }
    }
}