using System;
using Enums;
using Units.Games.Buildings.Enums;
using Units.Games.Buildings.Modules;
using Units.Games.Buildings.Units.Abstract;
using UnityEngine;

namespace Units.Games.Buildings.Units.InteractiveBuildings
{
    [RequireComponent(typeof(InteractionTrade))]
    public class Shelf : InteractiveBuilding
    {
        public override EBuildingType BuildingType { get; protected set; }
        public override Tuple<EMaterialType, EItemType> InputItemKey { get; protected set; }
        public override Tuple<EMaterialType, EItemType> OutItemKey { get; protected set; }
        
        public override void Initialize() { }
    }
}