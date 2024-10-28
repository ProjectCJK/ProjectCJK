using UnityEngine;

namespace ScriptableObjects.Scripts.HuntingZones
{
    [CreateAssetMenu(fileName = "So_HuntingZone", menuName = "Datas/Levels/HuntingZone")]
    public class HuntingZoneDataSO : ScriptableObject
    {
        [Header("### HuntingZone 기본 세팅 ###")]
        [Header("아이템 드롭 시 최소 거리")] public float ItemDropMinimumRange;
        [Header("아이템 드롭 시 최소 거리")] public float ItemDropMaximumRange;
    }
}