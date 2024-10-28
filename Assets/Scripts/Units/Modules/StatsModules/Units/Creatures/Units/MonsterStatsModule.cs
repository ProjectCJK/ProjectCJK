using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.HealthModules.Abstract;
using Units.Modules.MovementModules.Abstract;
using Units.Modules.StatsModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Interfaces;

namespace Units.Modules.StatsModules.Units
{
    public interface IMonsterStatsModule : ICreatureTypeProperty, IMovementProperty, IHealthProperty
    {
        
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