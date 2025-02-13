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
        
        public int TempMoney { get; set; }
        public bool IsItemReceiving { get; set; }
        public IItemFactory ItemFactory { get; }
        public int MaxInventorySize { get; }
        public int CurrentInventorySize { get; }
        public void ReceiveItemNoThroughTransfer(string inputItemKey, int count);
        public event Action OnInventoryCountChanged;
        public void Update();
    }

    public interface IInventoryProperty
    {
        public int MaxProductInventorySize { get; }
    }

    public abstract class InventoryModule : IInventoryModule
    {
        private const float SendItemInterval = 0.1f;
        private const float SendMoneyInterval = 0.015f;

        public abstract Dictionary<string, int> Inventory { get; set; }

        protected readonly Stack<IItem> spawnedItemStack = new();
        private bool _isItemReceiving;
        private float _lastSendTime;
        public event Action OnInventoryCountChanged;

        public abstract IItemFactory ItemFactory { get; }
        public abstract int MaxInventorySize { get; }
        public abstract Transform SenderTransform { get; }
        public abstract Transform ReceiverTransform { get; }

        public int CurrentInventorySize => Inventory.Values.Sum();

        public int TempMoney { get; set; }
        public bool IsItemReceiving { get; set; }

        public abstract void Initialize();

        public void Update()
        {
            SendItem();
        }

        public void ReceiveItemThroughTransfer(string inputItemKey, int count, Vector3 currentSenderPosition)
        {
            IsItemReceiving = true;

            IItem item = ItemFactory.GetItem(inputItemKey, count, currentSenderPosition, true);

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

            IItem item = ItemFactory.GetItem(inputItemKey, count, ReceiverTransform.position, true);
            OnItemReceived(inputItemKey, item);
            IsItemReceiving = false;
        }

        public bool HasMatchingItem(string inventoryKey)
        {
            return Inventory.ContainsKey(inventoryKey);
        }

        public bool CanReceiveItem()
        {
            return CurrentInventorySize + (IsItemReceiving ? 2 : 1) <= MaxInventorySize;
        }

        protected abstract void SendItem();

        protected abstract void OnItemReceived(string inputItemKey, IItem item);

        /// <summary>
        ///     아이템을 제거하는 메서드
        /// </summary>
        public virtual void RemoveItem(string itemKey)
        {
            if (!Inventory.ContainsKey(itemKey)) return;

            Inventory[itemKey]--;
            if (Inventory[itemKey] <= 0) Inventory.Remove(itemKey);
            OnInventoryCountChanged?.Invoke();
        }

        /// <summary>
        ///     아이템을 추가하는 메서드
        /// </summary>
        protected void AddItem(string itemKey, int count)
        {
            if (!Inventory.TryAdd(itemKey, count)) Inventory[itemKey] += count;

            OnInventoryCountChanged?.Invoke();
        }

        protected bool IsReadyToSend(bool isTargetTypeMoney)
        {
            if (isTargetTypeMoney)
            {
                return Time.time >= _lastSendTime + SendMoneyInterval;
            }
            else
            {
                return Time.time >= _lastSendTime + SendItemInterval;   
            }
        }

        protected void SetLastSendTime()
        {
            _lastSendTime = Time.time;
        }

        protected void PushSpawnedItem(Transform receiveTransform, IItem item, bool isVisible)
        {
            item.Transform.gameObject.SetActive(isVisible);
            item.Transform.rotation = Quaternion.identity;
            item.Transform.SetParent(receiveTransform);
            spawnedItemStack.Push(item);
        }

        protected IItem PopSpawnedItem()
        {
            return spawnedItemStack.Count > 0 ? spawnedItemStack.Pop() : null;
        }
    }
}