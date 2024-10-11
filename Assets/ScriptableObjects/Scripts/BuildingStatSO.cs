using Enums;
using Units.Games.Buildings.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    
    [CreateAssetMenu(fileName = "New Building Stat SO", menuName = "Stats/Building Stat")]
    public class BuildingStatSO : ScriptableObject
    {
        [Header("### 건물 기본 세팅 ###")]
        [Header("건물 타입")] public EBuildingType BuildingType;
        [Header("재료 타입")] public EMaterialType MaterialType;
        [Header("입력 아이템 타입")] public EItemType InputItemType;
        [Header("출력 아이템 타입")] public EItemType OutputItemType;
        
        [Space(10)]
        [Header("### 인벤토리 세팅 ###")]
        [Header("기본 인벤토리 크기")] public int BaseInventorySize;
        
        [Space(10)]
        [Header("### Spawner Settings ###")]
        [Header("SpawnPos / Prefab")] public SpawnData BaseSpawnData;
    }
}