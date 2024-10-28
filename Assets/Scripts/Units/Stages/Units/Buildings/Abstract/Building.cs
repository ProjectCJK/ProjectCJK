using System;
using System.Collections.Generic;
using Interfaces;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Abstract
{
    public interface IBuilding : IInitializable
    {
        public EBuildingType BuildingType { get; }
        public Tuple<EMaterialType, EItemType> InputItemKey { get; }
        public Tuple<EMaterialType, EItemType> OutItemKey { get; }
    }

    public abstract class Building : MonoBehaviour, IBuilding
    {
        public abstract EBuildingType BuildingType { get; protected set; }
        public abstract Tuple<EMaterialType, EItemType> InputItemKey { get; protected set; }
        public abstract Tuple<EMaterialType, EItemType> OutItemKey { get; protected set; }

        public abstract void Initialize();
    }
}