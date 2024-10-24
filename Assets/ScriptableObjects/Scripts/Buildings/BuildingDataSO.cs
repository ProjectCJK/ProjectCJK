using Units.Stages.Units.Buildings.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts.Buildings
{
    [CreateAssetMenu(fileName = "New Building Data SO", menuName = "Datas/Building Data")]
    public class BuildingDataSO : ScriptableObject
    {
        [Header("=== 건물 기본 세팅 ===")]
        [Header("건물 타입")] public EBuildingType BuildingType;
        [Header("건물 생산 속도")] public float productLeadTime;
        
        [Space(20), Header("=== 인벤토리 세팅 ===")]
        [Header("기본 인벤토리 크기")] public int BaseInventorySize;
    }
}