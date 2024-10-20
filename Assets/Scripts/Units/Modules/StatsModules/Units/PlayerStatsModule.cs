using System;
using ScriptableObjects.Scripts;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Abstract;
using Units.Stages.Creatures.Enums;

namespace Units.Modules.StatsModules.Units
{
    public interface IPlayerStatsModule : IMovementProperty, IInventoryProperty
    {
        public ECreatureType CreatureType { get; }
    }
    
    [Serializable]
    public class PlayerStatsModule : StatsModule, IPlayerStatsModule
    {
        public ECreatureType CreatureType => _playerDataSo.creatureType;
        public float MovementSpeed => _playerDataSo.BaseMovementSpeed;
        public float WaitingTime => _playerDataSo.BaseInteractionStandbySecond;
        public int MaxInventorySize => _playerDataSo.BaseInventorySize;

        private PlayerDataSo _playerDataSo;

        public PlayerStatsModule(PlayerDataSo playerDataSo)
        {
            _playerDataSo = playerDataSo;
            
            // TODO : 스탯을 런타임 중에 바꾸는 기능이 추가되면 (카드 시스템, 광고 보상으로 인한 일시적 스탯 강화 등) 값을 복사하고 이 값을 수정하는 식으로 해야 함.
            // MovementSpeed = playerStatSo.BaseMovementSpeed;
            // InventorySize = playerStatSo.BaseInventorySize;
        }
    }
}