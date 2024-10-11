using Enums;
using Units.Games.Buildings.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts
{
    
    [CreateAssetMenu(fileName = "New Building Stat SO", menuName = "Stats/Building Stat")]
    public class BuildingStatSO : UnitStatSo
    {
        [Header("### 건물 기본 세팅 ###")]
        [Header("건물 타입")] public EBuildingType BuildingType;
        [Header("재료 타입")] public EMaterialType MaterialType;
        [Header("입력 아이템 타입")] public EItemType InputItemType;
        [Header("출력 아이템 타입")] public EItemType OutputItemType;
        [Header("건물 생산 속도")] public float productLeadTime;
    }
}