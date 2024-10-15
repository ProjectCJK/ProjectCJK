using System.Collections.Generic;
using Controllers;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules;
using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Games.Buildings.Controllers;
using Units.Games.Creatures.Controllers;
using Units.Games.Items;
using Units.Games.Items.Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Units.Managers
{
    public class MainSceneManager : SceneSingleton<MainSceneManager>, IRegisterReference
    {
        [Header("Default Settings")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Grid _grid;
        [SerializeField] private CameraController _cameraController;
        
        [Header("PlayerSettings")]
        [SerializeField] private PlayerDataSo _playerDataSo;
        
        [Header("BuildingSettings")]
        [SerializeField] private List<BuildingDataSO> _buildingStatSo;
        
        [Header("ItemSettings")]
        [SerializeField] private ItemDataSO _itemDataSo;
        
        [Header("LevelSettings")]
        [SerializeField] private LevelDesignDataSO _levelDesignDataSo;

        private ICreatureController _creatureController;
        private IBuildingController _buildingController;
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
            _creatureController = new CreatureController(new CreatureFactory(_playerDataSo, _joystick, _itemController));
            _buildingController = new BuildingController(new BuildingFactory(_buildingStatSo, _grid, _itemController));
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
            _buildingController.InstantiateBuilding();
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