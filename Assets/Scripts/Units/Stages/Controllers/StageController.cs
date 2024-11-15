using System;
using System.Collections.Generic;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Managers;
using NavMeshPlus.Components;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.UnlockModules.Abstract;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.HuntingZones;
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
        public NavMeshSurface navigationSurface;
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
        [Space(20), Header("=== 스테이지 레벨 정의 ===")]
        public int StageLevel;
        [Space(20), Header("=== 재료 타입 정의 ===")]
        public List<MaterialMapping> materialMappings;
        [Space(20), Header("=== 최대 손님 수 ===")]
        public int MaxGuestCount;
        [Space(20), Header("=== 해금 조건 정의 ===")]
        public List<ActiveStatusSettings> activeStatusSettings;
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
        
        [Header("### Stage Custom Settings ### ")] [SerializeField]
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
            VolatileDataManager.Instance.SetCurrentStageLevel(_stageCustomSettings.StageLevel);
            
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
            
            InitializeZone();

            // _stageDefaultSettings.navigationSurface.BuildNavMeshAsync();
        }

        public void Initialize()
        {
            _buildingController.Initialize();
            _huntingZoneController.Initialize();
            _villageZoneController.Initialize();
            
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

                // 첫 번째 자식을 대상으로 UnlockZoneModule을 설정
                var targetChild = activeStatus.GameObject.transform.childCount > 0 
                    ? activeStatus.GameObject.transform.GetChild(0).gameObject 
                    : null;

                if (targetChild != null)
                {
                    var activeStatusModule = targetChild.GetComponent<UnlockZoneModule>();

                    if (activeStatusModule != null)
                    {
                        activeStatusModule.RequiredGoldForUnlock = activeStatus.RequiredGoldCountForUnlock;
                        activeStatusModule.SetCurrentState(activeStatus.InitialActiveStatus);
                        activeStatusModule.OnChangeActiveStatus += HandleOnChangeActiveStatus;
                    }
                    else
                    {
                        Debug.LogWarning($"UnlockZoneModule not found on the first child of {activeStatus.GameObject.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"No child found in {activeStatus.GameObject.name} to initialize UnlockZoneModule.");
                }
            }
        }

        private void HandleOnChangeActiveStatus(string targetKey, EActiveStatus activeStatus)
        {
            if (_buildingController.Buildings.ContainsKey(targetKey))
            {
                if (activeStatus == EActiveStatus.Active)
                {
                    QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Build, targetKey, 1);
                }

                BuildingZone targetBuilding = _buildingController.Buildings[targetKey];
                targetBuilding.HandleOnTriggerBuildingAnimation(EBuildingAnimatorParameter.Birth);
                VolatileDataManager.Instance.BuildingActiveStatuses[targetBuilding] = activeStatus;   
            }
            else if (_huntingZoneController.HuntingZones.ContainsKey(targetKey))
            {
                if (activeStatus == EActiveStatus.Active)
                {
                    QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Build, targetKey, 1);
                }

                HuntingZone huntingZone = _huntingZoneController.HuntingZones[targetKey];
                VolatileDataManager.Instance.HuntingZoneActiveStatuses[huntingZone] = activeStatus;   
            }

            if (activeStatusSettingIndex < _stageCustomSettings.activeStatusSettings.Count - 1)
            {
                ActiveStatusSettings activeStatusData = _stageCustomSettings.activeStatusSettings[++activeStatusSettingIndex];
                var activeStatusModule = activeStatusData.GameObject.GetComponent<UnlockZoneModule>();

                if (activeStatusModule != null)
                {
                    // 첫 번째 자식을 타겟으로 설정
                    GameObject targetChild = activeStatusData.GameObject.transform.childCount > 0
                        ? activeStatusData.GameObject.transform.GetChild(0).gameObject
                        : null;

                    if (targetChild != null && _buildingController.Buildings.ContainsKey(activeStatusModule.TargetKey))
                    {
                        VolatileDataManager.Instance.BuildingActiveStatuses[_buildingController.Buildings[activeStatusModule.TargetKey]] = EActiveStatus.Standby;
                    }

                    activeStatusModule.SetCurrentState(EActiveStatus.Standby);
                }
            }

            (EBuildingType?, EMaterialType?) parsedKey = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(targetKey);

            if (parsedKey is { Item1: EBuildingType.Stand, Item2: not null })
            {
                _currentActiveMaterials.Add(parsedKey.Item2.Value);
            }
        }
    }
}