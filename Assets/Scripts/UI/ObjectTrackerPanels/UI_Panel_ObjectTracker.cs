using System.Collections;
using UnityEngine;

namespace UI.ObjectTrackerPanels
{
    public class UI_Panel_ObjectTracker : MonoBehaviour
    {
        [SerializeField] private GameObject compassPrefab; // 화살표 프리팹
    
        private Camera _camera;
        private const float yOffset = 1f;

        private bool trackingTrigger;
        private Transform _targetTransform;

        public void RegisterReference()
        {
            _camera = Camera.main;
        }

        private void SetActive(bool value)
        {
            gameObject.SetActive(value);
            compassPrefab.gameObject.SetActive(value);
        }
        
        public void StartTrackingTarget(Transform targetTransform)
        {
            if (!trackingTrigger) trackingTrigger = true;
            _targetTransform = targetTransform;

            SetActive(true);
        }

        public void StopTrackingTarget()
        {
            if (trackingTrigger) trackingTrigger = false;
            trackingTrigger = false;
            
            SetActive(false);
        }
        
        private void Update()
        {
            if (trackingTrigger && _targetTransform != null)
            {
                Vector3 targetPosition = _targetTransform.position;

                if (_camera != null)
                {
                    Vector3 viewportPoint = _camera.WorldToViewportPoint(targetPosition);

                    var isOffScreen = viewportPoint.x < 0 || viewportPoint.x > 1 ||
                                      viewportPoint.y < 0 || viewportPoint.y > 1;

                    if (isOffScreen)
                    {
                        // 화면 밖에 있을 경우
                        viewportPoint.x = Mathf.Clamp(viewportPoint.x, 0.1f, 0.9f);
                        viewportPoint.y = Mathf.Clamp(viewportPoint.y, 0.1f, 0.9f);

                        Vector3 indicatorPosition =
                            _camera.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, 10f));
                        compassPrefab.transform.position = indicatorPosition;

                        // 화살표가 타겟을 향하게
                        Vector3 direction = (targetPosition - indicatorPosition).normalized;
                        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        compassPrefab.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
                    }
                    else
                    {
                        // 화면 안에 있을 경우
                        Vector3 abovePosition = targetPosition + Vector3.up * yOffset;
                        compassPrefab.transform.position = Vector3.Lerp(compassPrefab.transform.position, abovePosition,
                            Time.deltaTime * 5f);

                        // 화살표가 부드럽게 아래를 보게 설정
                        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
                        compassPrefab.transform.rotation = Quaternion.Lerp(compassPrefab.transform.rotation,
                            targetRotation, Time.deltaTime * 5f);
                    }
                }
            }
        }
    }
}