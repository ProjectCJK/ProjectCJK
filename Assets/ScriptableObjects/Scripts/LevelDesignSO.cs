using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "New Level Design SO", menuName = "Level")]
    public class LevelDesignSO : ScriptableObject
    {
        public SpawnData JoystickSpawnData;
        public SpawnData LevelSpawnData;
    }
}