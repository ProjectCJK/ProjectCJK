using System;
using System.Collections;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private const float defaultSmoothTime = 0.3f;
        private const float specificSmoothTime = 1.3f;
        private const float specificZoomInDepth = 2f;
        private const float specificZoomInZoomSpeed = 2f;

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

            var zoomStarted = false; // 줌 시작 여부 플래그
            var startDistance = Vector3.Distance(transform.position, newTargetPosition);

            float zoomSpeed = 0; // 초기 줌 속도
            const float zoomAcceleration = 0.1f; // 줌 가속도

            while (Vector3.Distance(transform.position, newTargetPosition) > 0.1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, newTargetPosition, ref _velocity, specificSmoothTime);

                // 줌인 조건: 전체 거리의 80%에 도달했을 때 시작
                if (zoomIn && !zoomStarted)
                {
                    var currentDistance = Vector3.Distance(transform.position, newTargetPosition);
                    if (currentDistance <= startDistance * 0.6f)
                    {
                        zoomStarted = true; // 줌 시작
                    }
                }

                // 줌인 진행
                if (zoomIn && zoomStarted)
                {
                    zoomSpeed += zoomAcceleration * Time.deltaTime; // 줌 속도 증가
                    _camera.orthographicSize = Mathf.MoveTowards(_camera.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
                }

                yield return null;
            }

            if (zoomIn)
            {
                _camera.orthographicSize = targetSize; // 줌 최종 크기 설정
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