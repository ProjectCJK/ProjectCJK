using ScriptableObjects.Scripts.Creatures.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Units
{
    [CreateAssetMenu(fileName = "So_Hunter", menuName = "Datas/Creatures/NPC/Hunter")]
    public class HunterDataSO : InventoryCreatureDataSO
    {
        [Space(20), Header("=== 전투 세팅 ===")]
        public int BaseHealth;
        public int BaseDamage;
        public float BaseAttackDelay;
        public float ItemPickupRange;
    }
}