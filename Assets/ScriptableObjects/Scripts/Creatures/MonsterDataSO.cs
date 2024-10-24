using System;
using System.Collections.Generic;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures
{
    [Serializable]
    public struct MonsterSprite
    {
        [Header("--- 재료 타입 ---")] public EMaterialType MaterialType;
        [Header("--- 이미지 타입 ---")] public Sprite Sprite;
    }
    
    [CreateAssetMenu(fileName = "Monster Data SO", menuName = "Datas/Monster Data")]
    public class MonsterDataSO : CreatureDataSO
    {
        [Space(20), Header("### Monster 기본 세팅 ###")]
        [Header("타입별 Sprite")] public List<MonsterSprite> MonsterSprites;

        [Space(20), Header("### 전투 스탯 ###")]
        [Header("기본 체력")] public int BaseHealth;
        [Header("기본 공격력")] public int BaseDamage;
        [Header("기본 공격 속도")] public float BaseAttackSpeed;

        [Space(20), Header("### 아이템 스탯 ###")]
        [Header("아이템 드롭 최소 거리")] public float MinimumRange;
        [Header("아이템 드롭 최대 거리")] public float MaxRange;
    }
}