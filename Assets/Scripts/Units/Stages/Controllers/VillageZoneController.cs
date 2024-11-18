using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Interfaces;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.HuntingZones;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using Random = System.Random;

namespace Units.Stages.Controllers
{
    public interface IVillageZoneController :
        IRegisterReference<ICreatureController, IBuildingController, HuntingZoneController, StageCustomSettings>, IInitializable
    {
        public IPlayer Player { get; }
        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }
    }

    [Serializable]
    public struct VillageSpawnData
    {
        [Header("=== Village Position ===")]
        public Transform VillageSpawner;
        public Transform PlayerSpawner;
        public List<Transform> GuestSpawner;
    }

    public class VillageZoneController : MonoBehaviour, IVillageZoneController
    {
        [SerializeField] private VillageSpawnData _villageSpawnData;

        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }
        public IPlayer Player { get; private set; }
        
        private readonly Queue<Tuple<string, Transform>> _deliveryTargetQueue = new();
        private readonly HashSet<IDeliveryMan> currentSpawnedDeliveryMans = new();
        private readonly HashSet<IGuest> currentSpawnedGuests = new();
        private readonly HashSet<IHunter> currentSpawnedHunters = new();
        private IBuildingController _buildingController;
        private ICreatureController _creatureController;
        private float _guestSpawnCheckTime;
        private float _guestSpawnElapsedTime;
        private GuestSpawnZoneDataSo _guestSpawnZoneDataSo;
        private IHuntingZoneController _huntingZoneController;
        private StageCustomSettings _stageCustomSettings;

        private Dictionary<HuntingZone, EActiveStatus> currentHuntingZones = new();
        private float _guestMaxCount => _stageCustomSettings.MaxGuestCount;

        public void RegisterReference(
            ICreatureController creatureController,
            IBuildingController buildingController,
            HuntingZoneController huntingZoneController,
            StageCustomSettings stageCustomSettings)
        {
            _creatureController = creatureController;
            _buildingController = buildingController;
            _huntingZoneController = huntingZoneController;

            _guestSpawnZoneDataSo = DataManager.Instance.GuestSpawnZoneDataSo;
            _stageCustomSettings = stageCustomSettings;

            currentHuntingZones = VolatileDataManager.Instance.HuntingZoneActiveStatuses;

            foreach (IGuest guest in ObjectPoolManager.Instance.GetAllObjects<IGuest>("GuestPool"))
            {
                guest.Transform.position = _villageSpawnData.GuestSpawner[0].transform.position;
            }

            if (_buildingController.Buildings.ContainsKey($"{EBuildingType.DeliveryLodging}"))
            {
                foreach (IDeliveryMan deliveryMan in ObjectPoolManager.Instance.GetAllObjects<IDeliveryMan>("DeliveryManPool"))
                {
                    deliveryMan.Transform.position = _buildingController.Buildings[$"{EBuildingType.DeliveryLodging}"].transform.position;
                }
            }

            if (_buildingController.Buildings.ContainsKey($"{EBuildingType.WareHouse}"))
            {
                foreach (IHunter hunter in ObjectPoolManager.Instance.GetAllObjects<IHunter>("HunterPool"))
                {
                    hunter.Transform.position = _buildingController.Buildings[$"{EBuildingType.WareHouse}"].transform.position;
                }
            }

            InstantiateLevels();
        }

        private void InstantiateLevels()
        {
            GameObject prefab = DataManager.Instance.levelPrefabSo.Levels[VolatileDataManager.Instance.CurrentStageLevel - 1].Village;
            GameObject village = Instantiate(prefab, _villageSpawnData.VillageSpawner);
            village.transform.localPosition = Vector3.zero;
            
            prefab = DataManager.Instance.levelPrefabSo.GuestSpawner;

            foreach (GameObject guestSpawner in _villageSpawnData.GuestSpawner.Select(t => Instantiate(prefab, t)))
            {
                guestSpawner.transform.localPosition = Vector3.zero;
            }
            
            prefab = DataManager.Instance.levelPrefabSo.PlayerSpawner;
            GameObject playerSpawner = Instantiate(prefab, _villageSpawnData.PlayerSpawner);
            playerSpawner.transform.localPosition = Vector3.zero;
        }

        public void Initialize()
        {
            SpawnPlayer();
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Cheat Code
            if (Input.GetKeyDown(KeyCode.E))
                if (VolatileDataManager.Instance.CurrentActiveMaterials.Count > 0)
                {
                    var randomPosition = new Random().Next(_villageSpawnData.GuestSpawner.Count);
                    IGuest guest = _creatureController.GetGuest(_villageSpawnData.GuestSpawner[randomPosition].position, ReturnGuest);
                    guest.SetTargetPurchaseQuantity(1);
                    guest.SetDestinations(GetRandomDestinationForGuest());

                    currentSpawnedGuests.Add(guest);
                }
#endif

            SpawnGuests();
            SpawnDeliveryMan();
            SpawnHunter();
            SpawnMonster();
            SetHunterDestination();
            SetDeliveryManDestination();
            _huntingZoneController.SendDroppedItem(currentSpawnedHunters);
        }

        private void SetDeliveryManDestination()
        {
            if (currentSpawnedDeliveryMans.Count <= 0) return;

            _deliveryTargetQueue.Clear();

            foreach (KeyValuePair<string, BuildingZone> building in _buildingController.Buildings)
                if (building.Value is Kitchen
                    {
                        ActiveStatus: EActiveStatus.Active, IsAnyItemOnInventory: true
                    } kitchen)
                {
                    var target = new Tuple<string, Transform>(kitchen.BuildingKey, kitchen.TradeZoneNpcTransform);
                    if (!_deliveryTargetQueue.Contains(target)) _deliveryTargetQueue.Enqueue(target);
                }

            if (_deliveryTargetQueue.Count == 0)
            {
                foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
                    if (deliveryMan.CommandState is CommandState.NoOrder or CommandState.Standby)
                    {
                        var defaultDestinationKey = $"{EBuildingType.Kitchen}_{EMaterialType.A}";
                        var defaultDestination = new Tuple<string, Transform>(defaultDestinationKey,
                            _buildingController.Buildings[defaultDestinationKey].gameObject.transform);

                        deliveryMan.SetDestinations(defaultDestination);
                        deliveryMan.CommandState = CommandState.Standby;
                    }
            }
            else
            {
                foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
                    if (deliveryMan.CommandState is CommandState.NoOrder or CommandState.Standby)
                    {
                        Tuple<string, Transform> target = _deliveryTargetQueue.Peek();
                        deliveryMan.SetDestinations(target);
                        deliveryMan.CommandState = CommandState.MoveTo;
                    }
            }

            foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
                if (deliveryMan.IsInventoryFull() && deliveryMan.CommandState == CommandState.MoveTo)
                {
                    Tuple<string, Transform> destination = deliveryMan.GetDestination();
                    (EBuildingType?, EMaterialType?) parsedKey =
                        ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(destination.Item1);

                    var standKey = $"{EBuildingType.Stand}_{parsedKey.Item2}";
                    var standDestination = new Tuple<string, Transform>(standKey,
                        _buildingController.Buildings[standKey].gameObject.transform);

                    deliveryMan.SetDestinations(standDestination);
                    deliveryMan.CommandState = CommandState.Deliver;
                }
                else if (!deliveryMan.HaveAnyItem() && deliveryMan.CommandState == CommandState.Deliver)
                {
                    deliveryMan.CommandState = CommandState.NoOrder;
                }
        }

        private void SetHunterDestination()
        {
            // 1. 헌터가 없으면 로직 종료
            if (currentSpawnedHunters.Count <= 0) return;

            // 2. 현재 활성화된 모든 헌터를 대상으로 타겟 설정
            foreach (IHunter hunter in currentSpawnedHunters)
            {
                if (!hunter.IsInventoryFull() && hunter.CommandState == CommandState.Deliver)
                    hunter.CommandState = CommandState.NoOrder;

                // 2-1. 헌터 인벤토리가 가득 찬 경우 복귀
                if (hunter.IsInventoryFull() && hunter.CommandState != CommandState.Deliver)
                {
                    hunter.SetDestinations(new Tuple<string, Transform>($"{EBuildingType.WareHouse}",
                        _buildingController.Buildings[$"{EBuildingType.WareHouse}"].transform)); // 복귀 위치 설정
                    hunter.CommandState = CommandState.Deliver;
                    continue; // 다른 조건을 확인하지 않고 다음 헌터로 넘어감
                }

                // 2-2. 헌터가 NoOrder 상태라면 새로운 타겟 몬스터 탐색
                if (hunter.CommandState == CommandState.NoOrder)
                {
                    IMonster closestMonster = null;
                    var closestDistance = float.MaxValue;

                    // 가장 가까운 활성화된 몬스터 찾기
                    foreach (KeyValuePair<HuntingZone, EActiveStatus> huntingZone in currentHuntingZones)
                        if (huntingZone.Value == EActiveStatus.Active)
                            foreach (IMonster monster in huntingZone.Key.CurrentSpawnedMonsters)
                                if (monster.Transform.gameObject.activeInHierarchy)
                                {
                                    var distance = Vector3.Distance(hunter.Transform.position,
                                        monster.Transform.position);
                                    if (distance < closestDistance)
                                    {
                                        closestDistance = distance;
                                        closestMonster = monster;
                                    }
                                }

                    // 가장 가까운 몬스터를 타겟으로 지정
                    if (closestMonster != null)
                    {
                        hunter.SetDestinations(new Tuple<string, Transform>($"{ECreatureType.Monster}",
                            closestMonster.Transform));
                        hunter.CommandState = CommandState.MoveTo;
                    }
                }

                // 2-3. 추적 중인 타겟 몬스터가 비활성화되었는지 확인
                if (hunter.CommandState == CommandState.MoveTo)
                {
                    Transform currentTarget = hunter.GetDestination().Item2;
                    if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
                        // 타겟이 비활성화되었으므로 헌터를 NoOrder 상태로 전환하여 다른 타겟을 대기
                        hunter.CommandState = CommandState.NoOrder;
                }
            }
        }

        private void SpawnDeliveryMan()
        {
            if (_buildingController.Buildings.TryGetValue($"{EBuildingType.DeliveryLodging}",
                    out BuildingZone buildingZone))
                if (buildingZone is DeliveryLodging deliveryLodging)
                    deliveryLodging.SpawnDeliveryMan(_creatureController, currentSpawnedDeliveryMans);
        }

        private void SpawnHunter()
        {
            if (_buildingController.Buildings.TryGetValue($"{EBuildingType.WareHouse}", out BuildingZone buildingZone))
            {
                if (buildingZone is WareHouse wareHouse)
                {
                    wareHouse.SpawnHunter(_creatureController, currentSpawnedHunters);                    
                }
            }
        }

        private void SpawnMonster()
        {
            _huntingZoneController.SpawnMonsters();
        }

        private void SpawnPlayer()
        {
            Player = _creatureController.GetPlayer();
            Player.Initialize(_villageSpawnData.PlayerSpawner.position, ReturnPlayer);

            OnRegisterPlayer?.Invoke(Player, true);
        }

        private void ReturnPlayer()
        {
            OnRegisterPlayer?.Invoke(Player, false);
        }

        private void SpawnGuests()
        {
            if (currentSpawnedGuests.Count < _guestMaxCount && VolatileDataManager.Instance.CurrentActiveMaterials.Count > 0)
            {
                if (_guestSpawnCheckTime == 0f)
                    _guestSpawnCheckTime = UnityEngine.Random.Range(_guestSpawnZoneDataSo.guestSpawnMinimumTime,
                        _guestSpawnZoneDataSo.guestSpawnMaximumTime);

                _guestSpawnElapsedTime += Time.deltaTime;

                if (_guestSpawnElapsedTime >= _guestSpawnCheckTime)
                {
                    var randomPosition = new Random().Next(_villageSpawnData.GuestSpawner.Count);
                    IGuest guest = _creatureController.GetGuest(_villageSpawnData.GuestSpawner[randomPosition].position, ReturnGuest);
                    guest.SetTargetPurchaseQuantity(1);
                    guest.SetDestinations(GetRandomDestinationForGuest());

                    currentSpawnedGuests.Add(guest);

                    _guestSpawnElapsedTime = 0f;
                    _guestSpawnCheckTime = 0f;
                }
            }
        }

        private void ReturnGuest(IGuest guest)
        {
            currentSpawnedGuests.Remove(guest);
        }

        private List<Tuple<string, Transform>> GetRandomDestinationForGuest()
        {
            List<EMaterialType> materialsList = VolatileDataManager.Instance.CurrentActiveMaterials.ToList();
            var randomIndex = new Random().Next(materialsList.Count);
            var targetKey = ParserModule.ParseEnumToString(EBuildingType.Stand, materialsList[randomIndex]);

            var managementDeskKey = ParserModule.ParseEnumToString(EBuildingType.ManagementDesk);
            var randomPosition = new Random().Next(_villageSpawnData.GuestSpawner.Count);
            
            var destinations = new List<Tuple<string, Transform>>
            {
                new(targetKey, _buildingController.Buildings[targetKey].TradeZoneNpcTransform),
                new(managementDeskKey, _buildingController.Buildings[managementDeskKey].TradeZoneNpcTransform),
                new(string.Empty, _villageSpawnData.GuestSpawner[randomPosition])
            };

            return destinations;
        }
    }
}