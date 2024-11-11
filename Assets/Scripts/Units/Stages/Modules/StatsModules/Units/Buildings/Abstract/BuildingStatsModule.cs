using System;
using ScriptableObjects.Scripts.Buildings.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;
using Units.Stages.Units.Buildings.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Abstract
{
    public abstract class BuildingStatsModule : StatsModule
    {
        public Action<EBuildingAnimatorParameter> OnTriggerBuildingAnimation;
        
        public abstract string BuildingKey { get; protected set; }
        public EBuildingType BuildingType { get; private set; }
        
        public int MaxInventorySize => buildingDataSo.BaseInventorySize;

        private readonly BuildingDataSO buildingDataSo;
        
        protected BuildingStatsModule(BuildingDataSO buildingDataSo)
        {
            this.buildingDataSo = buildingDataSo;
            BuildingType = buildingDataSo.BuildingType;
        }
    }
}