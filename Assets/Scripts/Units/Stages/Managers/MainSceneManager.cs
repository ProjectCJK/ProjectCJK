using System.Collections.Generic;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Controllers;
using UnityEngine;

namespace Units.Stages.Managers
{
    public class MainSceneManager : SceneSingleton<MainSceneManager>
    {
        [Header("Stage Settings")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Grid _grid;
        [SerializeField] private GameObject _joystickPrefab;
        [SerializeField] private GameObject _StagePrefab;
        [SerializeField] private CameraController _cameraController;
        
        [Header("ItemSettings")]
        [SerializeField] private ItemDataSO _itemDataSo;
        
        private IStageController _stageController;
        private IItemController _itemController;
        private Joystick _joystick;

        private void Awake()
        {
            InstantiateJoystick();
            InstantiateStage();
            
            _itemController = new ItemController(new ItemFactory(_itemDataSo));
            
            _stageController.RegisterReference(_joystick, _itemController);
        }

        private void Start()
        {
            RegisterCameraToPlayer();
            
            _stageController.Initialize();
        }
        
        private void InstantiateJoystick()
        {
            GameObject obj = Instantiate(_joystickPrefab, _canvas.transform);
            _joystick = obj.GetComponent<Joystick>();
        }

        private void InstantiateStage()
        {
            GameObject obj = Instantiate(_StagePrefab, _grid.transform);
            _stageController = obj.GetComponent<StageController>();
        }
        
        private void RegisterCameraToPlayer()
        {
            _cameraController.RegisterReference(_stageController.GetPlayerTransform());
        }
    }
}