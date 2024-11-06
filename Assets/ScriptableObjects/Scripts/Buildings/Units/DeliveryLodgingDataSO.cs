using ScriptableObjects.Scripts.Buildings.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Buildings.Units
{
    [CreateAssetMenu(fileName = "So_DeliveryLodging", menuName = "Datas/Buildings/DeliveryLodging")]
    public class DeliveryLodgingDataSO : BuildingDataSO
    {
        [Header("=== 배달부 기본값 세팅 === ")]
        public int BaseMaxDeliveryManCount;
    }
}