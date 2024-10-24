using UnityEngine;

namespace ScriptableObjects.Scripts.HuntingZones
{
    [CreateAssetMenu(fileName = "New HuntingZone Data SO", menuName = "Datas/HuntingZone Data")]
    public class HuntingZoneDataSO : ScriptableObject
    {
        [Header("### HuntingZone 기본 세팅 ###")]
        [Header("아이템 드롭 시 최소 거리")] public float ItemDropMinimumRange;
        [Header("아이템 드롭 시 최소 거리")] public float ItemDropMaximumRange;
    }
}