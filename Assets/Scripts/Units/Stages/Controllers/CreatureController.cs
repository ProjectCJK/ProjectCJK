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
    public interface ICreatureController : IRegisterReference<PlayerFactory, MonsterFactory, GuestFactory>
    {
        public Transform PlayerTransform { get; }
        public IPlayer GetPlayer();
        public IMonster GetMonster(Vector3 randomSpawnPoint, EMaterialType materialType, Action<IMonster> onReturn);
        public IGuest GetGuest(Vector3 startPosition, Action<IGuest> onReturn);
    }
    
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        public Transform PlayerTransform => _playerFactory.PlayerTransform;
        
        private IPlayerFactory _playerFactory;
        private IMonsterFactory _monsterFactory;
        private IGuestFactory _guestFactory;
        
        public void RegisterReference(PlayerFactory playerFactory, MonsterFactory monsterFactory, GuestFactory guestFactory)
        {
            _playerFactory = playerFactory;
            _monsterFactory = monsterFactory;
            _guestFactory = guestFactory;
        }

        public IPlayer GetPlayer()
        {
            IPlayer player = _playerFactory.GetPlayer();
            
            return player;
        }

        public IMonster GetMonster(Vector3 randomSpawnPoint, EMaterialType materialType, Action<IMonster> onReturn)
        {
            IMonster monster = _monsterFactory.GetMonster(randomSpawnPoint, materialType, onReturn);
            
            return monster;
        }

        public IGuest GetGuest(Vector3 startPosition, Action<IGuest> onReturn)
        {
            IGuest guest = _guestFactory.GetGuest(startPosition, onReturn);
            
            return guest;
        }
    }
}