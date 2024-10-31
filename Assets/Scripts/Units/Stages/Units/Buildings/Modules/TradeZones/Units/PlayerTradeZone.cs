using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Modules.TradeZones.Units
{
    public interface IPlayerTradeZone : ITradeZone
    {
        public bool CheckInputAccessorPlayer();
        public bool CheckOutputAccessorPlayer();
        public bool CheckAccessorPlayer();
    }
    public class PlayerTradeZone : TradeZone, IPlayerTradeZone
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