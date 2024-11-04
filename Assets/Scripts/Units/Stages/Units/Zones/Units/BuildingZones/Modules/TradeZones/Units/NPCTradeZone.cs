using System.Collections.Generic;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Abstract;
using UnityEngine;

namespace Units.Stages.Units.Zones.Units.BuildingZones.Modules.TradeZones.Units
{
    public interface INPCTradeZone : ITradeZone
    {
        public bool CheckInputAccessorNPC(ENPCType npcType);
        public bool CheckOutputAccessorNPC(ENPCType npcType);

        public bool CheckAccessorNPC(ENPCType npcType);
    }
    
    public class NPCTradeZone : TradeZone, INPCTradeZone
    {
        [Header("=== Unit Accessor Settings ===")]
        [Header("--- NPC ---")]
        [SerializeField] private List<ENPCType> _inputAccessNPCTypes;
        [SerializeField] private List<ENPCType> _outAccessNPCTypes;
        
        public bool CheckInputAccessorNPC(ENPCType npcType)
        {
            return _inputAccessNPCTypes.Contains(npcType);
        }

        public bool CheckOutputAccessorNPC(ENPCType npcType)
        {
            return _outAccessNPCTypes.Contains(npcType);
        }
        
        public bool CheckAccessorNPC(ENPCType npcType)
        {
            return CheckInputAccessorNPC(npcType) || CheckOutputAccessorNPC(npcType);
        }
    }
}