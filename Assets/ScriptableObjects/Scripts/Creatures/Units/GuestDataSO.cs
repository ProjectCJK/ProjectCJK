using System.Collections.Generic;
using ScriptableObjects.Scripts.Creatures.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Units
{
    [CreateAssetMenu(fileName = "So_Guest", menuName = "Datas/Creatures/NPC/Guest")]
    public class GuestDataSO : InventoryCreatureDataSO
    {
        [Space(20), Header("=== Sprites 세팅 ===")]
        [Header("타입별 Sprite")] public List<CreatureSprite> CreatureSprites;
    }
}