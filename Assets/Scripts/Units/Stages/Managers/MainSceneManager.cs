using Externals.Joystick.Scripts.Base;
using Modules.DesignPatterns.Singletons;
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
        
        private IStageController _stageController;

        
        private Joystick _joystick;

        private void Awake()
        {
            InstantiateJoystick();
            InstantiateStage();
            
            _stageController.RegisterReference(_joystick);
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
            _cameraController.RegisterReference(_stageController.GetPlayer().Transform);
        }
    }
}