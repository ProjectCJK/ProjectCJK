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
        [SerializeField] private Slider progressBar;
        
        private string targetSceneName;
        
        private const string loadingSceneName = "LoadingScene";
        
        public void LoadSceneWithLoadingScreen(ESceneName sceneName)
        {
            targetSceneName = $"{sceneName}";
            SceneManager.LoadScene(loadingSceneName);

            StartCoroutine(InitializeAndLoadTargetSceneAsync());
        }

        private IEnumerator InitializeAndLoadTargetSceneAsync()
        {
            // ProgressBar가 로드되기를 잠시 대기
            yield return new WaitForSeconds(0.1f);
            progressBar = FindObjectOfType<Slider>();

            if (progressBar == null)
            {
                Debug.LogError("Progress bar not found in LoadingScene.");
                yield break;
            }

            // 비동기 로드 시작
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false; // 씬 전환 대기

                // 로딩 진행률 업데이트
                while (!asyncLoad.isDone)
                {
                    progressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                    if (asyncLoad.progress >= 0.9f) // 로딩 완료
                    {
                        progressBar.value = 1f;
                        yield return new WaitForSeconds(0.5f); // 사용자 경험 개선을 위한 딜레이
                        asyncLoad.allowSceneActivation = true; // 씬 전환 허용
                    }

                    yield return null;
                }
            }

            // 로딩 씬 언로드
            yield return SceneManager.UnloadSceneAsync(loadingSceneName);
        }
    }
}