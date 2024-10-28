using System;
using System.Collections.Generic;
using System.Linq;
using Modules.DataStructures;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.StatsModules.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract
{
    public interface IBuildingInventoryModule : IInventoryModule
    {
        void RemoveItem(Tuple<EMaterialType, EItemType> inputItemKey);
        void RegisterItemReceiver(ICreatureItemReceiver itemReceiver);
        void UnRegisterItemReceiver(ICreatureItemReceiver itemReceiver);
        int GetItemCount(Tuple<EMaterialType, EItemType> inputItemKey);
    }

    public abstract class BuildingInventoryModule : InventoryModule, IBuildingInventoryModule
    {
        public override int MaxInventorySize => _buildingStatsModule.MaxProductInventorySize;
        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        private readonly IBuildingStatsModule _buildingStatsModule;
        private readonly PriorityQueue<ICreatureItemReceiver> _itemReceiverQueue = new();

        public Tuple<EMaterialType, EItemType> InputItemKey { get; }
        public Tuple<EMaterialType, EItemType> OutputItemKey { get; }

        protected BuildingInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            IBuildingStatsModule buildingStatsModule,
            Tuple<EMaterialType, EItemType> inputItemKey,
            Tuple<EMaterialType, EItemType> outputItemKey)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            ItemFactory = itemFactory;
            _buildingStatsModule = buildingStatsModule;
            InputItemKey = inputItemKey;
            OutputItemKey = outputItemKey;
        }

        public override void Initialize()
        {
            _itemReceiverQueue.Clear();
            Inventory.Clear();
        }

        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            if (!_itemReceiverQueue.Contains(itemReceiver))
            {
                _itemReceiverQueue.Enqueue(itemReceiver, (int)itemReceiver.CreatureType);
            }
        }

        public void UnRegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            _itemReceiverQueue.Remove(itemReceiver);
        }

        public int GetItemCount(Tuple<EMaterialType, EItemType> key) => Inventory.GetValueOrDefault(key, 0);

        protected override void SendItem()
        {
            if (_itemReceiverQueue.Count == 0 || !IsReadyToSend()) return;
            
            if (!Inventory.TryGetValue(OutputItemKey, out var itemCount) || itemCount <= 0) return;

            if (_itemReceiverQueue.TryPeek(out ICreatureItemReceiver currentItemReceiver) && currentItemReceiver.CanReceiveItem())
            {
                IItem item = PopSpawnedItem();
                ItemFactory.ReturnItem(item);

                if (currentItemReceiver.ReceiveItem(OutputItemKey, item.Transform.position))
                {
                    RemoveItem(OutputItemKey);
                }
            }
            else
            {
                _itemReceiverQueue.Dequeue();
            }
        }
    }
}