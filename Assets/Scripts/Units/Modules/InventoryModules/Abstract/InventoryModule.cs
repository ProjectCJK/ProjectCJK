using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Interfaces;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEditor.UIElements;
using UnityEngine;

namespace Units.Modules.InventoryModules.Abstract
{
    public interface IInventoryModule : IInitializable, IItemReceiver
    {
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

        private bool isItemReceiving;

        public abstract void Initialize();
        public void Update() => TrySendItem();

        protected abstract void SendItem();
        
        protected abstract void OnItemReceived(string inputItemKey, IItem item);
        
        public void ReceiveItem(string inputItemKey, Vector3 currentSenderPosition)
        {
            isItemReceiving = true;
            
            IItem item = ItemFactory.GetItem(inputItemKey, currentSenderPosition);

            // 아이템을 전송하고, 이후의 행동을 콜백으로 처리
            item.Transfer(currentSenderPosition, ReceiverTransform, () =>
            {
                OnItemReceived(inputItemKey, item);
                isItemReceiving = false;
            });
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
        protected void AddItem(string itemKey)
        {
            if (!Inventory.TryAdd(itemKey, 1))
            {
                Inventory[itemKey]++;
            }
            OnInventoryCountChanged?.Invoke();
        }

        public bool HasMatchingItem(string inventoryKey) => Inventory.ContainsKey(inventoryKey);

        public bool CanReceiveItem() => CurrentInventorySize + (isItemReceiving ? 2 : 1) <= MaxInventorySize;
        
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
        protected IItem PopSpawnedItem() => _spawnedItemStack.Pop();
    }
}