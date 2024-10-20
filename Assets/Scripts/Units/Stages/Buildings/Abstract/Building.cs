using System;
using System.Collections.Generic;
using Interfaces;
using ScriptableObjects.Scripts.ScriptableObjects;
using Units.Stages.Buildings.Enums;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace Units.Stages.Buildings.Abstract
{
    public interface IBuilding : IInitializable
    {
        public EBuildingType BuildingType { get; }
        public List<Tuple<EMaterialType, EItemType>> InputItemKey { get; }
        public List<Tuple<EMaterialType, EItemType>> OutItemKey { get; }
    }

    public abstract class Building : MonoBehaviour, IBuilding
    {
        public abstract EBuildingType BuildingType { get; }
        public abstract List<Tuple<EMaterialType, EItemType>> InputItemKey { get; protected set; }
        public abstract List<Tuple<EMaterialType, EItemType>> OutItemKey { get; protected set; }

        public abstract void Initialize();
    }
}