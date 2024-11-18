using Modules.DesignPatterns.Singletons;
using Units.Stages.Managers;
using UnityEngine;

namespace Managers
{
    public enum EES3Key
    {
        InitialSet,
        CurrentStage,
        BuildingActiveStatuses,
        Gold,
        RedGem,
        Diamond
    }

    public class GameManager : SingletonMono<GameManager>
    {
        [SerializeField] private GameObject _mainCamera;
        private float _saveInterval = 30f; // 30초마다 저장
        private float _saveTimer;

        public bool InGameTrigger { get; set; }
        
        protected override void Awake()
        {
            if (_mainCamera != null)
            {
                GameObject cam = Instantiate(_mainCamera);
                DontDestroyOnLoad(cam);
            }
            
            // 프레임 고정
            Application.targetFrameRate = 60;
            // VSync 비활성화
            QualitySettings.vSyncCount = 0;
            
            // ES3.settings 세팅
            ES3.CacheFile();
            ES3.settings = new ES3Settings(ES3.Location.Cache);
            
            ES3.Save($"{EES3Key.InitialSet}", true, ES3.settings);
            
            InGameTrigger = false;
        }

        private void Start()
        {
            LoadingSceneManager.Instance.LoadSceneWithLoadingScreen(ESceneName.MainScene);
        }

        private void Update()
        {
            // 30초마다 데이터를 저장
            _saveTimer += Time.deltaTime;
            if (_saveTimer >= _saveInterval)
            {
                SaveData();
                _saveTimer = 0f;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log($"OnApplicationPause called. Pause status: {pauseStatus}");
            if (pauseStatus) // 앱이 백그라운드로 이동할 때만 저장
            {
                SaveData();
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit called.");
            SaveData();
        }

        private void SaveData()
        {
            Debug.Log("Data saved.");
            ES3.StoreCachedFile(); // ES3 데이터 저장
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                Debug.Log("Exiting Play Mode - Saving data in editor.");
                SaveData(); // 에디터에서 플레이 모드 종료 시 데이터 저장
            }
        }
#endif
    }
}