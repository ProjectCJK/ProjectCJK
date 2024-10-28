using ScriptableObjects.Scripts.Buildings.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Buildings.Units
{
    [CreateAssetMenu(fileName = "So_Kitchen", menuName = "Datas/Buildings/Kitchen")]
    public class KitchenDataSO : BuildingDataSO
    {
        [Header("기본 재료 인벤토리 크기")] public int BaseMaterialInventorySize;
    }
}