using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheets;
using Modules.DesignPatterns.Singletons;
using UI;
using Units.Stages.Enums;
using Units.Stages.Modules;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
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
        ProductA_A,
        ProductA_B,
        ProductA_C,
        ProductA_D,
        ProductB_A,
        ProductB_B,
        ProductB_C,
        ProductB_D,
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
    public struct QuestDataBundle
    {
        public int ClearedCount;
        public int TotalCount;
        public float ProgressRatio;
        public int RewardCount;
        public string ThumbnailDescription;
        public int ThumbnailCurrentGoal;
        public int ThumbnailMaxGoal;
        public List<UIQuestInfoItem> QuestInfoItems;
        public Action AdvanceToNextQuestAction;
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
        public Action<EQuestType1, string> OnUpdateCurrentQuestProgress;

        private string[,] _gameData;
        private QuestData _questData;
        public int CurrentQuestMainIndex;
        public int CurrentQuestSubIndex;
        public Dictionary<int, bool> IsQuestClear;
        private int _maxSubIndexForStage;
        private UI_Panel_Quest _uiPanelQuest;

        public void RegisterReference(UI_Panel_Quest uiPanelQuest)
        {
            _uiPanelQuest = uiPanelQuest;
            _gameData = DataManager.Instance.QuestData.GetData();
            CurrentQuestMainIndex = 1;
            CurrentQuestSubIndex = 1;
            OnUpdateCurrentQuestProgress += HandleOnUpdateCurrentQuestProgress;
            Debug.Log("QuestManager: RegisterReference completed.");
        }

        public void InitializeQuestData()
        {
            _maxSubIndexForStage = Enumerable.Range(0, _gameData.GetLength(0))
                .Where(i => _gameData[i, 1] == CurrentQuestMainIndex.ToString())
                .Select(i => int.Parse(_gameData[i, 2]))
                .Max();

            var questData = Enumerable.Range(0, _gameData.GetLength(0))
                .Where(i => _gameData[i, 1] == CurrentQuestMainIndex.ToString() && _gameData[i, 2] == CurrentQuestSubIndex.ToString())
                .Select(i => Enumerable.Range(0, _gameData.GetLength(1)).Select(j => _gameData[i, j]).ToList())
                .ToList();

            if (questData.Count > 0)
            {
                _questData = new QuestData
                {
                    Stage = int.Parse(questData[0][1]),
                    ListRewardType = ParserModule.ParseStringToEnum<ECurrencyType>(questData[0][13]),
                    ListRewardCount = int.Parse(questData[0][14]),
                    Datas = new Dictionary<int, Data>()
                };

                for (var i = 0; i < questData.Count; i++)
                {
                    if (!_questData.Datas.ContainsKey(i))
                    {
                        _questData.Datas.Add(i, new Data
                        {
                            Description = questData[i][5],
                            QuestType1 = questData[i][6],
                            QuestType2 = questData[i][7],
                            MaxTargetGoal = int.Parse(questData[i][8]),
                            Reward1Type = questData[i][9],
                            Reward1Count = int.Parse(questData[i][10]),
                            Reward2Type = questData[i][11],
                            Reward2Count = int.Parse(questData[i][12])
                        });
                        
                        SetLevelUpOption1Goal(i);
                    }
                }
                
                IsQuestClear = _questData.Datas.Keys.ToDictionary(questNumber => questNumber, _ => false);
                UpdateUI();
                Debug.Log("QuestManager: InitializeQuestData completed.");
            }
            else
            {
                Debug.LogWarning("QuestManager: No quest data found.");
            }
        }

        private void SetLevelUpOption1Goal(int questIndex)
        {
            if (ParserModule.ParseStringToEnum<EQuestType1>(_questData.Datas[questIndex].QuestType1) == EQuestType1.LevelUpOption1)
            {
                (EBuildingType?, EMaterialType?) parsedEnum = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(_questData.Datas[questIndex].QuestType2);

                if (parsedEnum.Item1 == EBuildingType.Kitchen)
                {
                    _questData.Datas[questIndex].CurrentTargetGoal = VolatileDataManager.Instance.KitchenStatsModule[parsedEnum.Item2.Value].CurrentBuildingOption1Level;
                }
                else if (parsedEnum.Item1 == EBuildingType.ManagementDesk)
                {
                    _questData.Datas[questIndex].CurrentTargetGoal = VolatileDataManager.Instance.ManagementDeskStatsModule.CurrentBuildingOption1Level;
                }
                else if (parsedEnum.Item1 == EBuildingType.WareHouse)
                {
                    _questData.Datas[questIndex].CurrentTargetGoal = VolatileDataManager.Instance.WareHouseStatsModule.CurrentBuildingOption1Level;
                }
                else if (parsedEnum.Item1 == EBuildingType.DeliveryLodging)
                {
                    _questData.Datas[questIndex].CurrentTargetGoal = VolatileDataManager.Instance.DeliveryLodgingStatsModule.CurrentBuildingOption1Level;
                }
            }
            else if (ParserModule.ParseStringToEnum<EQuestType1>(_questData.Datas[questIndex].QuestType1) == EQuestType1.Build)
            {
                (EBuildingType?, EMaterialType?) parsedEnum = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(_questData.Datas[questIndex].QuestType2);

                foreach (KeyValuePair<BuildingZone, EActiveStatus> obj in VolatileDataManager.Instance.BuildingActiveStatuses)
                {
                    if (obj.Key.BuildingKey == _questData.Datas[questIndex].QuestType2 && obj.Value == EActiveStatus.Active)
                    {
                        _questData.Datas[questIndex].CurrentTargetGoal = 1;
                    }
                    else
                    {
                        _questData.Datas[questIndex].CurrentTargetGoal = 0;
                    }
                }
            }
        }

        public void UpdateUI()
        {
            var smallestUnclearedQuest = _questData.Datas
                .Where(kvp => !IsQuestClear[kvp.Key])
                .OrderBy(kvp => kvp.Key)
                .FirstOrDefault();

            string thumbnailDescription = null;
            int thumbnailCurrentGoal = 0;
            int thumbnailMaxGoal = 0;
            
            if (smallestUnclearedQuest.Value != null)
            {
                thumbnailDescription = smallestUnclearedQuest.Value.Description;
                thumbnailCurrentGoal = smallestUnclearedQuest.Value.CurrentTargetGoal;
                thumbnailMaxGoal = smallestUnclearedQuest.Value.MaxTargetGoal;
            }
      
            int clearedCount = IsQuestClear.Values.Count(cleared => cleared);
            int totalCount = IsQuestClear.Count;
            float progressRatio = (float)clearedCount / totalCount;

            List<UIQuestInfoItem> questInfoItems = _questData.Datas
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => new UIQuestInfoItem
                {
                    QuestDescriptionText = kvp.Value.Description,
                    Reward1CountText = kvp.Value.Reward1Count.ToString(),
                    Reward2CountText = kvp.Value.Reward2Count.ToString(),
                    QuestProgressText = $"{kvp.Value.CurrentTargetGoal} / {kvp.Value.MaxTargetGoal}",
                    CurrentProgressCount = kvp.Value.CurrentTargetGoal,
                    MaxProgressCount = kvp.Value.MaxTargetGoal
                }).ToList();

            var questDataBundle = new QuestDataBundle
            {
                ClearedCount = clearedCount,
                TotalCount = totalCount,
                ProgressRatio = progressRatio,
                RewardCount = _questData.ListRewardCount,
                ThumbnailDescription = thumbnailDescription,
                ThumbnailCurrentGoal = thumbnailCurrentGoal,
                ThumbnailMaxGoal = thumbnailMaxGoal,
                QuestInfoItems = questInfoItems,
                AdvanceToNextQuestAction = AdvanceToNextQuest
            };

            _uiPanelQuest.UpdateQuestPanel(questDataBundle);
        }

        private void HandleOnUpdateCurrentQuestProgress(EQuestType1 questType1, string questType2)
        {
            var parsedQuestType = ParserModule.ParseStringToEnum<EQuestType2>(questType2);

            if (parsedQuestType != null)
            {
                bool questUpdated = false;

                foreach (var (subIndex, data) in _questData.Datas)
                {
                    if (data.QuestType1 == questType1.ToString() && data.QuestType2 == questType2.ToString())
                    {
                        if (data.CurrentTargetGoal < data.MaxTargetGoal)
                        {
                            data.CurrentTargetGoal++;
                            questUpdated = true;
                        }
                    }
                }

                if (questUpdated)
                {
                    UpdateUI();
                }
            }
        }

        public void MarkQuestAsCleared(int questIndex)
        {
            CurrencyManager.Instance.AddCurrency(ParserModule.ParseStringToEnum<ECurrencyType>(_questData.Datas[questIndex].Reward2Type).Value, _questData.Datas[questIndex].Reward2Count);
            
            if (_questData.Datas.ContainsKey(questIndex) && IsQuestClear[questIndex] == false)
            {
                IsQuestClear[questIndex] = true;
                UpdateUI();

                if (IsQuestClear.Values.All(status => status))
                {
                    _uiPanelQuest.MainQuest.EnableRewardButton();
                }
            }
        }

        public void AdvanceToNextQuest()
        {
            CurrencyManager.Instance.AddCurrency(_questData.ListRewardType.Value, _questData.ListRewardCount);
            
            if (CurrentQuestSubIndex < _maxSubIndexForStage)
            {
                CurrentQuestSubIndex++;
                InitializeQuestData();
                UpdateUI();
            }
            else
            {
                Debug.Log("QuestManager: All quests for the current stage are complete!");
            }
        }
    }
}