using System;
using System.Collections.Generic;
using Buildings.Abstract;
using UnityEngine;

namespace Buildings.Controllers
{
    public class BuildingController
    {
        private readonly Dictionary<Tuple<int, int>, BaseBuilding> _buildings = new();
        
        public void Initialize(List<BaseBuilding> buildings)
        {
            foreach (BaseBuilding building in buildings)
            {
                Vector3 position = building.transform.position;
                var posX = (int) position.x;
                var posY = (int) position.y;

                _buildings.TryAdd(new Tuple<int, int>(posX, posY), building);
            }

            foreach (var building in _buildings)
            {
                Debug.Log($"{building.Value.gameObject.name} is located at ({building.Key.Item1}, {building.Key.Item2})");
            }
        }
    }
}