using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Enums;

namespace Modules
{
    [Serializable]
    public class ES3Saver
    {
        public int Gold;
        public int Diamond;
        public int RedGem;
        public Dictionary<string, Dictionary<string, int>> CreatureItems = new();

        public List<CostumeDataSerializable> CurrentCostumes = new();
        
        // BuildingInfo
        public Dictionary<string, EActiveStatus> BuildingActiveStatuses = new();
        public Dictionary<string, EActiveStatus> HuntingZoneActiveStatuses = new();
        public Dictionary<string, Dictionary<string, int>> BuildingInputItems = new();
        public Dictionary<string, Dictionary<string, int>> BuildingOutputItems = new();
        public Dictionary<string, int> CurrentBuildingLevel = new();
        public Dictionary<string, int> CurrentBuildingOption1Level = new();
        public Dictionary<string, int> CurrentBuildingOption2Level = new();
        public Dictionary<string, int> RequiredMoneyForBuildingActive = new();
        public Dictionary<string, int> RequiredMoneyForHuntingZoneActive = new();
        public int ActiveStatusSettingIndex = 0; 
        
        // Quest Data
        public int CurrentQuestSubIndex; // 현재 진행 중인 퀘스트 인덱스
        public Dictionary<int, bool> ClearedQuestStatuses = new(); // 퀘스트 클리어 상태
        public Dictionary<int, int> QuestProgress = new(); // 퀘스트 진행 상태 (현재 목표 값)
    }
}