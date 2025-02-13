using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Enums;

namespace Modules
{
    [Serializable]
    public class ES3Saver
    {
        public bool UpgradeZoneTrigger;
        
        public bool first_CurrentStage;
        public bool second_CurrentStage;
        
        public bool first_open;
        public bool first_loading;
        public bool first_ingame;
        public bool first_camera_complete;
        public bool first_tutorial_tap;
        public bool second_tutorial_tap;
        public bool third_tutorial_tap;
        public bool first_tutorial_popup_tap;
        public bool first_huntingzone_enter;
        public bool first_monster_kill;
        public bool first_food_setting;
        public bool first_food_production;
        public bool first_food_serve;
        public bool first_food_sales;
        public bool first_costume_gatcha;
        public bool first_costume_equip;
        public bool first_costume_upgrade;
        
        /// <summary>
        /// PopUpReward
        /// </summary>
        public bool SuperHunterInitialTrigger;
        
        /// <summary>
        /// TutorialSettings
        /// </summary>
        public bool InitialTutorialClear;
        public bool InitialCostumeGacha;
        public Dictionary<int, bool> PopUpTutorialClear = new();

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
        /// PlayerLevelSettings
        /// </summary>
        public int CurrentPlayerLevel;
        public int CurrentPlayerExp;
        
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
        public int CurrentListQuestIndex;
        public Dictionary<int, Dictionary<int, bool>> ListQuestClearStatuses = new();
        public Dictionary<int, Dictionary<int, bool>> QuestClearStatuses = new();
        public Dictionary<int, Dictionary<int, int>> QuestCurrentGoalCounts = new();
        
        public void ResetStageData()
        {
            Gold = 300;
            
            CreatureItems = new Dictionary<string, Dictionary<string, int>>();
            BuildingInputItems = new Dictionary<string, Dictionary<string, int>>();
            BuildingOutputItems = new Dictionary<string, Dictionary<string, int>>();
            CurrentBuildingLevel = new Dictionary<string, int>();
            CurrentBuildingOption1Level = new Dictionary<string, int>();
            CurrentBuildingOption2Level = new Dictionary<string, int>();
            
            ActiveStatusSettingIndex = 0;
            BuildingActiveStatuses = new Dictionary<string, EActiveStatus>();
            HuntingZoneActiveStatuses = new Dictionary<string, EActiveStatus>();
            RequiredMoneyForBuildingActive = new Dictionary<string, int>();
            RequiredMoneyForHuntingZoneActive = new Dictionary<string, int>();

            CurrentListQuestIndex = -1;
            ListQuestClearStatuses = new Dictionary<int, Dictionary<int, bool>>();
            QuestClearStatuses = new Dictionary<int, Dictionary<int, bool>>();
            QuestCurrentGoalCounts = new Dictionary<int, Dictionary<int, int>>();
        }
    }
}