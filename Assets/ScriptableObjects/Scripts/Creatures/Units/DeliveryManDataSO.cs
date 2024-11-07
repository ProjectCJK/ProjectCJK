using System.Collections.Generic;
using ScriptableObjects.Scripts.Creatures.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Units
{
    [CreateAssetMenu(fileName = "So_DeliveryMan", menuName = "Datas/Creatures/NPC/DeliveryMan")]
    public class DeliveryManDataSO : InventoryCreatureDataSO
    {
        [Space(20), Header("=== Sprites 세팅 ===")]
        [Header("타입별 Sprite")] public List<CreatureSprite> CreatureSprites;
    }
}