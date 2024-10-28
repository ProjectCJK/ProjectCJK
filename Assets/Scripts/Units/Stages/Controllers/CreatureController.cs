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
    public interface ICreatureController : IRegisterReference<Joystick, IItemController>, IInitializable
    {
        public Player Player { get; }
        public Monster GetMonster(EMaterialType materialType, Action<Monster> onReturn);
    }
    
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        public Player Player => _playerController.Player;
        public IMonsterFactory MonsterFactory => _monsterController.MonsterFactory;
        public IGuestFactory GuestFactory => _guestController.GuestFactory;
        
        [Header("=== 플레이어 세팅 ===")]
        [SerializeField] private Transform _playerSpawnPoint;
        
        [Header("=== 손님 NPC 세팅 ===")]
        [SerializeField] private Transform _customerSpawnPoint;
        
        private IPlayerController _playerController;
        private IMonsterController _monsterController;
        private IGuestController _guestController;

        private readonly HashSet<Guest> currentSpawnedGuests = new();
        
        public void RegisterReference(Joystick joystick, IItemController itemController)
        {
            _playerController = new PlayerController(_playerSpawnPoint, joystick, itemController);
            _monsterController = new MonsterController();
            _guestController = new GuestController();
        }
        
        public void Initialize()
        {
            _playerController.Initialize();
        }

        private void Update()
        {
#if UNITY_EDITOR
            // TODO : Cheat Code
            if (Input.GetKeyDown(KeyCode.E))
            {
                GetGuest();
            }      
#endif
        }

        public Monster GetMonster(EMaterialType materialType, Action<Monster> onReturn)
        {
            var monster = ObjectPoolManager.Instance.GetObject<Monster>(MonsterFactory.PoolKey, null);
            
            monster.Initialize(MonsterFactory.MonsterSprites[materialType], () =>
            {
                ObjectPoolManager.Instance.ReturnObject(MonsterFactory.PoolKey, monster);
                onReturn?.Invoke(monster);
            });

            return monster != null ? monster : null;
        }

        public Guest GetGuest()
        {
            var guest = ObjectPoolManager.Instance.GetObject<Guest>(GuestFactory.PoolKey, null);
            
            guest.Initialize(new Vector3(0, 10, 0));
            
            guest.transform.position = _customerSpawnPoint.position;
            currentSpawnedGuests.Add(guest);
            
            return guest != null ? guest : null;
        }
    }
}