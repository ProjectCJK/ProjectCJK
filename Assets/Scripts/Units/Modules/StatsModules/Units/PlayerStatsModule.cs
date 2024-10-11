using System;
using ScriptableObjects.Scripts;
using Units.Games.Creatures.Enums;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Abstract;

namespace Units.Modules.StatsModules.Units
{
    public interface IPlayerStatsModule : IMovementProperty, IInventoryProperty
    {
        public ECreatureType CreatureType { get; }
    }
    
    [Serializable]
    public class PlayerStatsModule : StatsModule, IPlayerStatsModule
    {
        public ECreatureType CreatureType => _playerStatSo.creatureType;
        public float MovementSpeed => _playerStatSo.BaseMovementSpeed;
        public float WaitingTime => _playerStatSo.BaseInteractionStandbySecond;
        public int MaxInventorySize => _playerStatSo.BaseInventorySize;

        private PlayerStatSO _playerStatSo;

        public PlayerStatsModule(PlayerStatSO playerStatSo)
        {
            _playerStatSo = playerStatSo;
            
            // TODO : 스탯을 런타임 중에 바꾸는 기능이 추가되면 (카드 시스템, 광고 보상으로 인한 일시적 스탯 강화 등) 값을 복사하고 이 값을 수정하는 식으로 해야 함.
            // MovementSpeed = playerStatSo.BaseMovementSpeed;
            // InventorySize = playerStatSo.BaseInventorySize;
        }
    }
}