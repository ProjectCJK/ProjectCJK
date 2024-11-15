using System;
using Externals.Joystick.Scripts.Base;
using Managers;
using Modules.DesignPatterns.Singletons;
using UI;
using Units.Stages.Controllers;
using Units.Stages.UI;
using UnityEngine;

namespace Units.Stages.Managers
{
    [Serializable]
    public struct MainSceneDefaultSetting
    {
        [Header("### Stage Settings ###")]
        [Header("=== UI Settings ===")]
        public RootCanvas Canvas;
        public GameObject JoystickPrefab;
        public GameObject StagePrefab;
        public CameraController CameraController;
    }

    public class MainSceneManager : SceneSingleton<MainSceneManager>
    {
        [SerializeField] private MainSceneDefaultSetting _mainSceneDefaultSetting;
        
        private IStageController _stageController;
        
        private Joystick _joystick;
        
        private UI_Panel_Currency _uiPanelCurrencyPrefab;
        private UI_Panel_BuildingEnhancement _upgradePanelPrefab;
        private UI_Panel_Quest _questPanelPrefab;

        private void Awake()
        {
            // 프레임 고정
            Application.targetFrameRate = 60;
            // VSync 비활성화
            QualitySettings.vSyncCount = 0;
            
            // ES3.settings 세팅
            // ES3.CacheFile();
            // ES3.settings = new ES3Settings(ES3.Location.Cache);
            //
            // ES3.Save<string>("temp", "talskdaskdj", ES3.settings);
            //
            // ES3.StoreCachedFile();
            
            InstantiatePrefabs();
            RegisterReference();
        }

        private void Start()
        {
            Initialize();
        }

        private void InstantiatePrefabs()
        {
            InstantiateJoystick();
            InstantiateStage();
        }

        private void InstantiateJoystick()
        {
            GameObject obj = Instantiate(_mainSceneDefaultSetting.JoystickPrefab, _mainSceneDefaultSetting.Canvas.Canvas_Joystick.transform);
            _joystick = obj.GetComponent<Joystick>();
        }

        private void InstantiateStage()
        {
            GameObject obj = Instantiate(_mainSceneDefaultSetting.StagePrefab);
            _stageController = obj.GetComponent<StageController>();
        }

        private void RegisterReference()
        {
            _stageController.RegisterReference(_joystick);
            
            VolatileDataManager.Instance.RegisterReference();
            CurrencyManager.Instance.RegisterReference(UIManager.Instance.UI_Panel_Currency);
            
            QuestManager.Instance.RegisterReference(UIManager.Instance.UI_Panel_Quest);
            CostumeManager.Instance.RegisterReference();
            // CostumeManager.Instance.RegisterReference(UIManager.Instance.UI_Panel_Costume);
            
            _mainSceneDefaultSetting.CameraController.RegisterReference(_stageController.PlayerTransform);
        }

        private void Initialize()
        {
            CurrencyManager.Instance.Initialize();
            _stageController.Initialize();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CurrencyManager.Instance.AddDiamond(100);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CurrencyManager.Instance.AddRedGem(100);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CurrencyManager.Instance.AddGold(100);
            }

            // if (Input.GetKeyDown(KeyCode.U))
            // {
            //     QuestManager.Instance.UpdateCurrentQuestProgress(EQuestType1.LevelUpOption1, $"{EQuestType2.Kitchen_A}");
            // }
            //
            // if (Input.GetKeyDown(KeyCode.I))
            // {
            //     QuestManager.Instance.UpdateCurrentQuestProgress(EQuestType1.Product, $"{EQuestType2.ProductA_A}");
            // }
            //
            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     QuestManager.Instance.UpdateCurrentQuestProgress(EQuestType1.Build, $"{EQuestType2.Kitchen_B}");
            // }
            //
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     QuestManager.Instance.UpdateCurrentQuestProgress(EQuestType1.Build, $"{EQuestType2.Stand_B}");
            // }
#endif
        }
    }
}