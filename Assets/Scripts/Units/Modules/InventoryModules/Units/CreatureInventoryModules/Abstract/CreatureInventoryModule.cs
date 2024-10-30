using System;
using System.Collections.Generic;
using System.Linq;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract
{
    public interface ICreatureInventoryModule : IInventoryModule, ICreatureItemReceiver
    {
        
    }
    
    public abstract class CreatureInventoryModule : InventoryModule, ICreatureInventoryModule
    {
        public abstract ECreatureType CreatureType { get; }
        public abstract override IItemFactory ItemFactory { get; }
        public abstract override Transform SenderTransform { get; }
        public abstract override Transform ReceiverTransform { get; }
        public override int MaxInventorySize => InventoryProperty.MaxProductInventorySize;
        
        private readonly HashSet<IInteractionTrade> _interactionTradeZones = new();
        private readonly IInventoryProperty InventoryProperty;

        protected CreatureInventoryModule(IInventoryProperty inventoryProperty)
        {
            InventoryProperty = inventoryProperty;
        }

        public override void Initialize()
        {
            Inventory.Clear();
        }

        protected override void SendItem()
        {
            if (!IsReadyToSend() || _interactionTradeZones.Count == 0) return;

            foreach (IInteractionTrade targetZone in _interactionTradeZones.ToList())
            {
                ProcessInteractionZone(targetZone);
            }

            SetLastSendTime();
        }

        protected void ProcessInteractionZone(IInteractionTrade interactionZone)
        {
            if (interactionZone == null) return;
            
            var targetInputItemKey = interactionZone.InputItemKey;
            
            if (string.Equals(targetInputItemKey, $"{ECurrencyType.Money}"))
            {
                interactionZone.ReceiveItem(targetInputItemKey, SenderTransform.position);
            }
            else
            {
                if (Inventory.TryGetValue(targetInputItemKey, out var itemCount) && itemCount > 0)
                {
                    if (interactionZone.CanReceiveItem())
                    {
                        interactionZone.ReceiveItem(targetInputItemKey, SenderTransform.position);
                        RemoveItem(targetInputItemKey);   
                    }
                }
            }
        }

        public void RegisterItemReceiver(IInteractionTrade interactionZone, bool isConnected)
        {
            if (isConnected)
            {
                RegisterZone(interactionZone);
            }
            else
            {
                UnregisterZone(interactionZone);
            }
        }

        private void RegisterZone(IInteractionTrade interactionZone)
        {
            if (interactionZone.CheckOutputAccessor(CreatureType) && interactionZone.RegisterItemReceiver(this, true))
            {
                Debug.Log($"{interactionZone.BuildingKey} => {CreatureType} 도킹 완료 @~@");
            }

            if (interactionZone.CheckInputAccessor(CreatureType) && _interactionTradeZones.Add(interactionZone))
            {
                Debug.Log($"{CreatureType} => {interactionZone.BuildingKey} 도킹 완료 @~@");
            }
        }

        private void UnregisterZone(IInteractionTrade interactionZone)
        {
            if (interactionZone.CheckOutputAccessor(CreatureType) && interactionZone.RegisterItemReceiver(this, false))
            {
                Debug.Log($"{interactionZone.BuildingKey} => {CreatureType} 도킹 해제 완료 @~@");
            }

            if (interactionZone.CheckInputAccessor(CreatureType) && _interactionTradeZones.Remove(interactionZone))
            {
                Debug.Log($"{CreatureType} => {interactionZone.BuildingKey} 도킹 해제 완료 @~@");
            }
        }
    }
}