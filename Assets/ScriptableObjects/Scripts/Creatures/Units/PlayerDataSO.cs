using ScriptableObjects.Scripts.Creatures.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Units
{
    [CreateAssetMenu(fileName = "So_Player", menuName = "Datas/Creatures/Player")]
    public class PlayerDataSO : InventoryCreatureDataSO
    {
        [Space(20), Header("=== 전투 세팅 ===")]
        public int BaseHealth;
        public int BaseDamage;
        public float BaseAttackDelay;
        public float ItemPickupRange;
        
        [Space(20), Header("=== 결제 세팅 ===")]
        public float BasePaymentDelay;
    }
}
