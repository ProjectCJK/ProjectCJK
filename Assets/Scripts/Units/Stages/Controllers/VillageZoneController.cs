using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Units;
using UnityEngine;
using Random = System.Random;

namespace Units.Stages.Controllers
{
    public interface IVillageZoneController :
        IRegisterReference<ICreatureController, IBuildingController, IHuntingZoneController, StageCustomSettings,
            List<EMaterialType>>, IInitializable
    {
        public IPlayer Player { get; }
        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }
    }

    public class VillageZoneController : MonoBehaviour, IVillageZoneController
    {
        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }

        [Header("=== 플레이어 생성 위치 ===")] [SerializeField]
        private Transform _playerSpawnPoint;

        [Header("=== 손님 NPC 생성 위치 ===")] [SerializeField]
        private Transform _guestSpawnPoint;

        public IPlayer Player { get; private set; }

        private ICreatureController _creatureController;
        private IBuildingController _buildingController;
        private IHuntingZoneController _huntingZoneController;
        private StageCustomSettings _stageCustomSettings;
        private readonly HashSet<IGuest> currentSpawnedGuests = new();
        private readonly HashSet<IDeliveryMan> currentSpawnedDeliveryMans = new();

        private GuestSpawnZoneDataSo _guestSpawnZoneDataSo;

        private float _guestSpawnElapsedTime;
        private float _guestSpawnCheckTime;
        private float _guestMaxCount => _stageCustomSettings.MaxGuestCount;
        private List<EMaterialType> _currentActiveStandType;

        private readonly Queue<Tuple<string, Transform>> _deliveryTargetQueue = new();

        public void RegisterReference(
            ICreatureController creatureController,
            IBuildingController buildingController,
            IHuntingZoneController huntingZoneController,
            StageCustomSettings stageCustomSettings,
            List<EMaterialType> currentActiveMaterials)
        {
            _creatureController = creatureController;
            _buildingController = buildingController;
            _huntingZoneController = huntingZoneController;

            _guestSpawnZoneDataSo = DataManager.Instance.GuestSpawnZoneDataSo;
            _stageCustomSettings = stageCustomSettings;
            _currentActiveStandType = currentActiveMaterials;
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
            {
                if (_currentActiveStandType.Count > 0)
                {
                    IGuest guest = _creatureController.GetGuest(_guestSpawnPoint.position, ReturnGuest);
                    guest.SetTargetPurchaseQuantity(1);
                    guest.SetDestinations(GetRandomDestinationForGuest());
                
                    currentSpawnedGuests.Add(guest);
                }
            }
#endif

            SpawnGuests();
            SpawnDeliveryMan();
            SpawnHunter();
            SetDeliveryManDestination();
        }

        private void SetDeliveryManDestination()
        {
            if (currentSpawnedDeliveryMans.Count <= 0) return;

            _deliveryTargetQueue.Clear();
            
            foreach (var building in _buildingController.Buildings)
            {
                if (building.Value is Kitchen { ActiveStatus: EActiveStatus.Active, IsAnyItemOnInventory: true } kitchen)
                {
                    var target = new Tuple<string, Transform>(kitchen.BuildingKey, kitchen.TradeZoneNpcTransform);
                    if (!_deliveryTargetQueue.Contains(target))
                    {
                        _deliveryTargetQueue.Enqueue(target);
                    }
                }
            }

            if (_deliveryTargetQueue.Count == 0)
            {
                foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
                {
                    if (deliveryMan.CommandState is CommandState.NoOrder or CommandState.Standby)
                    {
                        var defaultDestinationKey = $"{EBuildingType.Kitchen}_{EMaterialType.A}";
                        var defaultDestination = new Tuple<string, Transform>(defaultDestinationKey, _buildingController.Buildings[defaultDestinationKey].gameObject.transform);

                        deliveryMan.SetDestinations(defaultDestination);
                        deliveryMan.CommandState = CommandState.Standby;
                    }
                }
            }
            else
            {
                foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
                {
                    if (deliveryMan.CommandState is CommandState.NoOrder or CommandState.Standby)
                    {
                        Tuple<string, Transform> target = _deliveryTargetQueue.Peek();
                        deliveryMan.SetDestinations(target);
                        deliveryMan.CommandState = CommandState.MoveTo;
                    }
                }
            }

            foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
            {
                if (deliveryMan.IsInventoryFull() && deliveryMan.CommandState == CommandState.MoveTo)
                {
                    Tuple<string, Transform> destination = deliveryMan.GetDestination();
                    (EBuildingType?, EMaterialType?) parsedKey = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(destination.Item1);

                    var standKey = $"{EBuildingType.Stand}_{parsedKey.Item2}";
                    var standDestination = new Tuple<string, Transform>(standKey, _buildingController.Buildings[standKey].gameObject.transform);

                    deliveryMan.SetDestinations(standDestination);
                    deliveryMan.CommandState = CommandState.Deliver;
                }
                else if (!deliveryMan.HaveAnyItem() && deliveryMan.CommandState == CommandState.Deliver)
                {
                    deliveryMan.CommandState = CommandState.NoOrder;
                }
            }
        }

        private void SpawnDeliveryMan()
        {
            if (currentSpawnedDeliveryMans.Count < MaxDeliveryManCount())
            {
                IDeliveryMan deliveryMan = _creatureController.GetDeliveryMan(_buildingController.Buildings[$"{EBuildingType.DeliveryLodging}"].gameObject.transform.position);

                currentSpawnedDeliveryMans.Add(deliveryMan);
            }
        }
        
        private void SpawnHunter()
        {
            // if (currentSpawnedHunter.Count < MaxHunterCount())
            // {
            //     IHunter hunter = _creatureController.GetHunter(_buildingController.Buildings[$"{EBuildingType.WareHouse}"].gameObject.transform.position);
            //
            //     currentSpawnedHunter.Add(hunter);
            // }
        }

        private void SpawnPlayer()
        {
            Player = _creatureController.GetPlayer();
            Player.Initialize(_playerSpawnPoint.position, ReturnPlayer);

            OnRegisterPlayer?.Invoke(Player, true);
        }

        private void ReturnPlayer()
        {
            OnRegisterPlayer?.Invoke(Player, false);
        }

        private void SpawnGuests()
        {
            if (currentSpawnedGuests.Count < _guestMaxCount && _currentActiveStandType.Count > 0)
            {
                if (_guestSpawnCheckTime == 0f)
                {
                    _guestSpawnCheckTime = UnityEngine.Random.Range(_guestSpawnZoneDataSo.guestSpawnMinimumTime,
                        _guestSpawnZoneDataSo.guestSpawnMaximumTime);
                }

                _guestSpawnElapsedTime += Time.deltaTime;

                if (_guestSpawnElapsedTime >= _guestSpawnCheckTime)
                {
                    IGuest guest = _creatureController.GetGuest(_guestSpawnPoint.position, ReturnGuest);
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
            var randomIndex = new Random().Next(_currentActiveStandType.Count);
            var targetKey =
                ParserModule.ParseEnumToString(EBuildingType.Stand, _currentActiveStandType[randomIndex]);
            var managementDeskKey = ParserModule.ParseEnumToString(EBuildingType.ManagementDesk);

            var destinations = new List<Tuple<string, Transform>>
            {
                new(targetKey, _buildingController.Buildings[targetKey].TradeZoneNpcTransform),
                new(managementDeskKey, _buildingController.Buildings[managementDeskKey].TradeZoneNpcTransform),
                new(string.Empty, _guestSpawnPoint)
            };

            return destinations;
        }
        
        private int MaxDeliveryManCount()
        {
            if (_buildingController.Buildings.ContainsKey($"{EBuildingType.DeliveryLodging}"))
            {
                if (_buildingController.Buildings[$"{EBuildingType.DeliveryLodging}"] is DeliveryLodging { ActiveStatus: EActiveStatus.Active } deliveryLodging)
                {
                    return deliveryLodging.MaxDeliveryManCount;
                }
            }

            return 0;
        }
    }
}