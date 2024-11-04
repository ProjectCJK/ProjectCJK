using Units.Stages.Units.Zones.Units.BuildingZones.Modules.UnlockZones.Abstract;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Modules.UnlockZones.Units
{
    public interface IPlayerUnlockZone : IUnlockZone
    {
        public bool CheckInputAccessorPlayer();
        public bool CheckOutputAccessorPlayer();
        public bool CheckAccessorPlayer();
    }
    
    public class PlayerUnlockZone : UnlockZone
    {
        [Header("=== Unit Accessor Settings ===")]
        [Header("--- Player ---")]
        [SerializeField] private bool _inputAccessPlayer;
        [SerializeField] private bool _outputAccessPlayer;
        
        public bool CheckInputAccessorPlayer()
        {
            return _inputAccessPlayer;
        }
        
        public bool CheckOutputAccessorPlayer()
        {
            return _outputAccessPlayer;
        }
        
        public bool CheckAccessorPlayer()
        {
            return CheckInputAccessorPlayer() || CheckOutputAccessorPlayer();
        }
    }
}