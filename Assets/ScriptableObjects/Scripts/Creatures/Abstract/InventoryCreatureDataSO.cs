using System.Collections.Generic;
using ScriptableObjects.Scripts.Creatures.Units;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Abstract
{
    public abstract class InventoryCreatureDataSO : CreatureDataSO
    {
        [Space(20), Header("=== 인벤토리 세팅 ===")]
        [Header("기본 인벤토리 크기")] public int BaseInventorySize;
        [Header("상호 작용 시간 (단위 : 초)")] public float BaseInteractionStandbySecond;
        
        [Space(20), Header("=== Sprites 세팅 ===")]
        [Header("타입별 Sprite")] public List<CreatureSprite> CreatureSprites;
    }
}