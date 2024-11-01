using System;
using System.Collections.Generic;
using System.Linq;
using Modules.DataStructures;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.StatsModules.Abstract;
using Units.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract
{
    public interface IBuildingInventoryModule : IInventoryModule
    {
        public void RemoveItem(string inputItemKey);
        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);
        public int GetItemCount(string inputItemKey);
    }

    public abstract class BuildingInventoryModule : InventoryModule, IBuildingInventoryModule
    {
        public override int MaxInventorySize => _buildingStatsModule.MaxProductInventorySize;
        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        private readonly IBuildingStatsModule _buildingStatsModule;
        protected readonly PriorityQueue<ICreatureItemReceiver> _itemReceiverQueue = new();

        public string InputItemKey { get; }
        public string OutputItemKey { get; }

        protected BuildingInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            IBuildingStatsModule buildingStatsModule,
            string inputItemKey,
            string outputItemKey)
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

        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register)
        {
            switch (register)
            {
                case true:
                    if (!_itemReceiverQueue.Contains(itemReceiver))
                    {
                        _itemReceiverQueue.Enqueue(itemReceiver, (int)itemReceiver.CreatureType);
                        return true;
                    }
                    return false;
                case false:
                    _itemReceiverQueue.Remove(itemReceiver);
                    return true;
            }
        }

        public int GetItemCount(string key) => Inventory.GetValueOrDefault(key, 0);

        protected override void SendItem()
        {
            if (_itemReceiverQueue.Count == 0 || !IsReadyToSend()) return;
            
            if (!Inventory.TryGetValue(OutputItemKey, out var itemCount) || itemCount <= 0) return;

            if (_itemReceiverQueue.TryPeek(out ICreatureItemReceiver currentItemReceiver) && currentItemReceiver.CanReceiveItem())
            {
                IItem item = PopSpawnedItem();
                ItemFactory.ReturnItem(item);

                currentItemReceiver.ReceiveItemThroughTransfer(OutputItemKey, item.Transform.position);
                RemoveItem(OutputItemKey);
            }
            else
            {
                _itemReceiverQueue.Dequeue();
            }
        }
    }
}