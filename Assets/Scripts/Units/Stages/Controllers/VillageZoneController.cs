using System;
using System.Collections.Generic;
using Interfaces;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface IVillageZoneController : IInitializable
    {
        public void RegisterReference(ICreatureController creatureController, IBuildingController buildingController, IHuntingZoneController huntingZoneController);
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
        
        private readonly HashSet<IGuest> currentSpawnedGuests = new();

        public void RegisterReference(ICreatureController creatureController, IBuildingController buildingController, IHuntingZoneController huntingZoneController)
        {
            _creatureController = creatureController;
            _buildingController = buildingController;
            _huntingZoneController = huntingZoneController;
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
                IGuest guest = _creatureController.GetGuest(ReturnGuest);
                guest.Initialize(() => ReturnGuest(guest));
                guest.SetDestinations(_guestSpawnPoint.position, GetRandomDestination());
                guest.Transform.position = _guestSpawnPoint.position;
                
                currentSpawnedGuests.Add(guest);
            }
#endif
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

        private List<Transform> GetRandomDestination()
        {
            var destinations = new List<Transform>
            {
                _buildingController.Buildings[new Tuple<EBuildingType, EMaterialType>(EBuildingType.Stand, EMaterialType.A)].transform
            };

            return destinations;
        }
    }
}