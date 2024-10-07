using System;
using Enums;
using Externals.Joystick.Scripts.Base;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Buildings.Abstract;
using Units.Creatures.Abstract;
using Units.Systems.InventorySystems.Units;
using Units.Systems.MovementSystems.Units;
using Units.Systems.StatSystems.Units;
using UnityEngine;

namespace Units.Creatures.Units.Players
{
    public class Player : BaseCreature, IReferenceRegisterable<Joystick, Action<int?, bool>>, IInitializable
    {
        [SerializeField] private PlayerStatSO _playerStatSo;
        
        private PlayerStatSystem _playerStatSystem;
        private PlayerMovementSystem _playerMovementSystem;
        private PlayerInventorySystem _playerInventorySystem;

        public void RegisterReference(Joystick joystick, Action<int?, bool> action)
        {
            _playerStatSystem = new PlayerStatSystem(_playerStatSo);
            
            _playerInventorySystem = GetComponent<PlayerInventorySystem>();
            _playerInventorySystem.RegisterReference(_playerStatSystem);
            
            _playerMovementSystem = GetComponent<PlayerMovementSystem>();
            _playerMovementSystem.RegisterReference(joystick, _playerStatSystem);

            _playerMovementSystem.OnTriggerBuildingInteraction += action;
        }
        
        public override void Initialize()
        {
            _playerMovementSystem.Initialize();
            _playerInventorySystem.Initialize();
        }
        
        // 플레이어의 인벤토리에서 빌딩으로 아이템 전송
        public void TransferItemToBuilding(BaseBuilding building)
        {
            _playerInventorySystem.TransferItem(building);  // 인벤토리에서 아이템을 빌딩으로 전송
        }

        // 빌딩에서 아이템을 받을 수 있는지 확인
        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey)
        {
            return _playerInventorySystem.HasItem(InventoryKey);  // 인벤토리에서 일치하는 아이템 확인
        }

        // 빌딩으로부터 아이템 수신
        public void ReceiveItem(Tuple<EMaterialType, EItemType> itemKey)
        {
            _playerInventorySystem.AddItem(itemKey.Item2, itemKey.Item1);  // 아이템을 인벤토리에 추가
        }

        // 플레이어의 인벤토리가 가득 찼는지 확인
        public bool InventoryIsFull()
        {
            return _playerInventorySystem.InventoryIsFull();  // 인벤토리 상태 확인
        }
    }
}