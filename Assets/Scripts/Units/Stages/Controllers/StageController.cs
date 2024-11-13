using System;
using System.Collections.Generic;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Managers;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IStageController : IRegisterReference<Joystick>, IInitializable
    {
        public Transform PlayerTransform { get; }
    }

    [Serializable]
    public struct StageReferences
    {
        public CreatureController CreatureController;
        public BuildingController BuildingController;
        public HuntingZoneController HuntingZoneController;
        public VillageZoneController VillageZoneController;
    }

    [Serializable]
    public struct StageDefaultSettings
    {
        public StageReferences stageReferences;
    }

    [Serializable]
    public struct MaterialMapping
    {
        public EMaterialType MaterialType;
        public EStageMaterialType StageMaterialType;
    }

    [Serializable]
    public struct StageCustomSettings
    {
        [Header("--- 스테이지 레벨 정의 ---")] public int StageLevel;

        [Header("--- 재료 타입 정의 ---")] public List<MaterialMapping> materialMappings;

        [Header("최대 손님 수")] public int MaxGuestCount;

        [Header("--- 해금 조건 정의 ---")] public List<ActiveStatusSettings> activeStatusSettings;
    }

    [Serializable]
    public struct ActiveStatusSettings
    {
        public GameObject GameObject;
        public EActiveStatus InitialActiveStatus;
        public int RequiredGoldCountForUnlock;
    }

    public class StageController : MonoBehaviour, IStageController
    {
        [Header("### Stage Default Settings ### ")] [SerializeField]
        private StageDefaultSettings _stageDefaultSettings;

        [Space(10)] [Header("### Stage Custom Settings ### ")] [SerializeField]
        private StageCustomSettings _stageCustomSettings;

        private readonly List<EMaterialType> _currentActiveMaterials = new();

        private int activeStatusSettingIndex;

        private ICreatureController _creatureController => _stageDefaultSettings.stageReferences.CreatureController;
        private IBuildingController _buildingController => _stageDefaultSettings.stageReferences.BuildingController;

        private HuntingZoneController _huntingZoneController =>
            _stageDefaultSettings.stageReferences.HuntingZoneController;

        private IVillageZoneController _villageZoneController =>
            _stageDefaultSettings.stageReferences.VillageZoneController;

        public Transform PlayerTransform => _creatureController.PlayerTransform;

        public void RegisterReference(Joystick joystick)
        {
            InitializeZone();
            InitializeManager();

            var itemFactory = new ItemFactory(transform, _stageCustomSettings.materialMappings);
            var playerFactory = new PlayerFactory(joystick, itemFactory);
            var monsterFactory = new MonsterFactory(_stageCustomSettings.materialMappings);
            var guestFactory = new GuestFactory(itemFactory);
            var deliveryManFactory = new DeliveryManFactory(itemFactory);
            var hunterFactory = new HunterFactory(itemFactory);

            _creatureController.RegisterReference(playerFactory, monsterFactory, guestFactory, deliveryManFactory,
                hunterFactory);
            _buildingController.RegisterReference(itemFactory, _currentActiveMaterials);
            _villageZoneController.RegisterReference(_creatureController, _buildingController, _huntingZoneController,
                _stageCustomSettings, _currentActiveMaterials);
            _huntingZoneController.RegisterReference(_creatureController, itemFactory, _villageZoneController.Player);

            _villageZoneController.OnRegisterPlayer += _huntingZoneController.HandleOnRegisterPlayer;
        }

        public void Initialize()
        {
            _buildingController.Initialize();
            _huntingZoneController.Initialize();
            _villageZoneController.Initialize();
            
            VolatileDataManager.Instance.SetCurrentStageLevel(_stageCustomSettings.StageLevel);
            QuestManager.Instance.InitializeQuestData();
        }

        private void InitializeManager()
        {
            var materialMappings = new Dictionary<EMaterialType, EStageMaterialType>();
            foreach (MaterialMapping materialMapping in _stageCustomSettings.materialMappings)
                materialMappings.TryAdd(materialMapping.MaterialType, materialMapping.StageMaterialType);

            VolatileDataManager.Instance.MaterialMappings = materialMappings;
        }

        private void InitializeZone()
        {
            var firstLockZoneFounded = false;
            for (var index = 0; index < _stageCustomSettings.activeStatusSettings.Count; index++)
            {
                ActiveStatusSettings activeStatus = _stageCustomSettings.activeStatusSettings[index];

                if (activeStatus.InitialActiveStatus != EActiveStatus.Active && !firstLockZoneFounded)
                {
                    firstLockZoneFounded = true;
                    activeStatusSettingIndex = index;
                }

                var activeStatusModule = activeStatus.GameObject.GetComponent<UnlockZoneModule>();

                activeStatusModule.RequiredGoldForUnlock = activeStatus.RequiredGoldCountForUnlock;
                activeStatusModule.SetCurrentState(activeStatus.InitialActiveStatus);

                activeStatusModule.OnChangeActiveStatus += HandleOnChangeActiveStatus;
            }
        }

        private void HandleOnChangeActiveStatus(string targetKey, EActiveStatus activeStatus)
        {
            if (_buildingController.Buildings.ContainsKey(targetKey))
            {
                if (activeStatus == EActiveStatus.Active)
                {
                    QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Build, targetKey);
                }

                BuildingZone targetBuilding = _buildingController.Buildings[targetKey];
                targetBuilding.HandleOnTriggerBuildingAnimation(EBuildingAnimatorParameter.Birth);
                _buildingController.BuildingActiveStatuses[targetBuilding] = activeStatus;   
            }

            if (activeStatusSettingIndex < _stageCustomSettings.activeStatusSettings.Count - 1)
            {
                var activeStatusModule = _stageCustomSettings.activeStatusSettings[++activeStatusSettingIndex].GameObject.GetComponent<UnlockZoneModule>();

                if (_buildingController.Buildings.ContainsKey(activeStatusModule.TargetKey))
                {
                    _buildingController.BuildingActiveStatuses[_buildingController.Buildings[activeStatusModule.TargetKey]] = EActiveStatus.Standby;   
                }

                activeStatusModule.SetCurrentState(EActiveStatus.Standby);
            }

            (EBuildingType?, EMaterialType?) parsedKey = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(targetKey);
            
            if (parsedKey is { Item1: EBuildingType.Stand, Item2: not null })
            {
             _currentActiveMaterials.Add(parsedKey.Item2.Value);   
            }
        }
    }
}