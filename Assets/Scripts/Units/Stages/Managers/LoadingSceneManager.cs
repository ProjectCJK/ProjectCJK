using System.Collections;
using System.Collections.Generic;
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

        public void LoadSceneWithLoadingScreen(ESceneName sceneName)
        {
            if (isLoading) return;

            isLoading = true;
            targetSceneName = $"{sceneName}";
            SceneManager.LoadScene(loadingSceneName);

            StartCoroutine(InitializeLoadingScene());
        }

        private IEnumerator InitializeLoadingScene()
        {
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == loadingSceneName);

            SetupLoadingUI();
            yield return StartCoroutine(LoadTargetSceneAsync());
        }

        private void SetupLoadingUI()
        {
            if (Camera.main != null)
            {
                Camera.main.orthographicSize = 5;
                _rootCanvas = Instantiate(_rootCanvasPrefab, Vector3.zero, Quaternion.identity);
                
                var canvas = _rootCanvas.GetComponent<Canvas>();
                canvas.worldCamera = Camera.main;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
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
                asyncLoad.allowSceneActivation = false;

                float displayedProgress = 0f;

                // 리소스 로드 (0 ~ 0.5)
                while (asyncLoad.progress < 0.9f)
                {
                    float targetProgress = asyncLoad.progress / 0.9f * 0.5f;
                    displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, Time.deltaTime * 2f);
                    progressBar.value = displayedProgress;
                    yield return null;
                }

                progressBar.value = 0.5f;

                // 씬 활성화 준비 (0.5 ~ 0.9)
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

                // 로딩 바 완성 (0.9 ~ 1.0)
                yield return StartCoroutine(CompleteLoadingBar());

                // 로딩 씬 제거
                yield return StartCoroutine(UnloadLoadingScene());
            }
        }

        private IEnumerator WaitForSceneActivation()
        {
            while (!SceneManager.GetSceneByName(targetSceneName).isLoaded)
            {
                yield return null;
            }

            // 활성화 완료 후 MainScene 설정
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetSceneName));
        }

        private IEnumerator CompleteLoadingBar()
        {
            var displayedProgress = 0.9f;

            while (progressBar.value < 1f)
            {
                displayedProgress = Mathf.Lerp(displayedProgress, 1f, Time.deltaTime * 3f);
                progressBar.value = displayedProgress;

                if (Mathf.Abs(progressBar.value - 1f) < 0.01f)
                {
                    progressBar.value = 1f;
                    break;
                }

                yield return null;
            }

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
                progressBar.value = 1f;
                progressBar.gameObject.SetActive(false);
            }

            isLoading = false;
        }
    }
}
