using System;
using System.Collections.Generic;
using Interfaces;
using Units.Buildings.Abstract;
using UnityEngine;

namespace Units.Buildings.Controllers
{
    public class BuildingController : MonoBehaviour, IInitializable<List<BaseBuilding>>
    {
        private readonly Dictionary<int, BaseBuilding> _buildings = new();

        public void Initialize(List<BaseBuilding> buildings)
        {
            foreach (BaseBuilding building in buildings)
            {
                _buildings.TryAdd(building.gameObject.GetInstanceID(), building);
            }
        }
    }
}