using System;
using Enums;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Games.Buildings.Abstract;
using Units.Games.Buildings.Enums;
using Units.Games.Buildings.Modules;
using Units.Modules.InventoryModules.Units;
using Units.Modules.StatsModules.Units;
using UnityEngine;

namespace Units.Games.Buildings.Units.InteractiveBuildings
{
    public interface IBlender : IRegisterReference<BuildingStatSO>
    {
        
    }
    
    public class Blender : Building, IBlender
    {
        public override EBuildingType BuildingType { get; protected set; }
        public override Tuple<EMaterialType, EItemType> InputItemKey { get; protected set; }
        public override Tuple<EMaterialType, EItemType> OutItemKey { get; protected set; }

        private IBuildingStatsModule _buildingStatsModule;
        private IBuildingInventoryModule _buildingInventoryModule;
        private IInteractionTrade _interactionTrade;

        public void RegisterReference(BuildingStatSO buildingStatSo)
        {
            BuildingType = buildingStatSo.BuildingType;
            InputItemKey = new Tuple<EMaterialType, EItemType>(buildingStatSo.MaterialType, buildingStatSo.InputItemType);
            OutItemKey = new Tuple<EMaterialType, EItemType>(buildingStatSo.MaterialType, buildingStatSo.OutputItemType);

            _buildingStatsModule = new BuildingStatsModule(buildingStatSo);
            
            _buildingInventoryModule = new BuildingInventoryModule();
            _buildingInventoryModule.RegisterReference(_buildingStatsModule, InputItemKey, OutItemKey);
            
            _interactionTrade = GetComponentInChildren<InteractionTrade>();
            _interactionTrade.RegisterReference(_buildingInventoryModule);
        }
        
        public override void Initialize() { }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log($"{EMaterialType.A} {EItemType.Product} 추가!");
                _buildingInventoryModule.ReceiveItem(new Tuple<EMaterialType, EItemType>(EMaterialType.A, EItemType.Product));
            }
            
            _buildingInventoryModule.SendItem();
        }
    }
}