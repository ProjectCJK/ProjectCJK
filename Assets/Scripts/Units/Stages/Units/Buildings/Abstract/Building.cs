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
        public string BuildingKey { get; }
        public string InputItemKey { get; }
        public string OutItemKey { get; }
    }

    public abstract class Building : MonoBehaviour, IBuilding
    {
        public abstract string BuildingKey { get; protected set; }
        public abstract string InputItemKey { get; protected set; }
        public abstract string OutItemKey { get; protected set; }

        public abstract void Initialize();
    }
}