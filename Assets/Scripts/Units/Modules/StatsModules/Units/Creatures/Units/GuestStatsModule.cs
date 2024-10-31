using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.CollisionModules.Abstract;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.MovementModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Modules.StatsModules.Units.Creatures.Units
{
    public interface IGuestStatModule : IMovementProperty, ICreatureTypeProperty, IInteractionProperty, IInventoryProperty, INPCProperty
    {
        public void SetMaxInventorySize(int targetPurchaseQuantity);
    }
    
    public class GuestStatModule : IGuestStatModule
    {
        public int MaxProductInventorySize { get; private set; }

        public ECreatureType CreatureType => ECreatureType.NPC;
        public ENPCType NPCType => ENPCType.Guest;
        public float MovementSpeed => _guestDataSo.BaseMovementSpeed;
        public float WaitingTime => _guestDataSo.BaseInteractionStandbySecond;
        private readonly GuestDataSO _guestDataSo;
        private ENPCType _creatureType;

        public GuestStatModule(GuestDataSO guestDataSo)
        {
            _guestDataSo = guestDataSo;
        }
        
        public void SetMaxInventorySize(int targetPurchaseQuantity)
        {
            MaxProductInventorySize = targetPurchaseQuantity;
        }
    }
}