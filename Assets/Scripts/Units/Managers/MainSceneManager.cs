using System.Collections.Generic;
using Controllers;
using Enums;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Buildings.Controllers;
using Units.Creatures.Controllers;
using UnityEngine;

namespace Units.Managers
{
    public class MainSceneManager : MonoBehaviour, IReferenceRegisterable
    {
        [Header("Default Settings")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Grid _grid;
        [SerializeField] private CameraController _cameraController;
        
        [Header("Temp Settings")]
        [SerializeField] private LevelDesignSO _levelDesignSo;
        [SerializeField] private List<EMaterialType> _materialType;

        private ICreatureController _creatureController;
        private IBuildingController _buildingController;
        private Joystick _joystick;

        // 의존성 주입을 위한 생성자
        public MainSceneManager(ICreatureController creatureController, IBuildingController buildingController)
        {
            _creatureController = creatureController;
            _buildingController = buildingController;
        }

        private void Awake()
        {
            _creatureController = new CreatureController(new PlayerFactory());
            _buildingController = gameObject.AddComponent<BuildingController>();

            InitializeScene();
        }

        private void Start()
        {
            _creatureController.Initialize();
            _buildingController.Initialize();
        }

        // 전체적인 초기화 프로세스를 하나의 메서드로 통합
        private void InitializeScene()
        {
            RegisterReference();
            SpawnInitialObjects();
            RegisterCameraToPlayer();
        }

        public void RegisterReference()
        {
            _creatureController.RegisterReference(_materialType, _buildingController);
            _buildingController.RegisterReference(_materialType);
        }

        // 씬 초기 스폰 오브젝트들 처리
        private void SpawnInitialObjects()
        {
            _joystick = SpawnJoystick();
            SpawnLevelObjects();
            _creatureController.SpawnPlayer(_levelDesignSo.PlayerSpawnData, _joystick);
            _buildingController.SpawnBuilding(_levelDesignSo.BuildingSpawnData, _grid);
        }

        // 카메라가 플레이어를 추적하도록 설정
        private void RegisterCameraToPlayer()
        {
            _cameraController.RegisterReference(_creatureController.GetPlayerTransform());
        }

        // 조이스틱 스폰 메서드 분리
        private Joystick SpawnJoystick()
        {
            GameObject joystickObject = Instantiate(_levelDesignSo.JoystickSpawnData.prefab, _canvas.transform);
            return joystickObject.GetComponent<Joystick>();
        }

        // 레벨 오브젝트 스폰 메서드 분리
        private void SpawnLevelObjects()
        {
            Instantiate(_levelDesignSo.LevelSpawnData.prefab, _grid.transform);
        }
    }
}