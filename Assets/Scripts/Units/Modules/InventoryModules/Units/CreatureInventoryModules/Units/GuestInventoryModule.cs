using System;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Units;
using Unity.VisualScripting;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.CreatureInventoryModules.Units
{
    public interface IGuestInventoryModule : ICreatureInventoryModule
    {
        public event Action OnTargetQuantityReceived; 
    }

    public class GuestInventoryModule : CreatureInventoryModule, IGuestInventoryModule
    {
        public event Action OnTargetQuantityReceived; 
        public override ECreatureType CreatureType { get; }
        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        public GuestInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemFactory itemFactory,
            ECreatureType creatureType) : base(inventoryProperty)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            ItemFactory = itemFactory;
            CreatureType = creatureType;
        }
        
        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey);
            PushSpawnedItem(ReceiverTransform, item);
            
            if (!CanReceiveItem()) OnTargetQuantityReceived?.Invoke();
        }
    }
}