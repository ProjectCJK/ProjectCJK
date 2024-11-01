using Externals.Joystick.Scripts.Base;
using Managers;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Controllers;
using UnityEngine;

namespace Units.Stages.Managers
{
    public class MainSceneManager : SceneSingleton<MainSceneManager>
    {
        [Header("Stage Settings")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _joystickPrefab;
        [SerializeField] private GameObject _StagePrefab;
        [SerializeField] private CameraController _cameraController;

        private ICurrencyManager _currencyManager;
        private IStageController _stageController;
        
        private Joystick _joystick;

        private void Awake()
        {
            InstantiateManagers();
            InstantiateJoystick();
            InstantiateStage();
            
            _stageController.RegisterReference(_joystick);
        }
        
        private void Start()
        {
            RegisterCameraToPlayer();
            
            _stageController.Initialize();
        }
        
        private void InstantiateManagers()
        {
            _currencyManager = new CurrencyManager();
        }

        
        private void InstantiateJoystick()
        {
            GameObject obj = Instantiate(_joystickPrefab, _canvas.transform);
            _joystick = obj.GetComponent<Joystick>();
        }

        private void InstantiateStage()
        {
            GameObject obj = Instantiate(_StagePrefab);
            _stageController = obj.GetComponent<StageController>();
        }
        
        private void RegisterCameraToPlayer()
        {
            _cameraController.RegisterReference(_stageController.PlayerTransform);
        }
    }
}