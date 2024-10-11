using Units.Games.Creatures.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Scripts
{
    public class CreatureStatSO : UnitStatSo
    {
        [Header("### 유닛 기본 세팅 ###")]
        [Header("유닛 타입 (Player / NPC / 몬스터)")] public ECreatureType creatureType;
        [Header("기본 이동 속도")] public float BaseMovementSpeed;
        [Header("상호 작용 시작까지 대기 시간 (단위 : 초)")] public float BaseInteractionStandbySecond;
    }
}