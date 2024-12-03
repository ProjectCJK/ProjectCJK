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
            StopAllManagedCoroutines();

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
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == loadingSceneName);

            SetupLoadingUI();

            // 비동기 로딩 실행
            StartManagedCoroutine(LoadTargetSceneAsync());
        }

        private void SetupLoadingUI()
        {
            if (Camera.main != null)
            {
                Camera.main.orthographicSize = 5;
                _rootCanvas = Instantiate(_rootCanvasPrefab, Vector3.zero, Quaternion.identity);
                _rootCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            }

            if (_loadingPrefabs.Count > 0)
            {
                LoadingModule loadingModule = Instantiate(_loadingPrefabs[0], _rootCanvas.transform);
                progressBar = loadingModule.Slider;
            }

            if (progressBar != null) progressBar.value = 0f;
        }

        private IEnumerator LoadTargetSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false; // 씬 활성화를 지연

                var displayedProgress = 0f;

                // 리소스 로드 진행률 (0 ~ 0.5)
                while (asyncLoad.progress < 0.9f)
                {
                    float targetProgress = asyncLoad.progress / 0.9f * 0.5f; // 실제 진행률을 0.5로 변환
                    displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, Time.deltaTime * 2f);
                    progressBar.value = displayedProgress;
                    yield return null;
                }

                // 리소스 로드 완료 후 진행률 고정
                progressBar.value = 0.5f;

                // 씬 활성화 진행률 (0.5 ~ 0.9)
                while (!asyncLoad.isDone)
                {
                    displayedProgress = Mathf.Lerp(displayedProgress, 0.9f, Time.deltaTime * 0.5f);
                    progressBar.value = displayedProgress;

                    if (displayedProgress >= 0.89f)
                    {
                        asyncLoad.allowSceneActivation = true;
                        break;
                    }

                    yield return null;
                }

                // 씬 활성화 완료 대기
                yield return StartCoroutine(WaitForSceneActivation());

                // 로딩바가 0.9에서 1.0까지 부드럽게 이동
                yield return StartCoroutine(CompleteLoadingBar());

                // 로딩씬 제거
                yield return StartCoroutine(UnloadLoadingScene());
            }
        }

        private IEnumerator WaitForSceneActivation()
        {
            while (SceneManager.GetActiveScene().name != targetSceneName)
            {
                yield return null;
            }
        }

        private IEnumerator CompleteLoadingBar()
        {
            float displayedProgress = 0.9f;

            while (progressBar.value < 1f)
            {
                displayedProgress = Mathf.Lerp(displayedProgress, 1f, Time.deltaTime * 3f); // 빠르게 1까지 이동
                progressBar.value = displayedProgress;

                if (Mathf.Abs(progressBar.value - 1f) < 0.01f) // 1에 도달하면 탈출
                {
                    progressBar.value = 1f;
                    break;
                }

                yield return null;
            }

            // 1초 대기
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator UnloadLoadingScene()
        {
            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            if (targetScene.IsValid())
            {
                SceneManager.SetActiveScene(targetScene);
            }

            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadingSceneName);

            while (unloadOperation is { isDone: false })
            {
                yield return null;
            }

            if (progressBar != null)
            {
                progressBar.value = 1f; // 로딩 완료
                progressBar.gameObject.SetActive(false);
            }

            isLoading = false;
        }
    }
}