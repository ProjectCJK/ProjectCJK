using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheets;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Modules;
using Units.Stages.Units.Items.Enums;

namespace Managers
{
    public enum EQuestType1
    {
        Build,
        Get,
        LevelUpOption1,
        Product,
        Selling,
        Zone_Open
    }
    
    public enum EQuestType2
    {
        Kitchen_A,
        Kitchen_B,
        Kitchen_C,
        Kitchen_D,
        Material_A,
        Material_B,
        Material_C,
        Product_A,
        Product_B,
        Product_C,
        Product_D,
        Stand_B,
        Stand_C,
        Stand_D,
        WareHouse,
        ManagementDesk,
        DeliveryLodging,
        Money,
        Zone_Open
    }
    
    [Serializable]
    public struct QuestData
    {
        public int Stage { get; set; }
        public ECurrencyType? ListRewardType { get; set; }
        public int ListRewardCount { get; set; }
        public Dictionary<int, Data> Datas;
    }

    [Serializable]
    public struct Data
    {
        public string Description { get; set; }
        public string QuestType1 { get; set; }
        public string QuestType2 { get; set; }
        public int TargetGoal { get; set; }
        public string Reward1Type { get; set; }
        public int Reward1Count { get; set; }
        public string Reward2Type { get; set; }
        public int Reward2Count { get; set; }
    }
    
    public class QuestManager : Singleton<QuestManager>
    {
        public Action<EQuestType1, EQuestType2> OnUpdateCurrentQuestProgress;
        
        private string[,] _gameData;
        private QuestData _questData;
        
        // TODO : 데이터 저장해야함!
        public int CurrentQuestMainIndex;
        public int CurrentQuestSubIndex;
        public Dictionary<int, bool> IsQuestClear;
        
        public void RegisterReference()
        {
            _gameData = DataManager.Instance.QuestData.GetData();

            CurrentQuestMainIndex = 1;
            CurrentQuestSubIndex = 1;

            OnUpdateCurrentQuestProgress += HandleOnUpdateCurrentQuestProgress;
        }

        public void InitializeQuestData()
        {
            List<List<string>> questData = Enumerable.Range(0, _gameData.GetLength(0))
                .Where(i =>
                    _gameData[i, 1] == CurrentQuestMainIndex.ToString() &&
                    _gameData[i, 2] == CurrentQuestSubIndex.ToString())
                .Select(i =>
                    Enumerable.Range(0, _gameData.GetLength(1))
                        .Select(j => _gameData[i, j])
                        .ToList())
                .ToList();

            if (questData.Count > 0)
            {
                _questData = new QuestData()
                {
                    Stage = int.Parse(questData[0][1]),
                    ListRewardType = ParserModule.ParseStringToEnum<ECurrencyType>(questData[0][13]),
                    ListRewardCount = int.Parse(questData[0][14]),
                    
                    Datas = questData.ToDictionary(
                        row => int.Parse(row[2]),
                        row => new Data()
                        {
                            Description = row[5],
                            QuestType1 = row[6],
                            QuestType2 = row[7],
                            TargetGoal = int.Parse(row[8]),
                            Reward1Type = row[9],
                            Reward1Count = int.Parse(row[10]),
                            Reward2Type = row[11],
                            Reward2Count = int.Parse(row[12])
                        })
                };
            }
        }

        private void HandleOnUpdateCurrentQuestProgress(EQuestType1 questType1, EQuestType2 questType2)
        {
            
        }
    }
}