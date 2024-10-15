using System;
using Units.Games.Items.Enums;
using UnityEngine;

namespace Units.Modules.InventoryModules.Interfaces
{
    public interface IItemReceiver
    {
        public Transform ReceiverTransform { get; }
        public Tuple<EMaterialType, EProductType> InputItemKey { get; }
        public void ReceiveItem(Tuple<EMaterialType, EProductType> itemKey);
        public bool HasMatchingItem(Tuple<EMaterialType, EProductType> InventoryKey);
        public bool CanReceiveItem();
    }
}