using System;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Managers;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Controllers;
using Units.Stages.UI;
using UnityEngine;

namespace Units.Stages.Managers
{
    [Serializable]
    public struct MainSceneDefaultSetting
    {
        [Header("Stage Settings")]
        public Canvas Canvas;
        public GameObject JoystickPrefab;
        public GameObject StagePrefab;
        public CameraController CameraController;
        public GameObject UICurrencyPrefab;
    }
    public class MainSceneManager : SceneSingleton<MainSceneManager>
    {
        [SerializeField] private MainSceneDefaultSetting _mainSceneDefaultSetting;
        
        private IStageController _stageController;
        
        private Joystick _joystick;
        private CurrencyView _currencyView;

        private void Awake()
        {
            InstantiatePrefabs();
            RegisterReference();
        }

        private void InstantiatePrefabs()
        {
            InstantiateUI();
            InstantiateJoystick();
            InstantiateStage();
        }
        
        private void Start()
        {
            Initialize();
         
        }
        
        private void InstantiateUI()
        {
            GameObject obj = Instantiate(_mainSceneDefaultSetting.UICurrencyPrefab, _mainSceneDefaultSetting.Canvas.transform);
            _currencyView = obj.GetComponent<CurrencyView>(); 
        }
        
        private void InstantiateJoystick()
        {
            GameObject obj = Instantiate(_mainSceneDefaultSetting.JoystickPrefab, _mainSceneDefaultSetting.Canvas.transform);
            _joystick = obj.GetComponent<Joystick>();
        }

        private void InstantiateStage()
        {
            GameObject obj = Instantiate(_mainSceneDefaultSetting.StagePrefab);
            _stageController = obj.GetComponent<StageController>();
        }
        
        private void RegisterReference()
        {
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