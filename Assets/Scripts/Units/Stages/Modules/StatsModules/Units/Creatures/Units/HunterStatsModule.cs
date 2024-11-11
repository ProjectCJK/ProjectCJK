using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.BattleModules.Abstract;
using Units.Stages.Modules.CollisionModules.Abstract;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Creatures.Units
{
    public interface IHunterStatsModule : IMovementProperty, ICreatureTypeProperty, IInteractionProperty,
        IInventoryProperty, INPCProperty, IBattleProperty
    {
    }

    public class HunterStatsModule : StatsModule, IHunterStatsModule
    {
        private readonly HunterDataSO _hunterDataSo;
        private float _attackDelay;
        private int _damage;

        public HunterStatsModule(HunterDataSO hunterDataSo)
        {
            _hunterDataSo = hunterDataSo;
            MovementSpeed = _hunterDataSo.BaseMovementSpeed;
            Damage = _hunterDataSo.BaseDamage;
            AttackDelay = _hunterDataSo.BaseAttackDelay;
        }

        public int MaxProductInventorySize => _hunterDataSo.BaseInventorySize;

        public ECreatureType CreatureType => ECreatureType.NPC;
        public ENPCType NPCType => ENPCType.Hunter;

        public float MovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed = value;
        }

        public int Damage
        {
            get => _damage;
            set => _damage += value;
        }

        public float AttackDelay
        {
            get => _attackDelay;
            set => _attackDelay += value;
        }

        public float WaitingTime => _hunterDataSo.BaseInteractionStandbySecond;
    }
}