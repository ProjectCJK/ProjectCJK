using System;
using ScriptableObjects.Scripts;
using Units.Creatures.Interfaces;

namespace Units.Creatures.Units.Players
{
    [Serializable]
    public class PlayerStatSystem : IMovementProperty, IInventoryProperty
    {
        public float MovementSpeed => _playerStatSo.BaseMovementSpeed;
        public float WaitingTime => _playerStatSo.BaseInteractionStandbySecond;
        public int InventorySize => _playerStatSo.BaseInventorySize;

        private PlayerStatSO _playerStatSo;

        public PlayerStatSystem(PlayerStatSO playerStatSo)
        {
            _playerStatSo = playerStatSo;
            
            // TODO : 스탯을 런타임 중에 바꾸는 기능이 추가되면 (카드 시스템, 광고 보상으로 인한 일시적 스탯 강화 등) 값을 복사하고 이 값을 수정하는 식으로 해야 함.
            // MovementSpeed = playerStatSo.BaseMovementSpeed;
            // InventorySize = playerStatSo.BaseInventorySize;
        }
    }
}