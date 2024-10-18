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
        public List<Tuple<EMaterialType, EProductType>> InputItemKey { get; }
        public List<Tuple<EMaterialType, EProductType>> OutItemKey { get; }
    }

    public abstract class Building : MonoBehaviour, IBuilding
    {
        [SerializeField] protected BuildingDataSO buildingDataSo;

        public EBuildingType BuildingType => buildingDataSo.BuildingType;
        public abstract List<Tuple<EMaterialType, EProductType>> InputItemKey { get; protected set; }
        public abstract List<Tuple<EMaterialType, EProductType>> OutItemKey { get; protected set; }

        public abstract void Initialize();
    }
}