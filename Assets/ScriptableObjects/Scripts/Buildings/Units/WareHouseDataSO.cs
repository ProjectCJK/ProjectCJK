using ScriptableObjects.Scripts.Buildings.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Buildings.Units
{
    [CreateAssetMenu(fileName = "So_WareHouse", menuName = "Datas/Buildings/WareHouse")]
    public class WareHouseDataSO : BuildingDataSO
    {
        [Header("기본 재료 인벤토리 크기")] public int BaseMaterialInventorySize;
    }
}