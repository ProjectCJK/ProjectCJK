using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheets;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Modules;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

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
    public class Data
    {
        public string Description { get; set; }
        public string QuestType1 { get; set; }
        public string QuestType2 { get; set; }
        public int CurrentTargetGoal { get; set; }
        public int MaxTargetGoal { get; set; }
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

        public int CurrentQuestMainIndex;
        public int CurrentQuestSubIndex;
        public Dictionary<int, bool> IsQuestClear;

        private int _maxSubIndexForStage;

        public void RegisterReference()
        {
            _gameData = DataManager.Instance.QuestData.GetData();

            CurrentQuestMainIndex = 1;
            CurrentQuestSubIndex = 1;

            OnUpdateCurrentQuestProgress += HandleOnUpdateCurrentQuestProgress;
            Debug.Log("QuestManager: RegisterReference completed.");
        }

        public void InitializeQuestData()
        {
            // 현재 스테이지에서 가장 큰 List 값(_maxSubIndexForStage)을 찾기
            _maxSubIndexForStage = Enumerable.Range(0, _gameData.GetLength(0))
                .Where(i => _gameData[i, 1] == CurrentQuestMainIndex.ToString())
                .Select(i => int.Parse(_gameData[i, 2])) // List 값을 가져옴
                .Max();

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

                    Datas = new Dictionary<int, Data>()
                };

                // 수동으로 Dictionary에 데이터를 추가하여 QuestNumber를 키로 사용
                foreach (var row in questData)
                {
                    int questNumber = int.Parse(row[3]); // QuestNumber
                    if (!_questData.Datas.ContainsKey(questNumber))
                    {
                        _questData.Datas.Add(questNumber, new Data()
                        {
                            Description = row[5],
                            QuestType1 = row[6],
                            QuestType2 = row[7],
                            MaxTargetGoal = int.Parse(row[8]),
                            Reward1Type = row[9],
                            Reward1Count = int.Parse(row[10]),
                            Reward2Type = row[11],
                            Reward2Count = int.Parse(row[12])
                        });
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate QuestNumber {questNumber} encountered and ignored in quest data.");
                    }
                }

                // IsQuestClear 딕셔너리 초기화, QuestNumber를 키로 사용
                IsQuestClear = _questData.Datas.Keys.ToDictionary(questNumber => questNumber, _ => false);
                Debug.Log("QuestManager: InitializeQuestData completed. Loaded quest data for stage: " + _questData.Stage);
            }
            else
            {
                Debug.LogWarning("QuestManager: No quest data found for the current main and sub indices.");
            }
        }

        private void HandleOnUpdateCurrentQuestProgress(EQuestType1 questType1, EQuestType2 questType2)
        {
            bool questFound = false;

            foreach ((var subIndex, Data data) in _questData.Datas)
            {
                if (data.QuestType1 == questType1.ToString() && data.QuestType2 == questType2.ToString())
                {
                    questFound = true;

                    if (data.CurrentTargetGoal < data.MaxTargetGoal)
                    {
                        data.CurrentTargetGoal++;
                        Debug.Log($"QuestManager: Quest {subIndex} progress updated. Current goal: {data.CurrentTargetGoal}/{data.MaxTargetGoal}");
                    }

                    if (data.CurrentTargetGoal >= data.MaxTargetGoal)
                    {
                        IsQuestClear[subIndex] = true;
                        Debug.Log($"QuestManager: Quest {subIndex} completed!");
                    }
                }
            }

            if (!questFound)
            {
                Debug.LogWarning($"QuestManager: No matching quest found for type1: {questType1}, type2: {questType2}");
            }

            // Check if all quests in the current sub-index are completed
            if (IsQuestClear.Values.All(status => status))
            {
                AdvanceToNextQuest();
            }
        }

        private void AdvanceToNextQuest()
        {
            if (CurrentQuestSubIndex < _maxSubIndexForStage)
            {
                CurrentQuestSubIndex++;
                InitializeQuestData();
                Debug.Log($"QuestManager: Moved to next quest set - Main Index: {CurrentQuestMainIndex}, Sub Index: {CurrentQuestSubIndex}");
            }
            else
            {
                Debug.Log("QuestManager: All quests for the current stage are complete!");
            }
        }
    }
}