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

        public CreatureController CreatureController => _stageDefaultSettings.stageReferences.CreatureController;
        public BuildingController BuildingController => _stageDefaultSettings.stageReferences.BuildingController;
        public HuntingZoneController HuntingZoneController => _stageDefaultSettings.stageReferences.HuntingZoneController;

        private IVillageZoneController _villageZoneController =>
            _stageDefaultSettings.stageReferences.VillageZoneController;

        public Transform PlayerTransform => CreatureController.PlayerTransform;

        public void RegisterReference(Joystick joystick)
        {
            GameManager.Instance.ES3Saver.CurrentStageLevel = _stageCustomSettings.StageLevel;

            if (GameManager.Instance.ES3Saver.UpgradeZoneTrigger == false && _stageCustomSettings.StageLevel == 2)
            {
                GameManager.Instance.ES3Saver.UpgradeZoneTrigger = true;
            }
            
            InitializeManager();

            var itemFactory = new ItemFactory(transform, _stageCustomSettings.materialMappings);
            var playerFactory = new PlayerFactory(joystick, itemFactory);
            var monsterFactory = new MonsterFactory(_stageCustomSettings.materialMappings);
            var guestFactory = new GuestFactory(itemFactory);
            var deliveryManFactory = new DeliveryManFactory(itemFactory);
            var hunterFactory = new HunterFactory(itemFactory);

            CreatureController.RegisterReference(playerFactory, monsterFactory, guestFactory, deliveryManFactory, hunterFactory);
            BuildingController.RegisterReference(itemFactory);
            _villageZoneController.RegisterReference(CreatureController, BuildingController, HuntingZoneController, _stageCustomSettings);
            HuntingZoneController.RegisterReference(CreatureController, itemFactory, _villageZoneController.Player);

            _villageZoneController.OnRegisterPlayer += HuntingZoneController.HandleOnRegisterPlayer;
            
            InitializeZone();

            // _stageDefaultSettings.navigationSurface.BuildNavMeshAsync();
        }

        public void Initialize()
        {
            BuildingController.Initialize();
            HuntingZoneController.Initialize();
            _villageZoneController.Initialize();
            
            ObjectTrackerManager.Instance.Initialize();
            QuestManager.Instance.Initialize();

            if (!GameManager.Instance.ES3Saver.first_CurrentStage && GameManager.Instance.ES3Saver.CurrentStageLevel == 1)
            {
                GameManager.Instance.ES3Saver.first_CurrentStage = true;
                
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_stage {GameManager.Instance.ES3Saver.CurrentStageLevel}");
            }
            else if (!GameManager.Instance.ES3Saver.second_CurrentStage && GameManager.Instance.ES3Saver.CurrentStageLevel == 2)
            {
                GameManager.Instance.ES3Saver.second_CurrentStage = true;
                
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_stage {GameManager.Instance.ES3Saver.CurrentStageLevel}");
            }
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
            var savedIndex = GameManager.Instance.ES3Saver.ActiveStatusSettingIndex;
            
            for (var index = 0; index < _stageCustomSettings.activeStatusSettings.Count; index++)
            {
                ActiveStatusSettings activeStatus = _stageCustomSettings.activeStatusSettings[index];

                if (index < savedIndex)
                {
                    firstLockZoneFounded = true;
                }
                else if (activeStatus.InitialActiveStatus != EActiveStatus.Active && !firstLockZoneFounded)
                {
                    firstLockZoneFounded = true;
                    
                    GameManager.Instance.ES3Saver.ActiveStatusSettingIndex = index;
                }

                var activeStatusModule = activeStatus.GameObject.transform.GetComponentInChildren<UnlockZoneModule>();

                if (activeStatusModule != null)
                {
                    var savedStatus = EActiveStatus.Lock;
                        
                    if (activeStatusModule.TryGetComponent(out BuildingZone building))
                    {
                        // 로드된 상태 적용
                        savedStatus = GameManager.Instance.ES3Saver.BuildingActiveStatuses.ContainsKey(building.BuildingKey)
                            ? GameManager.Instance.ES3Saver.BuildingActiveStatuses[building.BuildingKey]
                            : activeStatus.InitialActiveStatus;
                    }
                    else if (activeStatusModule.TryGetComponent(out HuntingZone huntingZone))
                    {
                        savedStatus = GameManager.Instance.ES3Saver.HuntingZoneActiveStatuses.ContainsKey(huntingZone.HuntingZoneKey)
                            ? GameManager.Instance.ES3Saver.HuntingZoneActiveStatuses[huntingZone.HuntingZoneKey]
                            : activeStatus.InitialActiveStatus;
                    }
                    

                    activeStatusModule.RequiredGoldForUnlock = activeStatus.RequiredGoldCountForUnlock;
                    activeStatusModule.SetCurrentState(savedStatus);
                    activeStatusModule.OnChangeActiveStatus += HandleOnChangeActiveStatus;

                    // 상태 동기화
                    SyncActiveStatus(activeStatusModule.TargetKey, savedStatus);
                }
            }
        }

        private void HandleOnChangeActiveStatus(string targetKey, EActiveStatus activeStatus)
        {
            // 기존 건물 상태 처리
            if (BuildingController.Buildings.ContainsKey(targetKey))
            {
                if (activeStatus == EActiveStatus.Active)
                {
                    // 퀘스트 진행 상황 업데이트
                    QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Build, targetKey, 1);

                    // 재료 관련 데이터 추가
                    (EBuildingType?, EMaterialType?) parsedKey = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(targetKey);
                    if (parsedKey is { Item1: EBuildingType.StandA, Item2: not null })
                    {
                        VolatileDataManager.Instance.CurrentActiveMaterials.Add(parsedKey.Item2.Value);
                    }
                }

                // 건물 애니메이션 트리거 실행
                BuildingZone targetBuilding = BuildingController.Buildings[targetKey];
                targetBuilding.HandleOnTriggerBuildingAnimation(EBuildingAnimatorParameter.Birth);

                // 저장 데이터 동기화
                GameManager.Instance.ES3Saver.BuildingActiveStatuses[targetBuilding.BuildingKey] = activeStatus;
            }
            // 기존 사냥터 상태 처리
            else if (HuntingZoneController.HuntingZones.ContainsKey(targetKey))
            {
                if (activeStatus == EActiveStatus.Active)
                {
                    // 퀘스트 진행 상황 업데이트
                    QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Build, targetKey, 1);
                }

                // 사냥터 상태 저장
                HuntingZone huntingZone = HuntingZoneController.HuntingZones[targetKey];
                GameManager.Instance.ES3Saver.HuntingZoneActiveStatuses[huntingZone.HuntingZoneKey] = activeStatus;
            }

            // 다음 활성화 상태로 진행
            if (GameManager.Instance.ES3Saver.ActiveStatusSettingIndex < _stageCustomSettings.activeStatusSettings.Count - 1)
            {
                GameManager.Instance.ES3Saver.ActiveStatusSettingIndex += 1; 
                ActiveStatusSettings nextActiveStatusData = _stageCustomSettings.activeStatusSettings[GameManager.Instance.ES3Saver.ActiveStatusSettingIndex];

                var nextActiveStatusModule = nextActiveStatusData.GameObject.GetComponentInChildren<UnlockZoneModule>();
                if (nextActiveStatusModule != null)
                {
                    GameObject nextTargetChild = nextActiveStatusData.GameObject.transform.childCount > 0
                        ? nextActiveStatusData.GameObject.transform.GetChild(0).gameObject
                        : null;

                    if (nextTargetChild != null)
                    {
                        // 다음 대기 상태를 Standby로 설정
                        nextActiveStatusModule.SetCurrentState(EActiveStatus.Standby);

                        if (BuildingController.Buildings.ContainsKey(nextActiveStatusModule.TargetKey))
                        {
                            GameManager.Instance.ES3Saver.BuildingActiveStatuses[BuildingController.Buildings[nextActiveStatusModule.TargetKey].BuildingKey] = EActiveStatus.Standby;
                        }
                        else if (HuntingZoneController.HuntingZones.ContainsKey(nextActiveStatusModule.TargetKey))
                        {
                            GameManager.Instance.ES3Saver.HuntingZoneActiveStatuses[HuntingZoneController.HuntingZones[nextActiveStatusModule.TargetKey].HuntingZoneKey] = EActiveStatus.Standby;
                        }
                    }
                }
            }
        }

        private void SyncActiveStatus(string targetKey, EActiveStatus activeStatus)
        {
            if (BuildingController.Buildings.ContainsKey(targetKey))
            {
                BuildingZone targetBuilding = BuildingController.Buildings[targetKey];
                
                GameManager.Instance.ES3Saver.BuildingActiveStatuses[targetBuilding.BuildingKey] = activeStatus;

                if (activeStatus == EActiveStatus.Active)
                {
                    (EBuildingType?, EMaterialType?) parsedKey = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(targetKey);

                    if (parsedKey is { Item1: EBuildingType.StandA, Item2: not null })
                    {
                        VolatileDataManager.Instance.CurrentActiveMaterials.Add(parsedKey.Item2.Value);
                    }

                    if (GameManager.Instance.ES3Saver.SuperHunterInitialTrigger == false)
                    {
                        GameManager.Instance.ES3Saver.SuperHunterInitialTrigger = true;
                        VolatileDataManager.Instance.SuperHunterTrigger = true;
                    }
                }
            }
            else if (HuntingZoneController.HuntingZones.ContainsKey(targetKey))
            {
                HuntingZone huntingZone = HuntingZoneController.HuntingZones[targetKey];
                VolatileDataManager.Instance.HuntingZoneActiveStatuses[huntingZone] = activeStatus;
            }
        }
    }
}