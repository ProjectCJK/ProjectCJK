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

            if (!GameManager.Instance.ES3Saver.first_loading)
            {
                GameManager.Instance.ES3Saver.first_loading = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("first_loading");
            }
            
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
        }

        private IEnumerator InitializeLoadingScene()
        {
            // 로딩 씬이 로드될 때까지 대기
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == loadingSceneName);
            
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
                
                // Loading Prefab 0 생성
                LoadingModule loadingModule = Instantiate(_loadingPrefabs[0], _rootCanvas.transform);
                progressBar = loadingModule.Slider; // Slider 연결
            }
            else
            {
                // Loading Prefab 1 생성
                LoadingModule loadingModule = Instantiate(_loadingPrefabs[1], _rootCanvas.transform);
                progressBar = loadingModule.Slider; // Slider 연결
            }

            // progressBar가 설정되지 않았다면 에러 출력 후 종료
            if (progressBar == null)
            {
                isLoading = false; // 로딩 상태 초기화
                yield break;
            }
            // 비동기 로딩 실행
            StartManagedCoroutine(InitializeAndLoadTargetSceneAsync());
        }

        private IEnumerator InitializeAndLoadTargetSceneAsync()
        {
            // 비동기 로드 시작
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false; // 씬 전환 대기

                // 로딩 진행률 초기화
                var currentProgress = 0f;

                while (!asyncLoad.isDone)
                {
                    // 비동기 로딩 진행 상태 계산
                    var targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                    // 진행률을 부드럽게 증가
                    currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, Time.deltaTime * 0.5f);
                    progressBar.value = currentProgress;

                    // 로딩이 거의 완료되고, 진행률이 1에 도달한 경우 씬 전환
                    if (asyncLoad.progress >= 0.9f && currentProgress >= 0.99f)
                    {
                        progressBar.value = 1f;
                        yield return new WaitForSeconds(0.2f); // 부드러운 전환을 위한 약간의 대기
                        asyncLoad.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }

            // 로딩 씬 상태 확인 및 언로드
            Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);
            if (loadingScene.IsValid() && loadingScene.isLoaded)
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadingSceneName);
                while (unloadOperation is { isDone: false })
                {
                    yield return null;
                }
            }

            progressBar = null;
            isLoading = false; // 로딩 상태 초기화
        }
    }
}