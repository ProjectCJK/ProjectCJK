using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.CollisionModules.Abstract;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Creatures.Units
{
    public interface IDeliveryManStatsModule : IMovementProperty, ICreatureTypeProperty, IInteractionProperty,
        IInventoryProperty, INPCProperty
    {
    }

    public class DeliveryManStatsModule : StatsModule, IDeliveryManStatsModule
    {
        private readonly DeliveryManDataSO _deliveryManDataSo;

        public DeliveryManStatsModule(DeliveryManDataSO deliveryManDataSo)
        {
            _deliveryManDataSo = deliveryManDataSo;
            MovementSpeed = _deliveryManDataSo.BaseMovementSpeed;
        }

        public int MaxProductInventorySize => _deliveryManDataSo.BaseInventorySize;

        public ECreatureType CreatureType => ECreatureType.NPC;
        public ENPCType NPCType => ENPCType.DeliveryMan;

        public float MovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed = value;
        }

        public float WaitingTime => _deliveryManDataSo.BaseInteractionStandbySecond;
    }
}