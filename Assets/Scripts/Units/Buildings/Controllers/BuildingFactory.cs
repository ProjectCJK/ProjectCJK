using ScriptableObjects.Scripts;
using Units.Buildings.Abstract;
using UnityEngine;

namespace Units.Buildings.Controllers
{
    public interface IBuildingFactory
    {
        BaseBuilding CreateBuilding(SpawnData buildingSpawnData);
    }

    public class BuildingFactory : IBuildingFactory
    {
        public BaseBuilding CreateBuilding(SpawnData buildingSpawnData)
        {
            return Object.Instantiate(buildingSpawnData.prefab, buildingSpawnData.position, Quaternion.identity).GetComponent<BaseBuilding>();
        }
    }
}