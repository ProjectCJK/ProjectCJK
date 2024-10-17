using System;
using Units.Stages.Items.Enums;

namespace Units.Modules.InventoryModules.Interfaces
{
    public interface IBlenderItemReceiver : IBuildingItemReceiver
    {
        public Tuple<EMaterialType, EProductType> InputItemKey { get; }
    }
}