using System;
using System.Collections.Generic;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Modules;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;
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

        private GuestSpawnZoneDataSo _guestSpawnZoneDataSo;
        
        private float _guestSpawnElapsedTime;
        private float _guestSpawnCheckTime;
        private float _guestMaxCount => _stageCustomSettings.MaxGuestCount;
        private List<EMaterialType> _currentActiveStandType;

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
                IGuest guest = _creatureController.GetGuest(_guestSpawnPoint.position, ReturnGuest);
                guest.SetTargetPurchaseQuantity(1);
                guest.SetDestinations(GetRandomDestinationForGuest());
                
                currentSpawnedGuests.Add(guest);
            }
#endif
            
            if (currentSpawnedGuests.Count < _guestMaxCount && _currentActiveStandType.Count > 0)
            {
                SpawnGuests();   
            }
        }

        private void SpawnGuests()
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
    }
}