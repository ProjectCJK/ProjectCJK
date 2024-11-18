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
        
        private IStageController _stageController;
        
        private Joystick _joystick;
        
        private UI_Panel_Currency _uiPanelCurrencyPrefab;
        private UI_Panel_BuildingEnhancement _upgradePanelPrefab;
        private UI_Panel_Quest _questPanelPrefab;
        
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
            GameObject stage;
            
            if (ES3.KeyExists($"{EES3Key.CurrentStage}"))
            {
                var targetStage = ES3.Load<int>($"{EES3Key.CurrentStage}");
                stage = Instantiate(_mainSceneDefaultSetting.StagePrefab[targetStage + 1]);
            }
            else
            {
                stage = Instantiate(_mainSceneDefaultSetting.StagePrefab[0]);
            }
            
            _stageController = stage.GetComponent<StageController>();
        }

        private void RegisterReference()
        {
            _joystick = UIManager.Instance.Joystick;
            _stageController.RegisterReference(_joystick);
            
            CurrencyManager.Instance.RegisterReference(UIManager.Instance.UI_Panel_Currency);
            
            QuestManager.Instance.RegisterReference(UIManager.Instance.UI_Panel_Quest);
            CostumeManager.Instance.RegisterReference();
            
            UIManager.Instance.UI_Button_StageMap.onClick.RemoveAllListeners();
            UIManager.Instance.UI_Button_StageMap.onClick.AddListener(() => UIManager.Instance.UI_Panel_StageMap.gameObject.SetActive(true));
            UIManager.Instance.UI_Panel_StageMap.RegisterReference();

            if (Camera.main != null)
            {
                Camera.main.orthographicSize = 10;
                Camera.main.GetComponent<CameraController>().RegisterReference(_stageController.PlayerTransform);   
            }
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