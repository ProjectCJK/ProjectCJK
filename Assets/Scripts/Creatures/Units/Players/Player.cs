using Creatures.Abstract;
using Externals.Joystick.Scripts.Base;
using UnityEngine;

namespace Creatures.Units.Players
{
    public class Player : Creature
    {
        private PlayerStatSystem _playerStatSystem;
        private PlayerMovementSystem _playerMovementSystem;
        private PlayerInventorySystem _playerInventorySystem;
        
        private void Awake()
        {
            RegisterReference();
            Initialize();
        }

        private void RegisterReference()
        {
            _playerStatSystem = new PlayerStatSystem();
            
            _playerInventorySystem = GetComponent<PlayerInventorySystem>();
            _playerMovementSystem = GetComponent<PlayerMovementSystem>();
        }

        private void Initialize()
        {
            _playerMovementSystem.RegisterReference(_playerStatSystem);
        }
    }
}