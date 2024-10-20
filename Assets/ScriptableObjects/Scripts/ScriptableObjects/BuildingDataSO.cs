using Units.Stages.Buildings.Enums;
using Units.Stages.Items.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts.ScriptableObjects
{
    
    [CreateAssetMenu(fileName = "New Building Data SO", menuName = "Datas/Building Data")]
    public class BuildingDataSO : UnitDataSo
    {
        [Space(10)]
        [Header("### 건물 기본 세팅 ###")]
        [Header("건물 타입")] public EBuildingType BuildingType;
        [Header("재료 타입")] public EMaterialType MaterialType;
        [Header("입력 아이템 타입")] public EProductType inputProductType;
        [Header("출력 아이템 타입")] public EProductType outputProductType;
        [Header("건물 생산 속도")] public float productLeadTime;
    }
}