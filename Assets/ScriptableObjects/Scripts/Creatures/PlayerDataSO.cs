using ScriptableObjects.Scripts.Creatures.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures
{
    [CreateAssetMenu(fileName = "New Player Data SO", menuName = "Datas/Player Data")]
    public class PlayerDataSO : InventoryCreatureDataSO
    {
        [Space(20), Header("### 전투 스탯 ###")]
        public int BaseHealth;
        public int BaseDamage;
        public float BaseAttackDelay;
    }
}
