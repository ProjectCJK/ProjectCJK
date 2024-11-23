using Modules.DesignPatterns.Singletons;
using UnityEngine;

namespace Managers
{
    public class ObjectTrackerManager : SceneSingleton<ObjectTrackerManager>
    {
        public bool IsTracking { get; private set; }
        
        private GameObject _objectTracker;
        private Camera mainCamera;
        private Transform targetTransform;

        private const float yOffset = 1f;

        public void RegisterReference()
        {
            mainCamera = Camera.main;
            _objectTracker = Instantiate(DataManager.Instance.ObjectTrackerPrefab, UIManager.Instance.BranchCanvasGuide.transform);
        }

        public void Initialize()
        {
            _objectTracker.gameObject.SetActive(false);
        }

        public void StartTargetTracking(Transform target)
        {
            IsTracking = true;
            targetTransform = target;
            _objectTracker.gameObject.SetActive(true);
        }

        public void StopTargetTracking()
        {
            IsTracking = false;
            targetTransform = null;
            _objectTracker.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (targetTransform != null)
            {
                Vector3 targetPosition = targetTransform.position;
                Vector3 viewportPoint = mainCamera.WorldToViewportPoint(targetPosition);

                var isOffScreen = viewportPoint.x < 0 || viewportPoint.x > 1 ||
                                  viewportPoint.y < 0 || viewportPoint.y > 1;

                if (isOffScreen)
                {
                    // 화면 밖에 있을 경우
                    viewportPoint.x = Mathf.Clamp(viewportPoint.x, 0.05f, 0.95f);
                    viewportPoint.y = Mathf.Clamp(viewportPoint.y, 0.05f, 0.95f);

                    Vector3 indicatorPosition = mainCamera.ViewportToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, 10f));
                    _objectTracker.transform.position = indicatorPosition;

                    // 손목이 카메라 중심을 가리키고 손가락이 타겟을 가리킴
                    Vector3 cameraCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
                    Vector3 directionToTarget = (targetPosition - cameraCenter).normalized;
                    var angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
                    _objectTracker.transform.rotation = Quaternion.Euler(0, 0, angleToTarget - 90f);
                }
                else
                {
                    // 화면 안에 있을 경우
                    Vector3 belowPosition = targetPosition + Vector3.up * yOffset;
                    _objectTracker.transform.position = Vector3.Lerp(_objectTracker.transform.position, belowPosition, Time.deltaTime * 5f);

                    // 손가락이 아래를 가리키도록 설정 (180도 회전)
                    Quaternion handRotation = Quaternion.Euler(0, 0, 180); // 아래를 바라보는 기본 회전값
                    _objectTracker.transform.rotation = Quaternion.Lerp(_objectTracker.transform.rotation, handRotation, Time.deltaTime * 5f);
                }

            }
        }
    }
}