using System;
using Managers;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Units
{
    public interface IPlayerInventoryModule : ICreatureInventoryModule
    {
    }

    public class PlayerInventoryModule : CreatureInventoryModule, IPlayerInventoryModule
    {
        public PlayerInventoryModule(
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

        public override ECreatureType CreatureType { get; }
        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        public override void RegisterItemReceiver(ITradeZone zone, bool isConnected)
        {
            if (isConnected)
                RegisterZone(zone as IPlayerTradeZone);
            else
                UnregisterZone(zone as IPlayerTradeZone);
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            if (Enum.TryParse(inputItemKey, out ECurrencyType currencyType))
            {
                switch (currencyType)
                {
                    case ECurrencyType.Money:
                        QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Get, inputItemKey, item.Count);
                        CurrencyManager.Instance.AddCurrency(ECurrencyType.Gold, item.Count);
                        break;
                }
            }
            else
            {
                QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Get, inputItemKey, 1);
                AddItem(inputItemKey, item.Count);
            }
            
            ItemFactory.ReturnItem(item);
        }

        private void RegisterZone(IPlayerTradeZone zone)
        {
            if (zone.CheckInputAccessorPlayer() && interactionTradeZones.Add(zone))
                Debug.Log($"{CreatureType} => {zone.BuildingKey} 도킹 완료 @~@");

            if (zone.CheckOutputAccessorPlayer() && zone.RegisterItemReceiver(this, true))
                Debug.Log($"{zone.BuildingKey} => {CreatureType} 도킹 완료 @~@");
        }

        private void UnregisterZone(IPlayerTradeZone zone)
        {
            if (zone.CheckInputAccessorPlayer() && interactionTradeZones.Remove(zone))
                Debug.Log($"{CreatureType} => {zone.BuildingKey} 도킹 해제 완료 @~@");

            if (zone.CheckOutputAccessorPlayer() && zone.RegisterItemReceiver(this, false))
                Debug.Log($"{zone.BuildingKey} => {CreatureType} 도킹 해제 완료 @~@");
        }
    }
}