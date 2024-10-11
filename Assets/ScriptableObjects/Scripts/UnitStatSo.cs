using UnityEngine;

namespace ScriptableObjects.Scripts
{
    public class UnitStatSo : ScriptableObject
    {
        [Header("### Spawner Settings ###")]
        [Header("SpawnPos / Prefab")] public SpawnData BaseSpawnData;
        
        [Space(10)]
        [Header("### 인벤토리 세팅 ###")]
        [Header("기본 인벤토리 크기")] public int BaseInventorySize;
    }
}