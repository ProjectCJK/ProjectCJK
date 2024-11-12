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
        public GameObject UICurrencyPrefab;
        public GameObject UIBuildingUpgradePanelPrefab;
        public GameObject JoystickPrefab;
        public GameObject StagePrefab;
        public CameraController CameraController;
    }

    public class MainSceneManager : SceneSingleton<MainSceneManager>
    {
        [SerializeField] private MainSceneDefaultSetting _mainSceneDefaultSetting;
        
        private IStageController _stageController;
        
        private Joystick _joystick;
        
        private UI_Currency _uiCurrencyPrefab;
        private UI_BuildingEnhancement _upgradePanelPrefab;

        private void Awake()
        {
            // 프레임 고정
            Application.targetFrameRate = 60;
            // VSync 비활성화
            QualitySettings.vSyncCount = 0;
            
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
            InstantiateJoystick();
            InstantiateStage();
        }

        private void InstantiateUI()
        {
            GameObject currencyPrefab = Instantiate(_mainSceneDefaultSetting.UICurrencyPrefab, _mainSceneDefaultSetting.Canvas.transform);
            _uiCurrencyPrefab = currencyPrefab.GetComponent<UI_Currency>();
            
            GameObject upgradePanelPrefab = Instantiate(_mainSceneDefaultSetting.UIBuildingUpgradePanelPrefab, _mainSceneDefaultSetting.Canvas.transform);
            _upgradePanelPrefab = upgradePanelPrefab.GetComponent<UI_BuildingEnhancement>();
            UIManager.Instance.UIBuildingEnhancement = _upgradePanelPrefab;
        }

        private void InstantiateJoystick()
        {
            GameObject obj = Instantiate(_mainSceneDefaultSetting.JoystickPrefab,
                _mainSceneDefaultSetting.Canvas.Canvas_Joystick.transform);
            _joystick = obj.GetComponent<Joystick>();
        }

        private void InstantiateStage()
        {
            GameObject obj = Instantiate(_mainSceneDefaultSetting.StagePrefab);
            _stageController = obj.GetComponent<StageController>();
        }

        private void RegisterReference()
        {
            VolatileDataManager.Instance.RegisterReference();
            CurrencyManager.Instance.RegisterReference(_uiCurrencyPrefab);
            QuestManager.Instance.RegisterReference();

            _stageController.RegisterReference(_joystick);

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
#endif
        }
    }
}