using ScriptableObjects.Scripts.Creatures.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Units
{
    [CreateAssetMenu(fileName = "So_Player", menuName = "Datas/Creatures/Player")]
    public class PlayerDataSO : InventoryCreatureDataSO
    {
        [Space(20), Header("### 전투 스탯 ###")]
        public int BaseHealth;
        public int BaseDamage;
        public float BaseAttackDelay;
        public float ItemPickupRange;
    }
}
