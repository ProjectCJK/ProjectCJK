using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Interfaces;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Abstract
{
    public interface IInventoryModule : IInitializable, IItemReceiver
    {
        public void ReceiveItemNoThroughTransfer(string inputItemKey, int count);
        public event Action OnInventoryCountChanged;
        public IItemFactory ItemFactory { get; }
        public int MaxInventorySize { get; }
        public int CurrentInventorySize { get; }
        public void Update();
    }

    public interface IInventoryProperty
    {
        public int MaxProductInventorySize { get; }
    }

    public abstract class InventoryModule : IInventoryModule
    {
        public event Action OnInventoryCountChanged;

        public abstract IItemFactory ItemFactory { get; }
        public abstract int MaxInventorySize { get; }
        public abstract Transform SenderTransform { get; }
        public abstract Transform ReceiverTransform { get; }

        public int CurrentInventorySize => Inventory.Values.Sum();
        
        protected readonly Dictionary<string, int> Inventory = new();
        protected readonly Stack<IItem> _spawnedItemStack = new();
        
        private const float SendItemInterval = 0.2f;
        private float _lastSendTime;

        public bool IsItemReceiving;

        public abstract void Initialize();
        public void Update() => TrySendItem();

        protected abstract void SendItem();
        
        protected abstract void OnItemReceived(string inputItemKey, IItem item);

        public void ReceiveItemThroughTransfer(string inputItemKey, int count, Vector3 currentSenderPosition)
        {
            IsItemReceiving = true;
            
            IItem item = ItemFactory.GetItem(inputItemKey, count, currentSenderPosition);

            // 아이템을 전송하고, 이후의 행동을 콜백으로 처리
            item.Transfer(currentSenderPosition, ReceiverTransform, () =>
            {
                OnItemReceived(inputItemKey, item);
                IsItemReceiving = false;
            });
        }

        public void ReceiveItemNoThroughTransfer(string inputItemKey, int count)
        {
            IsItemReceiving = true;
            
            IItem item = ItemFactory.GetItem(inputItemKey, count, ReceiverTransform.position);
            OnItemReceived(inputItemKey, item);
            IsItemReceiving = false;
        }

        /// <summary>
        /// 아이템을 제거하는 메서드
        /// </summary>
        public void RemoveItem(string itemKey)
        {
            if (!Inventory.ContainsKey(itemKey)) return;

            Inventory[itemKey]--;
            if (Inventory[itemKey] <= 0)
            {
                Inventory.Remove(itemKey);
            }
            OnInventoryCountChanged?.Invoke();
        }

        /// <summary>
        /// 아이템을 추가하는 메서드
        /// </summary>
        protected void AddItem(string itemKey, int count)
        {
            if (!Inventory.TryAdd(itemKey, count))
            {
                Inventory[itemKey] += count;
            }
            
            OnInventoryCountChanged?.Invoke();
        }

        public bool HasMatchingItem(string inventoryKey) => Inventory.ContainsKey(inventoryKey);
        public bool CanReceiveItem() => CurrentInventorySize + (IsItemReceiving ? 2 : 1) <= MaxInventorySize;
        
        protected bool IsReadyToSend() => Time.time >= _lastSendTime + SendItemInterval;
        protected void SetLastSendTime() => _lastSendTime = Time.time;

        private void TrySendItem()
        {
            if (IsReadyToSend())
            {
                SendItem();
                SetLastSendTime();
            }
        }

        protected void PushSpawnedItem(Transform receiveTransform, IItem item)
        {
            item.Transform.SetParent(receiveTransform);
            _spawnedItemStack.Push(item);   
        }

        protected IItem PopSpawnedItem()
        {
            return _spawnedItemStack.Count > 0 ? _spawnedItemStack.Pop() : null;
        }
    }
}