using System;
using Enums;
using Interfaces;
using Modules;
using Modules.DataStructures;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Interfaces;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units
{
    public interface IBuildingInventoryModule : IInventoryModule, IRegisterReference<IInventoryProperty, Tuple<EMaterialType, EItemType>, Tuple<EMaterialType, EItemType>>, IBuildingItemReceiver
    {
        public void AddItem(Tuple<EMaterialType, EItemType> inputItemKey);
        public void RemoveItem(Tuple<EMaterialType, EItemType> inputItemKey);
    }
    
    public class BuildingInventoryModule : InventoryModule, IBuildingInventoryModule
    {
        public override int MaxInventorySize => _inventoryProperty.MaxInventorySize;

        private PriorityQueue<ICreatureItemReceiver> _itemReceiverQueue = new ();
        
        private IInventoryProperty _inventoryProperty;
        private ICreatureItemReceiver currentItemReceiver;
        
        private Tuple<EMaterialType, EItemType> _outputItemKey;

        public void RegisterReference(IInventoryProperty inventoryProperty, Tuple<EMaterialType, EItemType> inputItemKey, Tuple<EMaterialType, EItemType> outputItemKey)
        {
            _inventoryProperty = inventoryProperty;
            InputItemKey = inputItemKey;
            _outputItemKey = outputItemKey;
        }
        
        public override void Initialize()
        {
            _itemReceiverQueue.Clear();
            Inventory.Clear();
        }
        
        public void RegisterItemReceiver(ICreatureItemReceiver itemReceiver)
        {
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

        public override void SendItem()
        {
            if (_itemReceiverQueue.Count == 0) return;

            if (!Inventory.TryGetValue(_outputItemKey, out _))
            {
                Debug.Log($"(빌딩 -> 크리처) 아이템 전송 실행 조건 미충족 (!Inventory.TryGetValue(_outputItemKey, out var value) => {!Inventory.TryGetValue(_outputItemKey, out _)})");
                return;
            }
            
            if (!_itemReceiverQueue.TryPeek(out currentItemReceiver)) return;
            
            if (currentItemReceiver.CanReceiveItem() && Inventory[_outputItemKey] > 0)
            {
                currentItemReceiver.ReceiveItem(_outputItemKey);
                RemoveItem(_outputItemKey);
            }
            else
            {
                _itemReceiverQueue.Dequeue();
            }
        }
    }
}