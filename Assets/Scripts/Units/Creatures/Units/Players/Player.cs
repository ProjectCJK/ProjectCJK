using System;
using DefaultNamespace;
using Enums;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Creatures.Abstract;
using UnityEngine;

namespace Units.Creatures.Units.Players
{
    public class Player : BaseCreature, IReferenceRegisterable, IReferenceInitializable
    {
        private event Action<GameObject> OnInteractionTrigger;
        
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
            
            _playerInventorySystem = new PlayerInventorySystem();
            _playerInventorySystem.RegisterReference(_playerStatSystem);
            
            _playerMovementSystem = GetComponent<PlayerMovementSystem>();
            _playerMovementSystem.RegisterReference(_playerStatSystem, OnInteractionTrigger);
            
            _playerMovementSystem.OnInteractionTrigger += _ => Debug.Log("Interaction triggered");
            _playerMovementSystem.OnInteractionTrigger += GameObject => Debug.Log($"{GameObject.name}");
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