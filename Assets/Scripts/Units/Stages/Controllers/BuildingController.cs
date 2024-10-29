using System;
using System.Collections.Generic;
using AmplifyShaderEditor;
using Interfaces;
using Modules.DesignPatterns.FSMs.Enums;
using Units.Modules.FactoryModules.Units;
using Units.Stages.Enums;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using EBuildingType = Units.Stages.Units.Buildings.Enums.EBuildingType;
using IInitializable = Interfaces.IInitializable;

namespace Units.Stages.Controllers
{
    public interface IBuildingController : IRegisterReference<IItemFactory>, IInitializable
    {
        public Dictionary<Tuple<EBuildingType, EMaterialType>, Building> Buildings { get; }
        public Dictionary<Building, EActiveStatus> BuildingActiveStatuses { get; }
    }

    public class BuildingController : MonoBehaviour, IBuildingController
    {
        public Dictionary<Tuple<EBuildingType, EMaterialType>, Building> Buildings { get; } = new();
        public Dictionary<Building, EActiveStatus> BuildingActiveStatuses { get; } = new();

        private List<EMaterialType> _materials;

        public void RegisterReference(IItemFactory itemFactory)
        {
            foreach (Transform buildingTransform in transform)
            {
                var building = buildingTransform.GetComponent<Building>();
                
                switch (building)
                {
                    case IKitchen kitchen:
                        kitchen.RegisterReference(itemFactory);
                        break;
                    case IStand stand:
                        stand.RegisterReference(itemFactory);
                        break;
                }

                var key = new Tuple<EBuildingType, EMaterialType>(building.BuildingType, building.BuildingMaterialType);

                Buildings.TryAdd(key, building);
                
                //TODO : 이후 건물 해금 기능 추가 시 이 부분 수정해야 함
                BuildingActiveStatuses.TryAdd(building, EActiveStatus.Active);
            }
        }

        public void Initialize()
        {
            foreach (KeyValuePair<Tuple<EBuildingType, EMaterialType>, Building> building in Buildings)
            {
                building.Value.Initialize();
            }
        }
        
        public Transform GetBuildingTransform(Tuple<EBuildingType, EMaterialType> key) => Buildings[key].transform;
    }
}