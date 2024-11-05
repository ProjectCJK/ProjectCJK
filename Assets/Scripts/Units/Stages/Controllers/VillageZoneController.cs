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
                // if (_currentActiveStandType.Count > 0)
                // {
                //     IGuest guest = _creatureController.GetGuest(_guestSpawnPoint.position, ReturnGuest);
                //     guest.SetTargetPurchaseQuantity(1);
                //     guest.SetDestinations(GetRandomDestinationForGuest());
                //
                //     currentSpawnedGuests.Add(guest);
                // }

                SpawnDeliveryMan();
            }
#endif

            SpawnGuests();
            SetDeliveryManDestination();
        }
        
        private void SetDeliveryManDestination()
        {
            if (currentSpawnedDeliveryMans.Count <= 0) return;

            // 모든 Kitchen을 순회하며 활성 상태 및 아이템 보유 여부를 확인해 타겟 큐 업데이트
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

            // 타겟 큐가 비어 있을 경우 기본 위치로 이동하도록 설정
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
                // 타겟 큐에 빌딩이 있을 경우 Standby 또는 NoOrder 상태의 배달원에게 타겟을 할당
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

            // 배달 중인 배달원이 목적지에 도착하여 아이템을 수령했는지 확인, 수령 시 Stand로 전환
            foreach (IDeliveryMan deliveryMan in currentSpawnedDeliveryMans)
            {
                if (deliveryMan.IsInventoryFull() && deliveryMan.CommandState == CommandState.MoveTo)
                {
                    Tuple<string, Transform> destination = deliveryMan.GetDestination();
                    (EBuildingType?, EMaterialType?) parsedKey = EnumParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(destination.Item1);

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
            // TODO : 배달원 숙소 건물이 있다면
            IDeliveryMan deliveryMan = _creatureController.GetDeliveryMan(_buildingController
                .Buildings[$"{EBuildingType.ManagementDesk}"].gameObject.transform.position);

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
                EnumParserModule.ParseEnumToString(EBuildingType.Stand, _currentActiveStandType[randomIndex]);
            var managementDeskKey = EnumParserModule.ParseEnumToString(EBuildingType.ManagementDesk);

            var destinations = new List<Tuple<string, Transform>>
            {
                new(targetKey, _buildingController.Buildings[targetKey].TradeZoneNpcTransform),
                new(managementDeskKey, _buildingController.Buildings[managementDeskKey].TradeZoneNpcTransform),
                new(string.Empty, _guestSpawnPoint)
            };

            return destinations;
        }
    }
}