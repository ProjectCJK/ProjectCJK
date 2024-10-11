using System.Collections.Generic;
using Controllers;
using Enums;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules;
using ScriptableObjects.Scripts;
using Units.Games.Buildings.Controllers;
using Units.Games.Creatures.Controllers;
using UnityEngine;

namespace Units.Managers
{
    public class MainSceneManager : SceneSingleton<MainSceneManager>, IRegisterReference
    {
        [Header("Default Settings")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Grid _grid;
        [SerializeField] private CameraController _cameraController;

        [Header("PlayerSettings")]
        [SerializeField] private PlayerStatSO _playerStatSo;
        
        [Header("BuildingSettings")]
        [SerializeField] private List<BuildingStatSO> _buildingStatSo;
        
        [Header("LevelSettings")]
        [SerializeField] private LevelDesignSO _levelDesignSo;

        private ICreatureController _creatureController;
        private IBuildingController _buildingController;
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
            _creatureController = new CreatureController(new CreatureFactory(_playerStatSo, _joystick));
            _buildingController = new BuildingController(new BuildingFactory(_buildingStatSo, _grid));
        }

        // 씬 초기 스폰 오브젝트들 처리
        private void InstantiateLevels()
        {
            _joystick = SpawnJoystick();
            SpawnLevelObjects();
        }
        
        private void InstantiateUnits()
        {
            _creatureController.SpawnPlayer();
            _buildingController.SpawnBuilding();
        }

        // 카메라가 플레이어를 추적하도록 설정
        private void RegisterCameraToPlayer()
        {
            _cameraController.RegisterReference(_creatureController.GetPlayerTransform());
        }

        // 조이스틱 스폰
        private Joystick SpawnJoystick()
        {
            GameObject joystickObject = Instantiate(_levelDesignSo.JoystickSpawnData.Prefab, _canvas.transform);
            return joystickObject.GetComponent<Joystick>();
        }

        // 레벨 오브젝트 스폰
        private void SpawnLevelObjects()
        {
            Instantiate(_levelDesignSo.LevelSpawnData.Prefab, _grid.transform);
        }
    }
}