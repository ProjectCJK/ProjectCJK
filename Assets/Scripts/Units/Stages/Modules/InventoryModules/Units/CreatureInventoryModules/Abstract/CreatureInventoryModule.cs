using System.Collections.Generic;
using System.Linq;
using Managers;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract
{
    public interface ICreatureInventoryModule : IInventoryModule, ICreatureItemReceiver
    {
    }

    public abstract class CreatureInventoryModule : InventoryModule, ICreatureInventoryModule
    {
        protected readonly HashSet<ITradeZone> interactionTradeZones = new();

        private readonly IInventoryProperty InventoryProperty;

        protected CreatureInventoryModule(IInventoryProperty inventoryProperty)
        {
            InventoryProperty = inventoryProperty;
        }

        public abstract ECreatureType CreatureType { get; }
        public abstract override IItemFactory ItemFactory { get; }
        public abstract override Transform SenderTransform { get; }
        public abstract override Transform ReceiverTransform { get; }
        public override int MaxInventorySize => InventoryProperty.MaxProductInventorySize;

        public override void Initialize() { }

        public abstract void RegisterItemReceiver(ITradeZone zone, bool isConnected);

        protected override void SendItem()
        {
            if (interactionTradeZones.Count == 0) return;

            foreach (ITradeZone targetZone in interactionTradeZones.ToList()) ProcessInteractionZone(targetZone);
        }

        private void ProcessInteractionZone(ITradeZone zone)
        {
            if (zone == null) return;

            if (string.Equals(zone.InputItemKey, $"{ECurrencyType.Money}"))
            {
                if (!IsReadyToSend(true)) return;
                
                if (CurrencyManager.Instance.Gold > 0)
                {
                    var targetMoney = zone.CanReceiveMoney();

                    if (targetMoney > 0 && CurrencyManager.Instance.Gold > 0)
                    {
                        if (targetMoney > DataManager.GoldSendingMaximum) targetMoney = DataManager.GoldSendingMaximum;

                        targetMoney = targetMoney >= CurrencyManager.Instance.Gold
                            ? CurrencyManager.Instance.Gold
                            : targetMoney;
                        
                        zone.TempMoney += targetMoney;
                        
                        zone.ReceiveItemThroughTransfer(zone.InputItemKey, targetMoney, SenderTransform.position);

                        CurrencyManager.Instance.RemoveCurrency(ECurrencyType.Gold, targetMoney);
                        
                        SetLastSendTime();
                    }
                }
            }
            else if (string.Equals(zone.BuildingKey, $"{EBuildingType.WareHouse}"))
            {
                if (!IsReadyToSend(false)) return;
                
                if (Inventory.Count > 0)
                    if (zone.CanReceiveItem())
                        foreach (KeyValuePair<string, int> item in Inventory)
                        {
                            zone.ReceiveItemThroughTransfer(item.Key, 1, SenderTransform.position);
                            RemoveItem(item.Key);
                            SetLastSendTime();
                            break;
                        }
            }
            else
            {
                if (!IsReadyToSend(false)) return;
                
                if (Inventory.TryGetValue(zone.InputItemKey, out var itemCount) && itemCount > 0)
                    if (zone.CanReceiveItem())
                    {
                        zone.ReceiveItemThroughTransfer(zone.InputItemKey, 1, SenderTransform.position);
                        if (spawnedItemStack.Count > 0) ItemFactory.ReturnItem(PopSpawnedItem());
                        RemoveItem(zone.InputItemKey);
                        SetLastSendTime();
                    }
            }
        }
    }
}