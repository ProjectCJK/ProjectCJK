using System.Collections.Generic;
using Interfaces;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Units.Items.Enums;

namespace Managers
{
    public class VolatileDataManager : Singleton<VolatileDataManager>, IRegisterReference
    {
        public readonly Dictionary<string, int> ItemPrices = new();
        public float CurrentDeliveryManMoveSpeed;

        public int CurrentStageLevel { get; private set; }
        public Dictionary<EMaterialType, EStageMaterialType> MaterialMappings = new();

        public void RegisterReference() { }

        public int GetItemPrice(EItemType? item1, EMaterialType? item2)
        {
            if (item1.HasValue && item2.HasValue) return ItemPrices[$"{item1.Value}_{item2.Value}"];

            return 0;
        }
        
        public void SetCurrentStageLevel(int stageLevel)
        {
            CurrentStageLevel = stageLevel;
            
            QuestManager.Instance.InitializeQuestData();
        }
    }
}