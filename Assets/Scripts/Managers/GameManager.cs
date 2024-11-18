using Modules.DesignPatterns.Singletons;
using Units.Stages.Managers;
using UnityEngine;

namespace Managers
{
    public interface IGameManager
    {
    }
    
    public enum ES3Key
    {
        CurrentStage
    }

    public class GameManager : SingletonMono<GameManager>, IGameManager
    {
        public bool InGameTrigger;
        
        protected override void Awake()
        {
            // 프레임 고정
            Application.targetFrameRate = 60;
            // VSync 비활성화
            QualitySettings.vSyncCount = 0;
            
            // ES3.settings 세팅
            ES3.CacheFile();
            ES3.settings = new ES3Settings(ES3.Location.Cache);
            
            // ES3.Save("temp", "talskdaskdj", ES3.settings);
            // ES3.StoreCachedFile();
            
            InGameTrigger = false;
        }

        private void Start()
        {
            LoadingSceneManager.Instance.LoadSceneWithLoadingScreen(ESceneName.MainScene);
        }
    }
}