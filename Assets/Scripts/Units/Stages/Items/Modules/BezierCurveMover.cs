using System;
using UnityEngine;

namespace Units.Stages.Items.Modules
{
    public interface IBezierCurveMover
    {
    
    }

    public class BezierCurveMover : IBezierCurveMover
    {
        private event Action OnArrived;
        
        private Vector3 _pointA;
        private Vector3 _controlPoint;
        private Transform _pointB;

        private readonly Transform _itemTransform;
        private readonly float _baseSpeed;
        private float _moveDuration;
        private float _timeElapsed;
        private bool _isRunning;

        public BezierCurveMover(MonoBehaviour item, float baseSpeed)
        {
            _itemTransform = item.transform;
            _baseSpeed = baseSpeed;

            Reset();
        }

        public void Initialize(Vector3 pointA, Transform pointB, Action action)
        {
            _isRunning = true;
            _pointA = pointA;
            _itemTransform.transform.position = pointA;
            _pointB = pointB;
            OnArrived = action;

            // A와 B의 중간 좌표
            Vector2 middlePoint = (_pointA + _pointB.position) / 2;

            // A와 B의 방향 벡터 계산 (A에서 B로 향하는 방향)
            Vector2 direction = Vector2.up;

            // 중간 좌표에서 방향으로 5단위 이동한 제어점 위치
            _controlPoint = middlePoint + direction * 5f; // A와 B 사이 방향으로 5단위 이동

            var distance = Vector2.Distance(_pointA, _pointB.position);

            // 거리에 따라 이동 시간 설정 (기본 속도를 거리에 곱함)
            _moveDuration = distance / _baseSpeed;
        }

        public void Transfer()
        {
            if (!_isRunning) return;

            _timeElapsed += Time.deltaTime;
            var t = _timeElapsed / _moveDuration;

            Vector2 newPosition = CalculateQuadraticBezierPoint(t, _pointA, _controlPoint, _pointB.position);
            _itemTransform.transform.position = newPosition;

            if (t >= 1f && _isRunning) // t가 1에 도달한 후 도착 처리
            {
                OnArrived?.Invoke();
                Reset();
            }
        }

        private static Vector2 CalculateQuadraticBezierPoint(float t, Vector2 a, Vector2 p, Vector2 b)
        {
            return Mathf.Pow(1 - t, 2) * a + 2 * (1 - t) * t * p + Mathf.Pow(t, 2) * b;
        }

        private void Reset()
        {
            OnArrived = null;
            _isRunning = false;
            _timeElapsed = 0;
            _pointA = Vector3.zero;
            _pointB = null;
        }

    }
}