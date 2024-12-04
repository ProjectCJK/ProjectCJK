using System;
using System.Collections;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private const float defaultSmoothTime = 0.3f;
        private const float specificSmoothTime = 1.3f;
        private const float specificZoomInDepth = 3f; // 줌 깊이 감소

        private Transform _playerTransform;

        private Vector3 _velocity = Vector3.zero;
        private bool _specificCameraOnline;
        private float _originalCameraSize;
        private Camera _camera;

        private Vector3 _currentCameraPosition; // 현재 카메라 위치 저장

        // 쉐이킹 관련 변수
        private bool _isShaking; // 쉐이킹 활성화 여부
        private Vector3 _shakeOffset; // 현재 쉐이킹 오프셋
        private float _shakeTimeRemaining; // 쉐이킹 지속 시간
        private float[] _shakePattern = { -0.04f, 0.06f, -0.02f }; // 흔들림 패턴
        private int _currentShakeIndex; // 현재 흔들림 단계
        private float _shakeStepDuration = 0.05f; // 각 흔들림 단계의 지속 시간
        private float _timeSinceLastShake; // 단계 간 시간 경과

        public void RegisterReference(Transform playerTransform)
        {
            _playerTransform = playerTransform;

            if (Camera.main != null)
            {
                _camera = Camera.main;
                _originalCameraSize = _camera.orthographicSize;
                _currentCameraPosition = _camera.transform.position; // 초기 카메라 위치 저장
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

        public void ShakeCameraEffect()
        {
            _isShaking = true; // 쉐이킹 활성화
            _shakeTimeRemaining = _shakePattern.Length * _shakeStepDuration; // 총 쉐이킹 지속 시간 설정
            _currentShakeIndex = 0; // 쉐이킹 단계 초기화
            _timeSinceLastShake = 0f; // 단계 간 시간 초기화
            _shakeOffset = Vector3.zero; // 초기 쉐이킹 오프셋 초기화
        }

        private void LateUpdate()
        {
            if (!_specificCameraOnline)
            {
                if (_playerTransform == null) return;

                // 기본 플레이어 추적
                var targetPosition = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, defaultSmoothTime);

                // 쉐이킹 활성화 시 흔들림 추가
                if (_isShaking)
                {
                    ApplyShakeEffect();
                }
            }
        }

        private void ApplyShakeEffect()
        {
            if (_shakeTimeRemaining <= 0f)
            {
                _isShaking = false; // 쉐이킹 종료
                _shakeOffset = Vector3.zero; // 오프셋 초기화
                return;
            }

            _shakeTimeRemaining -= Time.deltaTime; // 쉐이킹 지속 시간 감소
            _timeSinceLastShake += Time.deltaTime; // 단계 간 경과 시간 증가

            // 현재 쉐이킹 단계가 완료되었으면 다음 단계로 전환
            if (_timeSinceLastShake >= _shakeStepDuration && _currentShakeIndex < _shakePattern.Length)
            {
                _shakeOffset = new Vector3(_shakePattern[_currentShakeIndex], _shakePattern[_currentShakeIndex], 0f);
                _currentShakeIndex++;
                _timeSinceLastShake = 0f; // 시간 초기화
            }

            // 카메라 위치에 쉐이킹 오프셋 적용
            transform.position += _shakeOffset;
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

            var zoomTargetDistance = startDistance * 0.9f; // 목표 줌 크기에 도달할 지점 계산

            while (Vector3.Distance(transform.position, newTargetPosition) > 0.1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, newTargetPosition, ref _velocity, specificSmoothTime);

                var currentDistance = Vector3.Distance(transform.position, newTargetPosition);

                if (zoomIn && !zoomStarted && currentDistance <= startDistance * 0.6f)
                {
                    zoomStarted = true;
                }

                if (zoomIn && (zoomStarted || currentDistance <= zoomTargetDistance))
                {
                    zoomSpeed += zoomAcceleration * Time.deltaTime;
                    _camera.orthographicSize = Mathf.MoveTowards(_camera.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

                    if (Mathf.Abs(_camera.orthographicSize - targetSize) < 0.01f)
                    {
                        break;
                    }
                }

                yield return null;
            }

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