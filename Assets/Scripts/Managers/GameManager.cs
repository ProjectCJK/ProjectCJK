using System.Collections.Generic;
using Modules;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Enums;
using Units.Stages.Managers;
using UnityEngine;

namespace Managers
{
    public enum EES3Key
    {
        InitialSet,
        ES3Saver,
        CurrentStage,
        BuildingActiveStatuses,
        Gold,
        RedGem,
        Diamond
    }
    
    public class GameManager : SingletonMono<GameManager>
    {
        [SerializeField] private GameObject _mainCamera;
        private const float _saveInterval = 60f; // 30초마다 저장
        private float _saveTimer;

        public bool InGameTrigger { get; set; }
        public ES3Saver ES3Saver { get; set; }

        protected override void Awake()
        {
            if (_mainCamera != null)
            {
                GameObject cam = Instantiate(_mainCamera);
                DontDestroyOnLoad(cam);
            }

            // 프레임 고정
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            // ES3.settings 세팅
            ES3.CacheFile();
            ES3.settings = new ES3Settings(ES3.Location.Cache);

            // 데이터 로드
            if (ES3.KeyExists($"{EES3Key.ES3Saver}"))
            {
                ES3Saver = ES3.Load<ES3Saver>($"{EES3Key.ES3Saver}", ES3.settings);
            }
            else
            {
                ES3Saver = new ES3Saver
                {
                    InitializeES3Saver = true,
                    TutorialClear = false,
                    CurrentPlayerLevel = 1,
                    CurrentPlayerExp = 0,
                    Gold = 10000,
                    Diamond = 0,
                    RedGem = 0,
                    ActiveStatusSettingIndex = 0
                };

                ES3.Save($"{EES3Key.ES3Saver}", ES3Saver, ES3.settings);
            }

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
            if (pauseStatus)
            {
                SaveData();
            }
        }

        private void OnApplicationQuit()
        {
            SaveData();
        }

        private void SaveData()
        {
            ES3.Save($"{EES3Key.ES3Saver}", ES3Saver, ES3.settings);
            ES3.StoreCachedFile();
            Debug.Log("Data saved.");
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
                SaveData(); // 에디터에서 플레이 모드 종료 시 데이터 저장
            }
        }
#endif
    }
}