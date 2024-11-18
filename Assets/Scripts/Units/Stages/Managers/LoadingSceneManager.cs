using System.Collections;
using Modules.DesignPatterns.Singletons;
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
        private Slider progressBar;
        private string targetSceneName;
        private bool isLoading;

        private const string loadingSceneName = "LoadingScene";

        public void LoadSceneWithLoadingScreen(ESceneName sceneName)
        {
            if (isLoading) return; // 이미 로드 중이면 무시

            isLoading = true;
            targetSceneName = $"{sceneName}";
            SceneManager.LoadScene(loadingSceneName);

            StartCoroutine(InitializeAndLoadTargetSceneAsync());
        }

        public void LoadSceneWithLoadingScreen(string sceneName)
        {
            if (isLoading) return; // 이미 로드 중이면 무시

            isLoading = true;
            targetSceneName = sceneName;
            SceneManager.LoadScene(loadingSceneName);

            StartCoroutine(InitializeAndLoadTargetSceneAsync());
        }

        private IEnumerator InitializeAndLoadTargetSceneAsync()
        {
            // ProgressBar 찾기 시도
            yield return StartCoroutine(FindProgressBar());

            if (progressBar == null)
            {
                Debug.LogError("Progress bar not found in LoadingScene.");
                isLoading = false; // 로딩 상태 초기화
                yield break;
            }

            // 비동기 로드 시작
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false; // 씬 전환 대기

                var currentProgress = 0f;

                // 로딩 진행률 업데이트
                while (!asyncLoad.isDone)
                {
                    // 목표 진행률 계산
                    var targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                    // 현재 진행률을 목표 진행률에 보간
                    currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * 5f);
                    progressBar.value = currentProgress;

                    // 로딩 완료 시 처리
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
                // 로딩 씬 언로드
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(loadingSceneName);
                while (unloadOperation is { isDone: false })
                {
                    yield return null; // 언로드 완료 대기
                }
            }
            else
            {
                Debug.LogWarning($"Scene '{loadingSceneName}' is not valid or already unloaded.");
            }

            isLoading = false; // 로딩 상태 초기화
        }

        private IEnumerator FindProgressBar()
        {
            progressBar = null;
            while (progressBar == null)
            {
                progressBar = FindObjectOfType<Slider>();
                if (progressBar == null)
                {
                    Debug.Log("Waiting for progress bar to be initialized...");
                    yield return null; // 다음 프레임까지 대기
                }
            }
        }
    }
}