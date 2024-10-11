using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "New Level Design SO", menuName = "Level")]
    public class LevelDesignSO : ScriptableObject
    {
        [Header("### 조이스틱 ###")]
        public SpawnData JoystickSpawnData;
        
        [Space(10)]
        [Header("### 레벨 ###")]
        public SpawnData LevelSpawnData;

        [Space(10)]
        [Header("### 재료 ###")]
        public List<EMaterialType> MaterialTypes;
        
        [Space(10)]
        [Header("### 아이템 ###")]
        public List<EItemType> ItemTypes;
    }
}