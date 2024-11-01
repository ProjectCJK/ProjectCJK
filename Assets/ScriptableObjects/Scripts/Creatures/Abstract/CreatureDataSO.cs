using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Abstract
{
    public class CreatureDataSO : ScriptableObject
    {
        [Header("### 유닛 기본 세팅 ###")]
        [Header("프리팹")] public GameObject prefab;
        [Header("기본 이동 속도")] public float BaseMovementSpeed;
    }
}