using System;
using Interfaces;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Controllers
{
    public interface ICreatureController : IRegisterReference<PlayerFactory, MonsterFactory, GuestFactory,
        DeliveryManFactory, HunterFactory>
    {
        public Transform PlayerTransform { get; }
        public IPlayer GetPlayer();
        public IMonster GetMonster(Vector3 randomSpawnPoint, EMaterialType materialType, Action<IMonster> onReturn);
        public IGuest GetGuest(Vector3 startPosition, Action<IGuest> onReturn);
        public IDeliveryMan GetDeliveryMan(Vector3 startPosition);
        public IHunter GetHunter(Vector3 startPosition);
    }

    public class CreatureController : MonoBehaviour, ICreatureController
    {
        private IDeliveryManFactory _deliveryManFactory;
        private IGuestFactory _guestFactory;
        private IHunterFactory _hunterManFactory;
        private IMonsterFactory _monsterFactory;

        private IPlayerFactory _playerFactory;
        public Transform PlayerTransform => _playerFactory.PlayerTransform;

        public void RegisterReference(
            PlayerFactory playerFactory,
            MonsterFactory monsterFactory,
            GuestFactory guestFactory,
            DeliveryManFactory deliveryManFactory,
            HunterFactory hunterFactory)
        {
            _playerFactory = playerFactory;
            _monsterFactory = monsterFactory;
            _guestFactory = guestFactory;
            _deliveryManFactory = deliveryManFactory;
            _hunterManFactory = hunterFactory;
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

        public IDeliveryMan GetDeliveryMan(Vector3 startPosition)
        {
            IDeliveryMan deliveryMan = _deliveryManFactory.GetDeliveryMan(startPosition);

            return deliveryMan;
        }

        public IHunter GetHunter(Vector3 startPosition)
        {
            IHunter hunter = _hunterManFactory.GetHunter(startPosition);

            return hunter;
        }
    }
}