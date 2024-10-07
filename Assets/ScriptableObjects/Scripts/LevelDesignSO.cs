using System;
using System.Collections.Generic;
using Units.Buildings.Abstract;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "New Level Design SO", menuName = "Level")]
    public class LevelDesignSO : ScriptableObject
    {
        public SpawnData JoystickSpawnData;
        public SpawnData LevelSpawnData;
        public SpawnData PlayerSpawnData;
        public List<SpawnData> BuildingSpawnData;
    }

    [Serializable]
    public struct SpawnData
    {
        public Vector3Int position;
        public GameObject prefab;
    }
}