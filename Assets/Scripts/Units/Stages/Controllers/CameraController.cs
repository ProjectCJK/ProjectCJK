using System;
using System.Collections;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private const float defaultSmoothTime = 0.3f;
        private const float specificSmoothTime = 1.3f;
        private const float specificZoomInDepth = 5f;
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
        
                // 줌인 상태라면 크기를 부드럽게 원래대로 되돌리기
                if (Math.Abs(_camera.orthographicSize - _originalCameraSize) > 0.01f)
                {
                    var recoverySpeed = specificZoomInZoomSpeed * Time.deltaTime; // 줌 풀 때 속도
                    _camera.orthographicSize = Mathf.MoveTowards(_camera.orthographicSize, _originalCameraSize, recoverySpeed);
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

            // 목표 줌 크기에 도달할 지점 계산 (0.9는 줌 완료 전 설정)
            var zoomTargetDistance = startDistance * 0.9f;

            while (Vector3.Distance(transform.position, newTargetPosition) > 0.1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, newTargetPosition, ref _velocity, specificSmoothTime);

                var currentDistance = Vector3.Distance(transform.position, newTargetPosition);

                // 줌 시작 조건: 특정 거리 이상에 도달했거나 줌 시작 지점 초과
                if (zoomIn && !zoomStarted && currentDistance <= startDistance * 0.6f)
                {
                    zoomStarted = true;
                }

                // 줌 진행: 목표 거리보다 가까워지거나 줌 시작
                if (zoomIn && (zoomStarted || currentDistance <= zoomTargetDistance))
                {
                    zoomSpeed += zoomAcceleration * Time.deltaTime; // 줌 속도 증가
                    _camera.orthographicSize = Mathf.MoveTowards(_camera.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

                    // 목표 줌 크기에 도달하면 루프 탈출
                    if (Mathf.Abs(_camera.orthographicSize - targetSize) < 0.01f)
                    {
                        break;
                    }
                }

                yield return null;
            }

            // 위치에 도달 후 줌이 완료되지 않았다면 최종적으로 크기 조정
            if (zoomIn && Mathf.Abs(_camera.orthographicSize - targetSize) > 0.01f)
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