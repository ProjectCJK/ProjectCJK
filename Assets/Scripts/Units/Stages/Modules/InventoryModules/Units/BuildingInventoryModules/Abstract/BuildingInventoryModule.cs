using System;
using System.Collections.Generic;
using Managers;
using Modules.DataStructures;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract
{
    public interface IBuildingInventoryModule : IInventoryModule
    {
        public void RemoveItem(string inputItemKey);
        public bool RegisterItemReceiver(ICreatureItemReceiver itemReceiver, bool register);
        public int GetItemCount(string inputItemKey);
    }

    public abstract class BuildingInventoryModule : InventoryModule, IBuildingInventoryModule
    {
        public Action<int> OnUpdateStackedItem;
        
        private readonly BuildingStatsModule _buildingStatsModule;
        private readonly PriorityQueue<ICreatureItemReceiver> _itemReceiverQueue = new();

        protected BuildingInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IItemFactory itemFactory,
            BuildingStatsModule upgradableBuildingStatsModule,
            string inputItemKey,
            string outputItemKey)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            ItemFactory = itemFactory;
            _buildingStatsModule = upgradableBuildingStatsModule;
            InputItemKey = inputItemKey;
            OutputItemKey = outputItemKey;
        }

        public string InputItemKey { get; }
        public string OutputItemKey { get; }
        public override int MaxInventorySize => _buildingStatsModule.MaxInventorySize;
        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

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

        public int GetItemCount(string key)
        {
            return Inventory.GetValueOrDefault(key, 0);
        }

        protected override void SendItem()
        {
            if (_itemReceiverQueue.Count == 0 || !IsReadyToSend()) return;

            if (!Inventory.TryGetValue(OutputItemKey, out var itemCount) || itemCount <= 0) return;

            if (_itemReceiverQueue.TryPeek(out ICreatureItemReceiver currentItemReceiver))
            {
                if (string.Equals(OutputItemKey, $"{ECurrencyType.Money}"))
                {
                    var goldAmount = Mathf.Min(Inventory[$"{ECurrencyType.Money}"], DataManager.GoldSendingMaximum);
                    currentItemReceiver.ReceiveItemThroughTransfer(OutputItemKey, goldAmount, SenderTransform.position);
                    Inventory[$"{ECurrencyType.Money}"] -= goldAmount;
                }
                else if (currentItemReceiver.CanReceiveItem())
                {
                    OnUpdateStackedItem?.Invoke(CurrentInventorySize);
                    currentItemReceiver.ReceiveItemThroughTransfer(OutputItemKey, 1, SenderTransform.position);
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