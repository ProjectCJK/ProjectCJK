using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects.Scripts;
using Units.Games.Buildings.Enums;
using Units.Games.Creatures.Abstract;
using Units.Games.Creatures.Units;
using Units.Games.Items.Enums;
using Units.Modules.InventoryModules.Units;
using UnityEngine;

namespace Units.Games.Buildings.Abstract
{
    public interface IBuilding : IInitializable
    {
        public abstract EBuildingType BuildingType { get; }
        public abstract Tuple<EMaterialType, EProductType> InputItemKey { get; }
        public abstract Tuple<EMaterialType, EProductType> OutItemKey { get; }
    }

    public abstract class Building : MonoBehaviour, IBuilding
    {
        public abstract EBuildingType BuildingType { get; protected set; }
        public abstract Tuple<EMaterialType, EProductType> InputItemKey { get; protected set; }
        public abstract Tuple<EMaterialType, EProductType> OutItemKey { get; protected set; }

        public abstract void Initialize();
    }
}