using System;
using Enums;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Creatures.Abstract;
using Units.Systems.InventorySystems.Units;
using Units.Systems.MovementSystems.Units;
using Units.Systems.StatSystems.Units;
using UnityEngine;

namespace Units.Creatures.Units.Players
{
    public class Player : BaseCreature, IReferenceRegisterable, IReferenceInitializable
    {
        [SerializeField] private PlayerStatSO _playerStatSo;
        
        private PlayerStatSystem _playerStatSystem;
        private PlayerMovementSystem _playerMovementSystem;
        private PlayerInventorySystem _playerInventorySystem;
        
        //TODO : CreatureController를 통해 초기화 진행
        private void Awake()
        {
            RegisterReference();
            InitializeReference();
        }

        public void RegisterReference()
        {
            _playerStatSystem = new PlayerStatSystem(_playerStatSo);
            
            _playerInventorySystem = GetComponent<PlayerInventorySystem>();
            _playerInventorySystem.RegisterReference(_playerStatSystem);
            
            _playerMovementSystem = GetComponent<PlayerMovementSystem>();
            _playerMovementSystem.RegisterReference(_playerStatSystem);
            
            _playerMovementSystem.OnTrade += _playerInventorySystem.RegisterTradeTarget;
        }

        public void InitializeReference()
        {
            _playerMovementSystem.Initialize();
            _playerInventorySystem.Initialize();
        }

        public bool ReceiveMaterial(EMaterial item)
        {
            return _playerInventorySystem.ReceiveMaterial(item);
        }
    }
}