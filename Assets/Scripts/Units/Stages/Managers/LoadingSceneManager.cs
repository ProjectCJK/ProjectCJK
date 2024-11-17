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
            yield return new WaitForSeconds(0.1f);
            progressBar = FindObjectOfType<Slider>();

            if (progressBar == null)
            {
                Debug.LogError("Progress bar not found in LoadingScene.");
                yield break;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            
            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false;

                // 로딩 진행률을 슬라이더에 업데이트
                while (!asyncLoad.isDone)
                {
                    progressBar.value = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                    if (asyncLoad.progress >= 0.9f)
                    {
                        // 로딩이 완료되면 약간의 딜레이 후 씬 전환
                        progressBar.value = 1f;
                        yield return new WaitForSeconds(0.5f);
                        asyncLoad.allowSceneActivation = true;
                    }

                    yield return null;
                }
            }

            SceneManager.UnloadSceneAsync(loadingSceneName);
        }
    }
}