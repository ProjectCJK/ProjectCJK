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
        public void InstantiatePlayer();
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
        
        public void InstantiatePlayer()
        {
            _player = _creatureFactory.CreateCreature();
        }
        
        public void Initialize()
        {
            _player.Initialize();
        }

        public Transform GetPlayerTransform() => _player.transform;
    }
}