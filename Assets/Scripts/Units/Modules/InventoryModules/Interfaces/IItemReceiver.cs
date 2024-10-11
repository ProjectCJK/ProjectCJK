using System;
using Enums;

namespace Units.Modules.InventoryModules.Interfaces
{
    public interface IItemReceiver
    {
        public Tuple<EMaterialType, EItemType> InputItemKey { get; }
        
        public void ReceiveItem(Tuple<EMaterialType, EItemType> itemKey);
        public bool HasMatchingItem(Tuple<EMaterialType, EItemType> InventoryKey);
        public bool CanReceiveItem();
    }
}