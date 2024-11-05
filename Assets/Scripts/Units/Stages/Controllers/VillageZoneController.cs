using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Abstract;
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Units;
using UnityEngine;
using Random = System.Random;

namespace Units.Stages.Controllers
{
    public interface IVillageZoneController : IRegisterReference<ICreatureController, IBuildingController, IHuntingZoneController, StageCustomSettings, List<EMaterialType>>, IInitializable
    {
        public IPlayer Player { get; }
        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }
    }
    
    public class VillageZoneController : MonoBehaviour, IVillageZoneController
    {
        public Action<IPlayer, bool> OnRegisterPlayer { get; set; }
        
        [Header("=== 플레이어 생성 위치 ===")]
        [SerializeField] private Transform _playerSpawnPoint;
        
        [Header("=== 손님 NPC 생성 위치 ===")]
        [SerializeField] private Transform _guestSpawnPoint;

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
        
        private Queue<Tuple<string, Transform>> _deliveryTargetQueue = new();

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

            foreach (KeyValuePair<string, BuildingZone> buildingZone in _buildingController.Buildings)
            {
                if (buildingZone.Value is Kitchen kitchen)
                {
                    kitchen.OnKitchenProductExisted += HandleOnKitchenProductExisted;
                } 
            }
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
                
                SpawnDeliveryMan();
            }
#endif
            
            SpawnGuests();
            SetDeliveryManDestination();
        }

        private void SetDeliveryManDestination()
        {
            if (currentSpawnedDeliveryMans.Count <= 0) return;

            if (_deliveryTargetQueue.Count <= 0)
            {
                foreach (IDeliveryMan currentSpawnedDeliveryMan in currentSpawnedDeliveryMans)
                {
                    if (currentSpawnedDeliveryMan.CommandState == CommandState.NoOrder)
                    {
                        var defaultDestinationBuildingKey = $"{EBuildingType.Kitchen}_{EMaterialType.A}";
                        var defaultDestination = new Tuple<string, Transform>(defaultDestinationBuildingKey, _buildingController.Buildings[defaultDestinationBuildingKey].gameObject.transform);
                        currentSpawnedDeliveryMan.SetDestinations(defaultDestination);
                        currentSpawnedDeliveryMan.CommandState = CommandState.Standby;
                    }
                }
            }
            else
            {
                foreach (IDeliveryMan currentSpawnedDeliveryMan in currentSpawnedDeliveryMans)
                {
                    if (currentSpawnedDeliveryMan.CommandState != CommandState.Deliver && currentSpawnedDeliveryMan.CommandState != CommandState.MoveTo)
                    {
                        Tuple<string, Transform> target = _deliveryTargetQueue.Peek();
                        currentSpawnedDeliveryMan.SetDestinations(target);
                        currentSpawnedDeliveryMan.CommandState = CommandState.MoveTo;
                    }
                }
            }
            
            foreach (IDeliveryMan currentSpawnedDeliveryMan in currentSpawnedDeliveryMans)
            {
                if (currentSpawnedDeliveryMan.IsInventoryFull() && currentSpawnedDeliveryMan.CommandState == CommandState.MoveTo)
                {
                    Tuple<string, Transform> destination = currentSpawnedDeliveryMan.GetDestination();
                    (EBuildingType?, EMaterialType?) parsedKey = EnumParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(destination.Item1);
                    var newKey = $"{EBuildingType.Stand}_{parsedKey.Item2}";
                    var newTarget = new Tuple<string, Transform>(newKey, _buildingController.Buildings[newKey].gameObject.transform);
                    currentSpawnedDeliveryMan.SetDestinations(newTarget);
                    currentSpawnedDeliveryMan.CommandState = CommandState.Deliver;
                }
                else if (!currentSpawnedDeliveryMan.HaveAnyItem() && currentSpawnedDeliveryMan.CommandState == CommandState.Deliver)
                {
                    currentSpawnedDeliveryMan.CommandState = CommandState.NoOrder;
                }
            }
        }
        
        private void SpawnDeliveryMan()
        {
            // TODO : 배달원 숙소 건물이 있다면
            IDeliveryMan deliveryMan = _creatureController.GetDeliveryMan(_buildingController.Buildings[$"{EBuildingType.ManagementDesk}"].gameObject.transform.position);

            currentSpawnedDeliveryMans.Add(deliveryMan);
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
                    _guestSpawnCheckTime = UnityEngine.Random.Range(_guestSpawnZoneDataSo.guestSpawnMinimumTime, _guestSpawnZoneDataSo.guestSpawnMaximumTime);
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
            var targetKey = EnumParserModule.ParseEnumToString(EBuildingType.Stand, _currentActiveStandType[randomIndex]);
            var managementDeskKey = EnumParserModule.ParseEnumToString(EBuildingType.ManagementDesk);
            
            var destinations = new List<Tuple<string, Transform>>
            {
                new(targetKey, _buildingController.Buildings[targetKey].TradeZoneNpcTransform),
                new(managementDeskKey, _buildingController.Buildings[managementDeskKey].TradeZoneNpcTransform),
                new(string.Empty, _guestSpawnPoint)
            };

            return destinations;
        }
        
        private void HandleOnKitchenProductExisted(string targetKey, bool value)
        {
            var target = new Tuple<string, Transform>(targetKey, _buildingController.Buildings[targetKey].gameObject.transform);

            if (value)
            {
                if (!_deliveryTargetQueue.Contains(target))
                {
                    (EBuildingType?, EMaterialType?) parsedKey = EnumParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(targetKey);

                    if (_buildingController.BuildingActiveStatuses[_buildingController.Buildings[$"{EBuildingType.Stand}_{parsedKey.Item2}"]] == EActiveStatus.Active)
                    {
                        _deliveryTargetQueue.Enqueue(target);   
                    }
                }   
            }
            else
            {
                if (_deliveryTargetQueue.Contains(target))
                {
                    var newQueue = new Queue<Tuple<string, Transform>>();

                    while (_deliveryTargetQueue.Count > 0)
                    {
                        Tuple<string, Transform> item = _deliveryTargetQueue.Dequeue();
                        if (!item.Equals(target)) newQueue.Enqueue(item);
                    }

                    _deliveryTargetQueue = newQueue;
                }
            }
        }
    }
}