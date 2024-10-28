using System;
using System.Collections.Generic;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface ICreatureController : IRegisterReference<Joystick, IItemFactory>
    {
        public Player GetPlayer();
        public IMonster GetMonster(EMaterialType materialType, Action<IMonster> onReturn);
        public Guest GetGuest(Transform targetBuildingTransform);
    }
    
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        [Header("=== 플레이어 세팅 ===")]
        [SerializeField] private Transform _playerSpawnPoint;
        
        [Header("=== 손님 NPC 세팅 ===")]
        [SerializeField] private Transform _customerSpawnPoint;
        
        private IPlayerFactory _playerFactory;
        private IMonsterFactory _monsterFactory;
        private IGuestFactory _guestFactory;
        private IItemFactory _itemFactory;

        private readonly HashSet<Guest> currentSpawnedGuests = new();

        public void RegisterReference(Joystick joystick, IItemFactory itemFactory)
        {
            _playerFactory = new PlayerFactory(_playerSpawnPoint.position, joystick, itemFactory);
            _monsterFactory = new MonsterFactory();
            _guestFactory = new GuestFactory();
        }

        public Player GetPlayer()
        {
            Player player = _playerFactory.GetPlayer();
            
            return player;
        }

        public IMonster GetMonster(EMaterialType materialType, Action<IMonster> onReturn)
        {
            IMonster monster = _monsterFactory.GetMonster(materialType, onReturn);
            
            return monster;
        }

        public Guest GetGuest(Transform targetBuilding)
        {
            Guest guest = _guestFactory.GetGuest();
            
            guest.Initialize(targetBuilding.position);
            guest.transform.position = _customerSpawnPoint.position;
            currentSpawnedGuests.Add(guest);
            
            return guest != null ? guest : null;
        }
    }
}