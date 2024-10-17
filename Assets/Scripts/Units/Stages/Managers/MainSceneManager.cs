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
    public class MainSceneManager : SceneSingleton<MainSceneManager>, IRegisterReference
    {
        [Header("Default Settings")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Grid _grid;
        [SerializeField] private CameraController _cameraController;
        
        [Header("CreatureSettings")]
        [SerializeField] private CreatureController _creatureController;

        [Header("BuildingSettings")]
        [SerializeField] private BuildingController _buildingController;
        [SerializeField] private List<BuildingDataSO> _buildingStatSo;
        
        [Header("ItemSettings")]
        [SerializeField] private ItemDataSO _itemDataSo;
        
        [Header("LevelSettings")]
        [SerializeField] private LevelDesignDataSO _levelDesignDataSo;
        
        private IItemController _itemController;
        private Joystick _joystick;

        private void Awake()
        {
            InstantiateLevels();
            RegisterReference();
            InstantiateUnits();
            RegisterCameraToPlayer();
        }

        private void Start()
        {
            _creatureController.Initialize();
            _buildingController.Initialize();
        }

        public void RegisterReference()
        {
            _itemController = new ItemController(new ItemFactory(_itemDataSo));
        }

        // 씬 초기 스폰 오브젝트들 처리
        private void InstantiateLevels()
        {
            _joystick = SpawnJoystick();
            SpawnLevelObjects();
        }
        
        private void InstantiateUnits()
        {
            _creatureController.InstantiatePlayer();
            // _buildingController.InstantiateBuilding();
            _itemController.InstantiateItem();
        }

        // 카메라가 플레이어를 추적하도록 설정
        private void RegisterCameraToPlayer()
        {
            _cameraController.RegisterReference(_creatureController.GetPlayerTransform());
        }

        // 조이스틱 스폰
        private Joystick SpawnJoystick()
        {
            GameObject joystickObject = Instantiate(_levelDesignDataSo.JoystickSpawnData.Prefab, _canvas.transform);
            return joystickObject.GetComponent<Joystick>();
        }

        private void SpawnLevelObjects()
        {
            GameObject tilemapObject = Instantiate(_levelDesignDataSo.LevelSpawnData.Prefab, _grid.transform);
            
            tilemapObject.transform.localPosition = _levelDesignDataSo.LevelSpawnData.Position;
        }
    }
}