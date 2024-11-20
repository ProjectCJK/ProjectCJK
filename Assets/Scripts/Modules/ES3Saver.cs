using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Enums;

namespace Modules
{
    [Serializable]
    public class ES3Saver
    {
        /// <summary>
        /// GameSettings
        /// </summary>
        public bool InitializeES3Saver;

        /// <summary>
        /// TutorialSettings
        /// </summary>
        public bool TutorialClear;

        /// <summary>
        /// StageSettings
        /// </summary>
        public int CurrentStageLevel;
        
        /// <summary>
        /// CurrencySettings
        /// </summary>
        public int Gold;
        public int Diamond;
        public int RedGem;
        
        /// <summary>
        /// InventorySettings
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> CreatureItems = new();
        public List<CostumeDataSerializable> CurrentCostumes = new();
        
        /// <summary>
        /// BuildingSettings
        /// </summary>
        
        public Dictionary<string, Dictionary<string, int>> BuildingInputItems = new();
        public Dictionary<string, Dictionary<string, int>> BuildingOutputItems = new();
        public Dictionary<string, int> CurrentBuildingLevel = new();
        public Dictionary<string, int> CurrentBuildingOption1Level = new();
        public Dictionary<string, int> CurrentBuildingOption2Level = new();
        
        /// <summary>
        /// UnlockSettings
        /// </summary>
        public int ActiveStatusSettingIndex; 
        public Dictionary<string, EActiveStatus> BuildingActiveStatuses = new();
        public Dictionary<string, EActiveStatus> HuntingZoneActiveStatuses = new();
        public Dictionary<string, int> RequiredMoneyForBuildingActive = new();
        public Dictionary<string, int> RequiredMoneyForHuntingZoneActive = new();
        
        
        /// <summary>
        /// QuestSettings
        /// </summary>
        public int CurrentQuestSubIndex; // 현재 진행 중인 퀘스트 인덱스
        public Dictionary<int, bool> ClearedQuestStatuses = new(); // 퀘스트 클리어 상태
        public Dictionary<int, int> QuestProgress = new(); // 퀘스트 진행 상태 (현재 목표 값)

        public void ResetStageData()
        {
            BuildingActiveStatuses = new Dictionary<string, EActiveStatus>();
            HuntingZoneActiveStatuses = new Dictionary<string, EActiveStatus>();
            BuildingInputItems = new Dictionary<string, Dictionary<string, int>>();
            BuildingOutputItems = new Dictionary<string, Dictionary<string, int>>();
            CurrentBuildingLevel = new Dictionary<string, int>();
            CurrentBuildingOption1Level = new Dictionary<string, int>();
            CurrentBuildingOption2Level = new Dictionary<string, int>();
            RequiredMoneyForBuildingActive = new Dictionary<string, int>();
            RequiredMoneyForHuntingZoneActive = new Dictionary<string, int>();
            ActiveStatusSettingIndex = 0;
            CurrentQuestSubIndex = 0;
            ClearedQuestStatuses = new Dictionary<int, bool>();
            QuestProgress = new Dictionary<int, int>();
        }
    }
}