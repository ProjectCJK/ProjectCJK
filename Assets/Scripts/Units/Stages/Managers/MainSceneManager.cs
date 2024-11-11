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
        [Header("Stage Settings")] public RootCanvas Canvas;
        public GameObject JoystickPrefab;
        public GameObject StagePrefab;
        public CameraController CameraController;
        public GameObject UICurrencyPrefab;
    }

    public class MainSceneManager : SceneSingleton<MainSceneManager>
    {
        [SerializeField] private MainSceneDefaultSetting _mainSceneDefaultSetting;
        private CurrencyView _currencyView;

        private Joystick _joystick;

        private IStageController _stageController;

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
            GameObject obj = Instantiate(_mainSceneDefaultSetting.UICurrencyPrefab,
                _mainSceneDefaultSetting.Canvas.transform);
            _currencyView = obj.GetComponent<CurrencyView>();
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
            CurrencyManager.Instance.RegisterReference(_currencyView);

            _stageController.RegisterReference(_joystick);

            _mainSceneDefaultSetting.CameraController.RegisterReference(_stageController.PlayerTransform);
        }

        private void Initialize()
        {
            CurrencyManager.Instance.Initialize();
            _stageController.Initialize();
        }
    }
}