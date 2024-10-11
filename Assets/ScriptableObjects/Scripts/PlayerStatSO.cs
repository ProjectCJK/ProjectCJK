using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "New Player Stat SO", menuName = "Stats/Player Stat")]
    public class PlayerStatSO : CreatureStatSO
    {
        [Header("### 유닛 기본 세팅 ###")]
        [Header("기본 이동 속도")] public float BaseMovementSpeed;
        [Header("상호 작용 시작까지 대기 시간 (단위 : 초)")] public float BaseInteractionStandbySecond;
        
        [Space(10)]
        [Header("### 인벤토리 세팅 ###")]
        [Header("기본 인벤토리 크기")] public int BaseInventorySize;

        [Space(10)]
        [Header("### Spawner Settings ###")]
        [Header("SpawnPos / Prefab")] public SpawnData BaseSpawnData;
    }
}
