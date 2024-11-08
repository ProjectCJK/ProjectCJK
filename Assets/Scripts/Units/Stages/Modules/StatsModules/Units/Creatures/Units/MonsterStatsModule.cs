using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.HealthModules.Abstract;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Creatures.Units
{
    public interface IMonsterStatsModule : ICreatureTypeProperty, IMovementProperty, IHealthProperty
    {
        
    }

    public class MonsterStatsModule : StatsModule, IMonsterStatsModule
    {
        public ECreatureType CreatureType => ECreatureType.Monster;
        
        public float MovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed += value;
        }


        public int MaxHealth => _monsterDataSo.BaseHealth;

        private readonly MonsterDataSO _monsterDataSo;
        
        public MonsterStatsModule(MonsterDataSO monsterDataSo)
        {
            _monsterDataSo = monsterDataSo;
            movementSpeed = _monsterDataSo.BaseMovementSpeed;
        }
    }
}