using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Scripts.Levels
{
    [CreateAssetMenu(fileName = "So_LevelPrefab", menuName = "Datas/Buildings/LevelPrefab")]
    public class LevelPrefabSO : ScriptableObject
    {
        [Header("=== Kitchen ===")]
        public List<GameObject> Kitchen;
        
        [Space(20), Header("=== Stand ===")]
        public List<GameObject> Stand;
        
        [Space(20), Header("=== ManagementDesk ===")]
        public GameObject ManagementDesk;
        
        [Space(20), Header("=== DeliveryLodging ===")]
        public GameObject DeliveryLodging;
        
        [Space(20), Header("=== WareHouse ===")]
        public GameObject WareHouse;
        
        [Space(20), Header("=== UnitSpawner ===")]
        public GameObject PlayerSpawner;
        public GameObject GuestSpawner;
        public GameObject ZombieSpawner;
        
        [Space(20), Header("=== Level ===")]
        public List<LevelGameObject> Levels;
    }

    [Serializable]
    public struct LevelGameObject
    {
        public GameObject Village;
        public List<GameObject> HuntingZones;
    }
}