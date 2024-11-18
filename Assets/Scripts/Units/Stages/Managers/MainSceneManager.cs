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
        public List<GameObject> StagePrefab;
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
            VolatileDataManager.Instance.RegisterReference();
            
            InstantiatePrefabs();
            RegisterReference();
        }

        private void Start()
        {
            Initialize();
        }

        private void InstantiatePrefabs()
        {
            InstantiateUI();
            InstantiateStage();
        }

        private void InstantiateUI()
        {
            MainSceneUIManager.Instance.RegisterReference(_mainSceneDefaultSetting.Canvas.transform);
        }

        private void InstantiateStage()
        {
            GameObject stage;
            
            if (ES3.KeyExists($"{EES3Key.CurrentStage}"))
            {
                var targetStage = ES3.Load<int>($"{EES3Key.CurrentStage}") + 1;
                stage = Instantiate(_mainSceneDefaultSetting.StagePrefab[targetStage]);
            }
            else
            {
                stage = Instantiate(_mainSceneDefaultSetting.StagePrefab[0]);
            }
            
            _stageController = stage.GetComponent<StageController>();
        }

        private void RegisterReference()
        {
            _joystick = MainSceneUIManager.Instance.Joystick;
            _stageController.RegisterReference(_joystick);
            
            CurrencyManager.Instance.RegisterReference(MainSceneUIManager.Instance.UI_Panel_Currency);
            
            QuestManager.Instance.RegisterReference(MainSceneUIManager.Instance.UI_Panel_Quest);
            CostumeManager.Instance.RegisterReference();
            
            MainSceneUIManager.Instance.UI_Button_StageMap.onClick.RemoveAllListeners();
            MainSceneUIManager.Instance.UI_Button_StageMap.onClick.AddListener(() => MainSceneUIManager.Instance.UI_Panel_StageMap.gameObject.SetActive(true));
            MainSceneUIManager.Instance.UI_Panel_StageMap.RegisterReference();
            
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
                  
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CurrencyManager.Instance.AddGold(10000);
                CurrencyManager.Instance.AddDiamond(10000);
                CurrencyManager.Instance.AddRedGem(10000);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CurrencyManager.Instance._gold = 0;
                CurrencyManager.Instance._diamond = 0;
                CurrencyManager.Instance._redGem = 0;
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