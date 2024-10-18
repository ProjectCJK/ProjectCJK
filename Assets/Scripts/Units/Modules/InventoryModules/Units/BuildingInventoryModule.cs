using System;
using System.Collections.Generic;
using Interfaces;
using Modules;
using Modules.DataStructures;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units
{
    public interface IBuildingInventoryModule : IInventoryModule
    {
        public void AddItem(Tuple<EMaterialType, EProductType> inputItemKey);
        public void RemoveItem(Tuple<EMaterialType, EProductType> inputItemKey);
        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver);
        public void UnRegisterItemReceiver(ICreatureItemReceiver itemReceiver);
        public int GetItemCount(Tuple<EMaterialType, EProductType> inputItemKey);
    }
    
    public abstract class BuildingInventoryModule : InventoryModule, IBuildingInventoryModule
    {
        public override int MaxInventorySize => _inventoryProperty.MaxInventorySize;
        public override Transform ReceiverTransform { get; }
        public List<Tuple<EMaterialType, EProductType>> InputItemKey { get; }
        public List<Tuple<EMaterialType, EProductType>> OutItemKey { get; }

        private PriorityQueue<ICreatureItemReceiver> _itemReceiverQueue = new ();
        
        private readonly IInventoryProperty _inventoryProperty;
        private readonly IItemController _itemController;
        private readonly Transform _senderTransform;
        
        private ICreatureItemReceiver currentItemReceiver;

        protected BuildingInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemController itemController,
            List<Tuple<EMaterialType, EProductType>> inputItemKey,
            List<Tuple<EMaterialType, EProductType>> outputItemKey)
        {
            _senderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            _inventoryProperty = inventoryProperty;
            _itemController = itemController;
            InputItemKey = inputItemKey;
            OutItemKey = outputItemKey;
        }
        
        public override void Initialize()
        {
            _itemReceiverQueue.Clear();
            Inventory.Clear();
        }
        
        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            if (_itemReceiverQueue.Contains(itemReceiver)) return;
            _itemReceiverQueue.Enqueue(itemReceiver, (int) itemReceiver.CreatureType);
            Debug.Log($"{itemReceiver}과 연결 성공, 현재 PriorityQueue Count : {_itemReceiverQueue.Count}개");
        }

        public void UnRegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
            PriorityQueue<ICreatureItemReceiver> queue = _itemReceiverQueue;
            var newQueue = new PriorityQueue<ICreatureItemReceiver>();

            while (queue.Count > 0)
            {
                if (queue.Dequeue() != itemReceiver)
                {
                    newQueue.Enqueue(currentItemReceiver, (int)currentItemReceiver.CreatureType);
                }
            }

            _itemReceiverQueue = newQueue;
                
            Debug.Log($"{itemReceiver}과 연결 종료, 현재 PriorityQueue Count : {_itemReceiverQueue.Count}개");
        }

        public int GetItemCount(Tuple<EMaterialType, EProductType> key)
        {
            return Inventory.GetValueOrDefault(key, 0);
        }

        public override void SendItem()
        {
            // 대기열에 등록된 유닛 체크
            if (_itemReceiverQueue.Count == 0) return;
         
            if (!IsReadyToSend()) return;

            foreach (var currentItemKey in OutItemKey)
            {
                if (Inventory.TryGetValue(currentItemKey, out var outputItem))
                {
                    // 대기열에서 우선순위가 가장 높은 Receiver 반환
                    if (!_itemReceiverQueue.TryPeek(out currentItemReceiver)) return;
            
                    // 반환된 Receiver가 현재 아이템 수령이 가능한 상태이고, OutputItem을 1개 이상 가지고 있다면
                    if (currentItemReceiver.CanReceiveItem() && outputItem > 0)
                    {
                        RemoveItem(currentItemKey);
                        currentItemReceiver.ReceiveItem(currentItemKey);
                        _itemController.TransferItem(currentItemKey, _senderTransform.position, currentItemReceiver.ReceiverTransform);
                    }
                    else
                    {
                        _itemReceiverQueue.Dequeue();
                    }     
                }
            }
            
            SetLastSendTime();
        }
    }
}