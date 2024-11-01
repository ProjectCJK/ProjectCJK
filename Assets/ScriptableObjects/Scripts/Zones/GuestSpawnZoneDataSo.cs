using UnityEngine;

namespace ScriptableObjects.Scripts.Zones
{
    [CreateAssetMenu(fileName = "So_GuestSpawnZone", menuName = "Datas/Levels/GuestSpawnZone")]
    public class GuestSpawnZoneDataSo : ScriptableObject
    {
        [Header("### GuestSpawnZone 기본 세팅 ###")]
        [Header("손님 생성 최소 주기 (단위 : 초)")] public float guestSpawnMinimumTime;
        [Header("손님 생성 최대 주기 (단위 : 초)")] public float guestSpawnMaximumTime;
    }
}