using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.CollisionModules.Abstract;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Creatures.Units
{
    public interface IGuestStatModule : IMovementProperty, ICreatureTypeProperty, IInteractionProperty, IInventoryProperty, INPCProperty
    {
        public void SetMaxInventorySize(int targetPurchaseQuantity);
    }
    
    public class GuestStatModule : StatsModule, IGuestStatModule
    {
        public int MaxProductInventorySize { get; private set; }

        public ECreatureType CreatureType => ECreatureType.NPC;
        public ENPCType NPCType => ENPCType.Guest;
        
        public float MovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed += value;
        }


        public float WaitingTime => _guestDataSo.BaseInteractionStandbySecond;
        
        private readonly GuestDataSO _guestDataSo;

        public GuestStatModule(GuestDataSO guestDataSo)
        {
            _guestDataSo = guestDataSo;
            MovementSpeed = _guestDataSo.BaseMovementSpeed;
        }
        
        public void SetMaxInventorySize(int targetPurchaseQuantity)
        {
            MaxProductInventorySize = targetPurchaseQuantity;
        }
    }
}