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
        public Transform PlayerTransform { get; }
        public IPlayer GetPlayer();
        public IMonster GetMonster(EMaterialType materialType, Action<IMonster> onReturn);
        public IGuest GetGuest(Action<IGuest> onReturn);
    }
    
    public class CreatureController : MonoBehaviour, ICreatureController
    {
        public Transform PlayerTransform => _playerFactory.PlayerTransform;
        
        private IPlayerFactory _playerFactory;
        private IMonsterFactory _monsterFactory;
        private IGuestFactory _guestFactory;
        private IItemFactory _itemFactory;

        public void RegisterReference(Joystick joystick, IItemFactory itemFactory)
        {
            _playerFactory = new PlayerFactory(joystick, itemFactory);
            _monsterFactory = new MonsterFactory();
            _guestFactory = new GuestFactory();
        }

        public IPlayer GetPlayer()
        {
            IPlayer player = _playerFactory.GetPlayer();
            
            return player;
        }

        public IMonster GetMonster(EMaterialType materialType, Action<IMonster> onReturn)
        {
            IMonster monster = _monsterFactory.GetMonster(materialType, onReturn);
            
            return monster;
        }

        public IGuest GetGuest(Action<IGuest> onReturn)
        {
            IGuest guest = _guestFactory.GetGuest(onReturn);
            
            return guest;
        }
    }
}