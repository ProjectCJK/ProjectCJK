using System;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Modules.InventoryModules.Interfaces
{
    public interface IItemReceiver
    {
        public Transform ReceiverTransform { get; }
        public void ReceiveItem(Tuple<EMaterialType, EItemType> itemKey);
        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey);
        public bool CanReceiveItem();
    }
}