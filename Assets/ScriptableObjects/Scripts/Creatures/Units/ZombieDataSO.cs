using UnityEngine;

namespace ScriptableObjects.Scripts.Creatures.Units
{
    [CreateAssetMenu(fileName = "So_Zombie", menuName = "Datas/Creatures/Zombie")]
    public class ZombieDataSO : ScriptableObject
    {
        public GameObject ZombiePrefab;
        
        [Header("=== Sprites 세팅 ===")]
        [Header("타입별 Sprite")] public CreatureSprite CreatureSprites;
    }
}