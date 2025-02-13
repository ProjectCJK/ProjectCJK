using System;
using System.Collections.Generic;
using Externals.Joystick.Scripts.Base;
using Managers;
using Modules.DesignPatterns.Singletons;
using UI;
using UI.BuildingEnhancementPanel;
using UI.CurrencyPanel;
using UI.QuestPanels;
using Units.Stages.Controllers;
using Units.Stages.Modules;
using Units.Stages.UI;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Managers
{
    [Serializable]
    public struct MainSceneDefaultSetting
    {
        [Header("### Stage Settings ###")]
        [Header("=== UI Settings ===")]
        public List<GameObject> StagePrefab;
    }

    public class MainSceneManager : SceneSingleton<MainSceneManager>
    {
        [SerializeField] private MainSceneDefaultSetting _mainSceneDefaultSetting;
        
        public StageController StageController;
        public CameraController CameraController;
        private Joystick _joystick;
        
        public bool Initialized { get; set; }

        private void Awake()
        {
            Initialized = false;
            
            VolatileDataManager.Instance.RegisterReference();
            
            InstantiatePrefabs();
            RegisterReference();
        }

        private void Start()
        {
            Initialize();
            Initialized = true;
        }

        private void InstantiatePrefabs()
        {
            InstantiateUI();
            InstantiateStage();
        }

        private void InstantiateUI()
        {
            UIManager.Instance.RegisterReference();
        }

        private void InstantiateStage()
        {
            GameObject stage = Instantiate(GameManager.Instance.ES3Saver.CurrentStageLevel == 0
                ? _mainSceneDefaultSetting.StagePrefab[0]
                : _mainSceneDefaultSetting.StagePrefab[GameManager.Instance.ES3Saver.CurrentStageLevel - 1]);

            StageController = stage.GetComponent<StageController>();
        }

        private void RegisterReference()
        {
            _joystick = UIManager.Instance.Joystick;
            StageController.RegisterReference(_joystick);

            ObjectTrackerManager.Instance.RegisterReference();
            LevelManager.Instance.RegisterReference();
            CurrencyManager.Instance.RegisterReference();
            QuestManager.Instance.RegisterReference(StageController);
            CostumeManager.Instance.RegisterReference();
            StageManager.Instance.RegisterReference();

            if (Camera.main != null)
            {
                Camera.main.orthographicSize = 15;
                CameraController = Camera.main.GetComponent<CameraController>();
                CameraController.RegisterReference(StageController.PlayerTransform);
            }
            
            TutorialManager.Instance.RegisterReference(CameraController);
        }

        private void Initialize()
        {
            LevelManager.Instance.Initialize();
            CurrencyManager.Instance.Initialize();
            StageController.Initialize();

            if (!GameManager.Instance.ES3Saver.first_ingame)
            {
                GameManager.Instance.ES3Saver.first_ingame = true;
                Firebase.Analytics.FirebaseAnalytics.LogEvent("first_ingame");
            }
            
            if (GameManager.Instance.ES3Saver.InitialTutorialClear == false)
            {
                StartCoroutine(TutorialManager.Instance.TutorialInitialDialog());
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CurrencyManager.Instance.AddCurrency(ECurrencyType.Diamond, 100);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CurrencyManager.Instance.AddCurrency(ECurrencyType.RedGem, 100);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CurrencyManager.Instance.AddCurrency(ECurrencyType.Gold, 100);
            }
                  
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CurrencyManager.Instance.AddCurrency(ECurrencyType.Gold, 10000);
                CurrencyManager.Instance.AddCurrency(ECurrencyType.Diamond, 10000);
                CurrencyManager.Instance.AddCurrency(ECurrencyType.RedGem, 10000);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CurrencyManager.Instance.RemoveCurrency(ECurrencyType.Gold, 10000);
                CurrencyManager.Instance.RemoveCurrency(ECurrencyType.Diamond, 10000);
                CurrencyManager.Instance.RemoveCurrency(ECurrencyType.RedGem, 10000);
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