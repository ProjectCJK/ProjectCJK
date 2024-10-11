using UnityEngine;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "New Item Stat SO", menuName = "Stats/Item Stat")]
    public class ItemStatSO : ScriptableObject
    {
        public Sprite AMaterial;
        public Sprite AProduct;
    }
}