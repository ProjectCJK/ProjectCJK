using System;
using System.Collections.Generic;
using System.Linq;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
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

        protected readonly HashSet<ITradeZone> interactionTradeZones = new();

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
            if (!IsReadyToSend() || interactionTradeZones.Count == 0) return;

            foreach (ITradeZone targetZone in interactionTradeZones.ToList())
            {
                ProcessInteractionZone(targetZone);
            }

            SetLastSendTime();
        }

        protected void ProcessInteractionZone(ITradeZone zone)
        {
            if (zone == null) return;

            var targetInputItemKey = zone.InputItemKey;

            if (string.Equals(targetInputItemKey, $"{ECurrencyType.Money}"))
            {
                zone.ReceiveItemThroughTransfer(targetInputItemKey, SenderTransform.position);
            }
            else
            {
                if (Inventory.TryGetValue(targetInputItemKey, out var itemCount) && itemCount > 0)
                {
                    if (zone.CanReceiveItem())
                    {
                        zone.ReceiveItemThroughTransfer(targetInputItemKey, SenderTransform.position);
                        RemoveItem(targetInputItemKey);
                    }
                }
            }
        }

        public abstract void RegisterItemReceiver(ITradeZone zone, bool isConnected);
    }
}