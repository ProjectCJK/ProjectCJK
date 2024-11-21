using System;
using System.Collections.Generic;
using System.Linq;
using GoogleSheets;
using Modules.DesignPatterns.Singletons;
using UI;
using UI.QuestPanels;
using Units.Stages.Enums;
using Units.Stages.Managers;
using Units.Stages.Modules;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Enums;
using Units.Stages.Units.HuntingZones;
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
        Stand_A,
        Stand_B,
        Stand_C,
        Stand_D,
        HuntingZone_A,
        HuntingZone_B,
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
        public Action<EQuestType1, string, int> OnUpdateCurrentQuestProgress;

        public Dictionary<int, bool> IsQuestClear;
        
        private string[,] _gameData;
        private QuestData _questData;
        
        private UI_Panel_Quest _uiPanelQuest;
        
        private int CurrentQuestSubIndex;
        private int _maxSubIndexForStage;

        public void RegisterReference()
        {
            _uiPanelQuest = UIManager.Instance.UI_Panel_Main.UI_Panel_Quest;
            _gameData = DataManager.Instance.QuestData.GetData();
            CurrentQuestSubIndex = 1;
            OnUpdateCurrentQuestProgress += HandleOnUpdateCurrentQuestProgress;
            Debug.Log("QuestManager: RegisterReference completed.");
        }

        public void InitializeQuestData()
        {
            _maxSubIndexForStage = Enumerable.Range(0, _gameData.GetLength(0))
                .Where(i => _gameData[i, 1] == GameManager.Instance.ES3Saver.CurrentStageLevel.ToString())
                .Select(i => int.Parse(_gameData[i, 2]))
                .Max();

            List<List<string>> questData = Enumerable.Range(0, _gameData.GetLength(0))
                .Where(i => _gameData[i, 1] == GameManager.Instance.ES3Saver.CurrentStageLevel.ToString() &&
                            _gameData[i, 2] == CurrentQuestSubIndex.ToString())
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

                        // 저장 대상 제외 (SetLevelUpOption1Goal 관련)
                        if (ParserModule.ParseStringToEnum<EQuestType1>(_questData.Datas[i].QuestType1) == EQuestType1.LevelUpOption1 || ParserModule.ParseStringToEnum<EQuestType1>(_questData.Datas[i].QuestType1) == EQuestType1.Build)
                        {
                            SetLevelUpOption1Goal(i);
                        }
                        else
                        {
                            // 저장된 진행도 로드
                            _questData.Datas[i].CurrentTargetGoal = GameManager.Instance.ES3Saver.QuestProgress.ContainsKey(i)
                                ? GameManager.Instance.ES3Saver.QuestProgress[i]
                                : 0;
                        }
                    }
                }

                // 클리어 상태 로드
                IsQuestClear = _questData.Datas.Keys.ToDictionary(
                    questNumber => questNumber,
                    questNumber => GameManager.Instance.ES3Saver.ClearedQuestStatuses.ContainsKey(questNumber) &&
                                   GameManager.Instance.ES3Saver.ClearedQuestStatuses[questNumber]
                );

                UpdateUI();
            }
        }

        private void SetLevelUpOption1Goal(int questIndex)
        {
            if (ParserModule.ParseStringToEnum<EQuestType1>(_questData.Datas[questIndex].QuestType1) == EQuestType1.LevelUpOption1)
            {
                (EBuildingType?, EMaterialType?) parsedEnum = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(_questData.Datas[questIndex].QuestType2);

                switch (parsedEnum.Item1)
                {
                    case EBuildingType.Kitchen:
                    {
                        if (parsedEnum.Item2 != null)
                        {
                            var value = GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[VolatileDataManager.Instance.KitchenStatsModule[parsedEnum.Item2.Value].BuildingKey];
                            _questData.Datas[questIndex].CurrentTargetGoal = value >= _questData.Datas[questIndex].MaxTargetGoal
                                ? _questData.Datas[questIndex].MaxTargetGoal
                                : value;
                        }

                        break;
                    }
                    case EBuildingType.ManagementDesk:
                    {
                        var value= GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[VolatileDataManager.Instance.ManagementDeskStatsModule.BuildingKey];
                        _questData.Datas[questIndex].CurrentTargetGoal = value >= _questData.Datas[questIndex].MaxTargetGoal
                            ? _questData.Datas[questIndex].MaxTargetGoal
                            : value;
                        break;
                    }
                    case EBuildingType.WareHouse:
                    {
                        var value = GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[VolatileDataManager.Instance.WareHouseStatsModule.BuildingKey];
                        _questData.Datas[questIndex].CurrentTargetGoal = value >= _questData.Datas[questIndex].MaxTargetGoal
                            ? _questData.Datas[questIndex].MaxTargetGoal
                            : value;
                        break;
                    }
                    case EBuildingType.DeliveryLodging:
                    {
                        var value = GameManager.Instance.ES3Saver.CurrentBuildingOption1Level[VolatileDataManager.Instance.DeliveryLodgingStatsModule.BuildingKey];
                        _questData.Datas[questIndex].CurrentTargetGoal = value >= _questData.Datas[questIndex].MaxTargetGoal
                            ? _questData.Datas[questIndex].MaxTargetGoal
                            : value;
                        break;
                    }
                }
            }
            else if (ParserModule.ParseStringToEnum<EQuestType1>(_questData.Datas[questIndex].QuestType1) == EQuestType1.Build)
            {
                (EBuildingType?, EMaterialType?) parsedEnum = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(_questData.Datas[questIndex].QuestType2);

                if (parsedEnum.Item1 != null)
                {
                    foreach (KeyValuePair<string, EActiveStatus> obj in GameManager.Instance.ES3Saver.BuildingActiveStatuses)
                    {
                        if (obj.Key == _questData.Datas[questIndex].QuestType2 && obj.Value == EActiveStatus.Active)
                        {
                            _questData.Datas[questIndex].CurrentTargetGoal = 1;
                            break;
                        }

                        _questData.Datas[questIndex].CurrentTargetGoal = 0;
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, EActiveStatus> obj in GameManager.Instance.ES3Saver.HuntingZoneActiveStatuses)
                    {
                        if (obj.Key == _questData.Datas[questIndex].QuestType2 && obj.Value == EActiveStatus.Active)
                        {
                            _questData.Datas[questIndex].CurrentTargetGoal = 1;
                            break;
                        }

                        _questData.Datas[questIndex].CurrentTargetGoal = 0;
                    }
                }
            }
        }

        private void UpdateUI()
        {
            KeyValuePair<int, Data> smallestUnclearedQuest = _questData.Datas
                .Where(kvp => !IsQuestClear[kvp.Key])
                .OrderBy(kvp => kvp.Key)
                .FirstOrDefault();

            string thumbnailDescription = null;
            var thumbnailCurrentGoal = 0;
            var thumbnailMaxGoal = 0;
            
            if (smallestUnclearedQuest.Value != null)
            {
                thumbnailDescription = smallestUnclearedQuest.Value.Description;
                thumbnailCurrentGoal = smallestUnclearedQuest.Value.CurrentTargetGoal;
                thumbnailMaxGoal = smallestUnclearedQuest.Value.MaxTargetGoal;
            }
      
            var clearedCount = IsQuestClear.Values.Count(cleared => cleared);
            var totalCount = IsQuestClear.Count;
            var progressRatio = (float)clearedCount / totalCount;

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

        private void HandleOnUpdateCurrentQuestProgress(EQuestType1 questType1, string questType2, int value)
        {
            EQuestType2? parsedQuestType = ParserModule.ParseStringToEnum<EQuestType2>(questType2);

            if (parsedQuestType != null)
            {
                var questUpdated = false;

                foreach (var kvp in _questData.Datas)
                {
                    if (kvp.Value.QuestType1 == questType1.ToString() && kvp.Value.QuestType2 == questType2)
                    {
                        if (kvp.Value.CurrentTargetGoal < kvp.Value.MaxTargetGoal)
                        {
                            kvp.Value.CurrentTargetGoal += value;

                            if (kvp.Value.CurrentTargetGoal >= kvp.Value.MaxTargetGoal)
                            {
                                kvp.Value.CurrentTargetGoal = kvp.Value.MaxTargetGoal;
                            }
                            
                            GameManager.Instance.ES3Saver.QuestProgress[kvp.Key] = kvp.Value.CurrentTargetGoal;
                            
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
            ECurrencyType? questReward1Type = ParserModule.ParseStringToEnum<ECurrencyType>(_questData.Datas[questIndex].Reward1Type);
            if (questReward1Type != null)
            {
                AddQuestReward(questReward1Type.Value, _questData.Datas[questIndex].Reward1Count);
            }

            ECurrencyType? questReward2Type = ParserModule.ParseStringToEnum<ECurrencyType>(_questData.Datas[questIndex].Reward2Type);
            if (questReward2Type != null)
            {
                AddQuestReward(questReward2Type.Value, _questData.Datas[questIndex].Reward2Count);
            }
            
            if (_questData.Datas.ContainsKey(questIndex) && IsQuestClear[questIndex] == false)
            {
                IsQuestClear[questIndex] = true;
                UpdateUI();

                GameManager.Instance.ES3Saver.ClearedQuestStatuses[questIndex] = true;
                
                if (IsQuestClear.Values.All(status => status))
                {
                    _uiPanelQuest.MainQuest.EnableRewardButton();
                }
            }
        }

        private void AdvanceToNextQuest()
        {
            if (_questData.ListRewardType != null)
            {
                CurrencyManager.Instance.AddCurrency(_questData.ListRewardType.Value, _questData.ListRewardCount);
            }

            if (CurrentQuestSubIndex < _maxSubIndexForStage)
            {
                CurrentQuestSubIndex++;
                
                GameManager.Instance.ES3Saver.ClearedQuestStatuses.Clear();
                GameManager.Instance.ES3Saver.QuestProgress.Clear();
                GameManager.Instance.ES3Saver.CurrentQuestSubIndex = CurrentQuestSubIndex;

                InitializeQuestData();
                UpdateUI();
            }
            else
            {
                Debug.Log("QuestManager: All quests for the current stage are complete!");
            }
        }
        
        private void AddQuestReward(ECurrencyType currencyType, int reward)
        {
            switch (currencyType)
            {
                case ECurrencyType.Money:
                case ECurrencyType.Diamond:
                case ECurrencyType.RedGem:
                    CurrencyManager.Instance.AddCurrency(currencyType, reward);
                    break;
                case ECurrencyType.Star:
                    LevelManager.Instance.AddExp(reward);
                    break;
            }
        }
    }
}