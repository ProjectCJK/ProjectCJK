using System.Collections.Generic;
using ScriptableObjects.Scripts.Structs;
using UnityEngine;

namespace ScriptableObjects.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Level Design Data SO", menuName = "Datas/Level Design Data")]
    public class LevelDesignDataSO : ScriptableObject
    {
        [Header("### 조이스틱 ###")] public SpawnData JoystickSpawnData;
        
        [Space(10)]
        [Header("### 레벨 ###")] public SpawnData LevelSpawnData;
    }
}