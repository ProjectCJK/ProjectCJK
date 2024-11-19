using System.Collections.Generic;
using Interfaces;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Enums;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.HuntingZones;
using Units.Stages.Units.Items.Enums;

namespace Managers
{
    public class VolatileDataManager : Singleton<VolatileDataManager>, IRegisterReference
    {
        public Player Player;

        public int CurrentStageLevel { get; private set; }

        public DeliveryLodgingStatsModule DeliveryLodgingStatsModule;
        public ManagementDeskStatsModule ManagementDeskStatsModule;
        public WareHouseStatsModule WareHouseStatsModule;
        public Dictionary<EMaterialType, EStageMaterialType> MaterialMappings = new();

        public readonly Dictionary<string, int> ItemPrices = new();
        public readonly Dictionary<EMaterialType, KitchenStatsModule> KitchenStatsModule = new();
        public readonly Dictionary<EMaterialType, StandStatsModule> StandStatsModule = new();
        public readonly Dictionary<HuntingZone, EActiveStatus> HuntingZoneActiveStatuses = new();
        public readonly HashSet<EMaterialType> CurrentActiveMaterials = new();
        public readonly Dictionary<ECostumeOptionType, float> CostumeEquipmentOption = new();
        public readonly Dictionary<ECostumeType, CostumeItemData> EquippedCostumes = new();

        public void RegisterReference()
        {
        }

        public int GetItemPrice(EItemType? item1, EMaterialType? item2)
        {
            if (item1.HasValue && item2.HasValue) return ItemPrices[$"{item1.Value}_{item2.Value}"];

            return 0;
        }

        public void SetCurrentStageLevel(int stageLevel)
        {
            CurrentStageLevel = stageLevel;
        }
    }
}