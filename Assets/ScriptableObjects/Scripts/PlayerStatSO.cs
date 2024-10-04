using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Scripts
{
    [CreateAssetMenu(fileName = "New Player Stat SO", menuName = "Stats/Player Stat")]
    public class PlayerStatSO : ScriptableObject
    {
        public float BaseMovementSpeed;
        public int BaseInventorySize;
        public float BaseInteractionStandbySecond;
    }
}
