using System;
using System.Collections.Generic;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Abstract;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Abstract;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Buildings;
using Units.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units
{
    public interface IStandInventoryModule : IBuildingInventoryModule
    {
        
    }
    
    public class StandInventoryModule : BuildingInventoryModule, IStandInventoryModule
    {
        public StandInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IStandStatsModule standStatsModule,
            IItemFactory itemFactory,
            string inputItemKey,
            string outputItemKey)
            : base(senderTransform, receiverTransform, itemFactory, standStatsModule, inputItemKey, outputItemKey)
        {
        }
        
        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            AddItem(inputItemKey);
            PushSpawnedItem(item);
        }
    }
}