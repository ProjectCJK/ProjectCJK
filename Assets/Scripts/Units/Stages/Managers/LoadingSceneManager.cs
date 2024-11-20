using System.Collections;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Modules;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Units.Stages.Managers
{
    public enum ESceneName
    {
        MainScene
    }

    public class LoadingSceneManager : SingletonMono<LoadingSceneManager>
    {
        [SerializeField] private Canvas _rootCanvasPrefab;
        [SerializeField] private List<LoadingModule> _loadingPrefabs;

        private string targetSceneName;
        private bool isLoading;
        private Slider progressBar;
        private Canvas _rootCanvas;

        private const string loadingSceneName = "LoadingScene";

        private readonly List<Coroutine> activeCoroutines = new(); // 모든 실행 중인 코루틴을 저장

        public void LoadSceneWithLoadingScreen(ESceneName sceneName)
        {
            if (isLoading) return; // 이미 로드 중이면 무시

            isLoading = true;
            targetSceneName = $"{sceneName}";
            SceneManager.LoadScene(loadingSceneName);

            StartManagedCoroutine(InitializeLoadingScene());
        }

        private void StartManagedCoroutine(IEnumerator coroutine)
        {
            // 실행 중인 모든 코루틴 종료
            StopAllManagedCoroutines();

            // 새 코루틴 실행 및 관리
            Coroutine newCoroutine = StartCoroutine(coroutine); // StartCoroutine의 반환값을 저장
            activeCoroutines.Add(newCoroutine); // 리스트에 추가
        }

        private void StopAllManagedCoroutines()
        {
            foreach (Coroutine coroutine in activeCoroutines)
            {
                StopCoroutine(coroutine); // 실행 중인 모든 코루틴 정지
            }

            activeCoroutines.Clear(); // 리스트 초기화
            Debug.Log("All active coroutines have been stopped.");
        }

        private IEnumerator InitializeLoadingScene()
        {
            // 로딩 씬이 로드될 때까지 대기
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == loadingSceneName);

            Debug.Log("LoadingScene is active.");

            if (Camera.main != null)
            {
                Camera.main.orthographicSize = 5;
                _rootCanvas = Instantiate(_rootCanvasPrefab, Vector3.zero, Quaternion.identity);
                _rootCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            }

            // GameManager 상태에 따라 Prefab 생성
            if (!GameManager.Instance.InGameTrigger)
            {
                GameManager.Instance.InGameTrigger = true;
                Debug.Log("Creating loading UI (InGameTrigger: false).");
                // Loading Prefab 0 생성
                var loadingModule = Instantiate(_loadingPrefabs[0], _rootCanvas.transform);
                progressBar = loadingModule.Slider; // Slider 연결
            }
            else
            {
                Debug.Log("Creating loading UI (InGameTrigger: true).");

                // Loading Prefab 1 생성
                var loadingModule = Instantiate(_loadingPrefabs[1], _rootCanvas.transform);
                progressBar = loadingModule.Slider; // Slider 연결
            }

            // progressBar가 설정되지 않았다면 에러 출력 후 종료
            if (progressBar == null)
            {
                Debug.LogError("Progress bar could not be assigned from the loading UI.");
                isLoading = false; // 로딩 상태 초기화
                yield break;
            }

            Debug.Log("ProgressBar successfully assigned. Starting target scene loading.");

            // 비동기 로딩 실행
            StartManagedCoroutine(InitializeAndLoadTargetSceneAsync());
        }

        private IEnumerator InitializeAndLoadTargetSceneAsync()
        {
            Debug.Log("Starting InitializeAndLoadTargetSceneAsync...");

            // 비동기 로드 시작
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false; // 씬 전환 대기

                var currentProgress = 0f;

                // 로딩 진행률 업데이트
                while (!asyncLoad.isDone)
                {
                    var targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                    currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * 5f);
                    progressBar.value = currentProgress;

                    if (asyncLoad.progress >= 0.9f && Mathf.Abs(currentProgress - 1f) < 0.01f)
                    {
                        progressBar.value = 1f;
                        yield return new WaitForSeconds(0.5f); // 사용자 경험 개선을 위한 딜레이
                        asyncLoad.allowSceneActivation = true; // 씬 전환 허용
                    }

                    yield return null;
                }
            }

            // 로딩 씬 상태 확인
            Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);
            if (loadingScene.IsValid() && loadingScene.isLoaded)
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadingSceneName);
                while (unloadOperation is { isDone: false })
                {
                    yield return null;
                }
            }
            else
            {
                Debug.LogWarning($"Scene '{loadingSceneName}' is not valid or already unloaded.");
            }

            progressBar = null;
            isLoading = false; // 로딩 상태 초기화
            Debug.Log("Scene loading completed.");
        }
    }
}