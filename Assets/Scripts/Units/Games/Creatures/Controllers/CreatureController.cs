using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Games.Buildings.Controllers;
using Units.Games.Creatures.Abstract;
using UnityEngine;

namespace Units.Games.Creatures.Controllers
{
    public interface ICreatureController : IInitializable
    {
        public void SpawnPlayer();
        public Transform GetPlayerTransform();
    }
    
    public class CreatureController : ICreatureController
    {
        private readonly ICreatureFactory _creatureFactory;
        private Creature _player;
        
        public CreatureController(ICreatureFactory creatureFactory)
        {
            _creatureFactory = creatureFactory;
        }
        
        public void Initialize()
        {
            _player.Initialize();
        }

        public void SpawnPlayer()
        {
            _player = _creatureFactory.CreateCreature();
        }

        public Transform GetPlayerTransform() => _player.transform;
    }
}