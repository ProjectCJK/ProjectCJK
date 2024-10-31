using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts.Buildings.Abstract
{
    public abstract class BuildingDataSO : ScriptableObject
    {
        [Header("=== 건물 기본 세팅 ===")]
        [Header("건물 타입")] public EBuildingType BuildingType;
        [Header("건물 생산 속도")] public float BaseProductLeadTime;
        
        [Space(20), Header("=== 인벤토리 세팅 ===")]
        [Header("기본 생산품 인벤토리 크기")] public int BaseProductInventorySize;
    }
}