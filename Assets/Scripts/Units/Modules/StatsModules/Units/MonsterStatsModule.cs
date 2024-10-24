using ScriptableObjects.Scripts.Creatures;
using Units.Modules.HealthModules.Abstract;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Interfaces;

namespace Units.Modules.StatsModules.Units
{
    public interface IMonsterStatsModule : IMovementProperty, IHealthProperty
    {
        public ECreatureType Type { get; }
    }
    
    public class MonsterStatsModule : StatsModule, IMonsterStatsModule
    {
        public ECreatureType Type => _monsterDataSo.type;
        public float MovementSpeed => _monsterDataSo.BaseMovementSpeed;
        public int MaxHealth => _monsterDataSo.BaseHealth;

        private readonly MonsterDataSO _monsterDataSo;
        
        public MonsterStatsModule(MonsterDataSO monsterDataSo)
        {
            _monsterDataSo = monsterDataSo;
        }
    }
}