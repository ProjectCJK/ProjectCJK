using System;
using System.Collections.Generic;
using ScriptableObjects.Scripts.Creatures.Abstract;
using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Units
{
    [Serializable]
    public class CreatureSprite
    {
        public List<Sprite> Body;
        public List<Sprite> Bag;
        public List<Sprite> Head;
        public List<Sprite> BackHair;
        public List<Sprite> Scarf;
        public List<Sprite> Hat;
        public List<Sprite> Leg_Left;
        public List<Sprite> Leg_Right;
    }
    
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
