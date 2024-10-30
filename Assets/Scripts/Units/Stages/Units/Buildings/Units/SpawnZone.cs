using Units.Stages.Units.Creatures.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface ISpawnZone
    {
        public ECreatureType CreatureType { get; }
    }
    
    public class SpawnZone : MonoBehaviour
    {
        
    }
}