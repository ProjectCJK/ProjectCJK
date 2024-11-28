using System;
using System.Collections.Generic;
using System.Linq;
using Modules.DesignPatterns.Singletons;
using UI.MainPanels;
using UI.QuestPanels;
using Units.Stages.Controllers;
using Units.Stages.Enums;
using Units.Stages.Modules;
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
        Display,
        LevelUpOption2,
        Open,
        Equip,
        Upgrade
    }

    public enum EQuestType2
    {
        KitchenA_A,
        KitchenA_B,
        KitchenA_C,
        KitchenB_A,
        Material_A,
        Material_B,
        Material_C,
        ProductA_A,
        ProductA_B,
        ProductA_C,
        ProductB_A,
        StandA_A,
        StandA_B,
        StandA_C,
        StandB_A,
        HuntingZone_A,
        HuntingZone_B,
        WareHouse,
        ManagementDesk,
        DeliveryLodging,
        Money,
        Zone_Open,
        HuntingZone_C,
        Quest
    }

    public enum EQuestTarget
    {
        HuntingZone_A_Unlock,
        HuntingZone_A_Hunt,
        HuntingZone_B_Unlock,
        HuntingZone_B_Hunt,
        HuntingZone_C_Unlock,
        HuntingZone_C_Hunt,
        KitchenA_A_Unlock,
        KitchenA_A_Product,
        KitchenA_A_Upgrade,
        KitchenA_B_Unlock,
        KitchenA_B_Product,
        KitchenA_B_Upgrade,
        KitchenA_C_Unlock,
        KitchenA_C_Product,
        KitchenA_C_Upgrade,
        KitchenB_A_Unlock,
        KitchenB_A_Product,
        KitchenB_A_Upgrade,
        StandA_A_Unlock,
        StandA_B_Unlock,
        StandA_C_Unlock,
        StandB_A_Unlock,
        ManagementDesk_Upgrade,
        ManagementDesk_GetMoney,
        WareHouse_Unlock,
        WareHouse_Upgrade,
        DeliveryLodging_Unlock,
        DeliveryLodging_Upgrade,
        ProductA_A_Sell,
        ProductA_B_Sell,
        ProductA_C_Sell,
        ProductB_A_Sell,
        Temp_ManagementDesk,
        Quest
    }

    [Serializable]
    public class ParsedStageQuestData
    {
        public int StageIndex;
        public Sprite StageIcon;
        public string StageName;
        
        public List<ParsedListQuestData> ParsedListQuestDatas;
        public List<ParsedQuestData> ParsedQuestDatas;
        
        public ParsedStageQuestData(int stageIndex, Sprite stageIcon, string stageName)
        {
            StageIndex = stageIndex;
            StageIcon = stageIcon;
            StageName = stageName;
        }
    }

    [Serializable]
    public class ParsedListQuestData
    {
        public int ListIndex;
        public ECurrencyType ListRewardType;
        public int ListRewardCount;
        public bool IsClear;

        public List<int> ParsedQuestDataIndex;
    }
    
    [Serializable]
    public class ParsedQuestData
    {
        public Sprite QuestIcon;
        public int QuestIndex;
        
        public string QuestDescription;
        
        public EQuestType1 QuestType1;
        public EQuestType2 QuestType2;
        
        public int CurrentGoal;
        public int MaxGoal;
        public bool IsClear;
        
        public ECurrencyType RewardType1;
        public int RewardType1Count;
        public ECurrencyType RewardType2;
        public int RewardType2Count;
        
        public EQuestTarget TrackingTarget;
    }

    public class QuestManager : Singleton<QuestManager>
    {
        public Action<EQuestType1, string, int> OnUpdateCurrentQuestProgress;
        
        private string[,] _gameData;
        
        private UI_Panel_Quest _uiPanelQuest;
        private UI_Panel_QuestClear _uiPanelReward;
        
        private StageController _stageController;

        private readonly Dictionary<int, ParsedStageQuestData> _parsedStageQuestDatas = new();

        public void RegisterReference(StageController stageController)
        {
            _stageController = stageController;
            
            _uiPanelQuest = UIManager.Instance.UI_Panel_Main.UI_Panel_Quest;
            _uiPanelReward = UIManager.Instance.UI_Panel_Main.UI_Panel_QuestClear;
            _gameData = DataManager.Instance.QuestData.GetData();
            
            _uiPanelQuest.RegisterReference();
            _uiPanelReward.RegisterReference();
            
            OnUpdateCurrentQuestProgress += HandleOnUpdateCurrentQuestProgress;
            
            UIManager.Instance.Joystick.OnClickPanel += () => _uiPanelQuest.MainQuest.gameObject.SetActive(false);
            Debug.Log("QuestManager: RegisterReference completed.");
        }
        
        public void Initialize()
        {
            List<List<string>> rawDatas = Enumerable.Range(2, _gameData.GetLength(0) - 2)
                .Select(i => Enumerable.Range(0, _gameData.GetLength(1)).Select(j => _gameData[i, j]).ToList())
                .ToList();

            var currentStageIndex = -1;
            var currentListIndex = -1;

            ParsedStageQuestData currentStageData = null;
            ParsedListQuestData currentListData = null;

            foreach (List<string> rawData in rawDatas)
            {
                // 1. 스테이지 데이터 추가
                var stageIndex = int.Parse(rawData[1]);
                if (stageIndex != currentStageIndex)
                {
                    currentStageData = AddNewStage(stageIndex);
                    currentStageIndex = stageIndex;
                }

                // 2. 리스트 데이터 추가
                var listIndex = int.Parse(rawData[2]);
                if (listIndex != currentListIndex)
                {
                    currentListData = AddNewList(currentStageData, stageIndex, listIndex, rawData);
                    currentListIndex = listIndex;
                }

                // 3. 퀘스트 데이터 추가
                ParsedQuestData questData = CreateQuestData(rawData, stageIndex);
                _parsedStageQuestDatas[stageIndex].ParsedQuestDatas.Add(questData);
                currentListData?.ParsedQuestDataIndex.Add(questData.QuestIndex);
            }

            if (GameManager.Instance.ES3Saver.CurrentListQuestIndex == -1) GameManager.Instance.ES3Saver.CurrentListQuestIndex = 0;

            UpdateUI();
        }
        
        private ParsedStageQuestData AddNewStage(int stageIndex)
        {
            Sprite stageIcon = DataManager.Instance.MapSprites[stageIndex - 1];
            var stageName = stageIndex == 1 ? "Dangerous vacant lot" : "Dilapidated road";

            var newStageData = new ParsedStageQuestData(stageIndex, stageIcon, stageName)
            {
                ParsedListQuestDatas = new List<ParsedListQuestData>(),
                ParsedQuestDatas = new List<ParsedQuestData>()
            };
            
            if (stageIndex == 2)
            {
                for (var i = 0; i < _parsedStageQuestDatas[1].ParsedQuestDatas.Count; i++)
                {
                    newStageData.ParsedQuestDatas.Add(new ParsedQuestData());
                }
            }

            _parsedStageQuestDatas.TryAdd(stageIndex, newStageData);
            return newStageData;
        }

        private ParsedListQuestData AddNewList(ParsedStageQuestData currentStageData, int stageIndex, int listIndex, List<string> rawData)
        {
            var listRewardType = Enum.Parse<ECurrencyType>(rawData[13]);
            var listRewardCount = int.Parse(rawData[14]);

            var newListData = new ParsedListQuestData
            {
                ListIndex = listIndex,
                ListRewardType = listRewardType,
                ListRewardCount = listRewardCount,
                ParsedQuestDataIndex = new List<int>(),
                IsClear = GetOrAdd(GetOrAdd(GameManager.Instance.ES3Saver.ListQuestClearStatuses, stageIndex, new Dictionary<int, bool>()), listIndex, false)
            };

            currentStageData?.ParsedListQuestDatas.Add(newListData);
            return newListData;
        }
        
        private ParsedQuestData CreateQuestData(List<string> rawData, int stageIndex)
        {
            var questData = new ParsedQuestData
            {
                QuestIndex = int.Parse(rawData[3]),
                QuestDescription = rawData[5],
                QuestType1 = Enum.Parse<EQuestType1>(rawData[6]),
                QuestType2 = Enum.Parse<EQuestType2>(rawData[7]),
                CurrentGoal = 0,
                MaxGoal = int.Parse(rawData[8]),
                IsClear = false,
                RewardType1 = Enum.Parse<ECurrencyType>(rawData[9]),
                RewardType1Count = int.Parse(rawData[10]),
                RewardType2 = Enum.Parse<ECurrencyType>(rawData[11]),
                RewardType2Count = int.Parse(rawData[12]),
                TrackingTarget = Enum.Parse<EQuestTarget>(rawData[15])
            };

            questData.IsClear = GetOrAdd(GetOrAdd(GameManager.Instance.ES3Saver.QuestClearStatuses, stageIndex, new Dictionary<int, bool>()), questData.QuestIndex, false);
            questData.CurrentGoal = questData.QuestType1 is EQuestType1.LevelUpOption1 or EQuestType1.LevelUpOption2 or EQuestType1.Build ? SetLevelUpOption1Goal(questData) : GetOrAdd(GetOrAdd(GameManager.Instance.ES3Saver.QuestCurrentGoalCounts, stageIndex, new Dictionary<int, int>()), questData.QuestIndex, 0);

            return questData;
        }

        private int SetLevelUpOption1Goal(ParsedQuestData parsedQuestData)
        {
            return parsedQuestData.QuestType1 switch
            {
                EQuestType1.LevelUpOption1 => CalculateOption1LevelUpGoal(parsedQuestData),
                EQuestType1.LevelUpOption2 => CalculateOption2LevelUpGoal(parsedQuestData),
                EQuestType1.Build => CalculateBuildGoal(parsedQuestData),
                _ => 0
            };
        }

        private int CalculateOption1LevelUpGoal(ParsedQuestData parsedQuestData)
        {
            (EBuildingType?, EMaterialType?) parsedEnum = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(parsedQuestData.QuestType2.ToString());

            if (parsedEnum.Item1 == null) return 0;

            var value = parsedEnum.Item1 switch
            {
                EBuildingType.KitchenA when parsedEnum.Item2.HasValue =>
                    VolatileDataManager.Instance.KitchenStatsModule != null &&
                    VolatileDataManager.Instance.KitchenStatsModule.ContainsKey(parsedEnum.Item2.Value)
                        ? GetBuildingOption1Level(VolatileDataManager.Instance.KitchenStatsModule[parsedEnum.Item2.Value].BuildingKey)
                        : 0,
                EBuildingType.ManagementDesk => 
                    VolatileDataManager.Instance.ManagementDeskStatsModule != null
                        ? GetBuildingOption1Level(VolatileDataManager.Instance.ManagementDeskStatsModule.BuildingKey)
                        : 0,
                EBuildingType.WareHouse => 
                    VolatileDataManager.Instance.WareHouseStatsModule != null
                        ? GetBuildingOption1Level(VolatileDataManager.Instance.WareHouseStatsModule.BuildingKey)
                        : 0,
                EBuildingType.DeliveryLodging => 
                    VolatileDataManager.Instance.DeliveryLodgingStatsModule != null
                        ? GetBuildingOption1Level(VolatileDataManager.Instance.DeliveryLodgingStatsModule.BuildingKey)
                        : 0,
                _ => 0
            };

            return Math.Min(value, parsedQuestData.MaxGoal);
        }
        
        private int CalculateOption2LevelUpGoal(ParsedQuestData parsedQuestData)
        {
            (EBuildingType?, EMaterialType?) parsedEnum = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(parsedQuestData.QuestType2.ToString());

            if (parsedEnum.Item1 == null) return 0;

            var value = parsedEnum.Item1 switch
            {
                EBuildingType.ManagementDesk => 
                    VolatileDataManager.Instance.ManagementDeskStatsModule != null
                        ? GetBuildingOption2Level(VolatileDataManager.Instance.ManagementDeskStatsModule.BuildingKey)
                        : 0,
                _ => 0
            };

            return Math.Min(value, parsedQuestData.MaxGoal);
        }

        private int CalculateBuildGoal(ParsedQuestData parsedQuestData)
        {
            var questType2 = $"{parsedQuestData.QuestType2}";
            (EBuildingType?, EMaterialType?) parsedEnum = ParserModule.ParseStringToEnum<EBuildingType, EMaterialType>(questType2);
            
            Dictionary<string, EActiveStatus> activeStatuses = parsedEnum.Item1 != null ? GameManager.Instance.ES3Saver.BuildingActiveStatuses : GameManager.Instance.ES3Saver.HuntingZoneActiveStatuses;

            return activeStatuses.TryGetValue(questType2, out var status) && status == EActiveStatus.Active ? 1 : 0;
        }

        private int GetBuildingOption1Level(string buildingKey)
        {
            return GameManager.Instance.ES3Saver.CurrentBuildingOption1Level.GetValueOrDefault(buildingKey, 0);
        }
        
        private int GetBuildingOption2Level(string buildingKey)
        {
            return GameManager.Instance.ES3Saver.CurrentBuildingOption2Level.GetValueOrDefault(buildingKey, 0);
        }

        private void UpdateUI()
        {
            var currentStageIndex = GameManager.Instance.ES3Saver.CurrentStageLevel;
            var currentListIndex = GameManager.Instance.ES3Saver.CurrentListQuestIndex;
            
            if (currentListIndex + 1 > _parsedStageQuestDatas[currentStageIndex].ParsedListQuestDatas.Count)
            {
                _uiPanelQuest.UpdateStageLastQuestPanel();
                return;
            }
            
            ParsedStageQuestData currentStageQuestData = _parsedStageQuestDatas[GameManager.Instance.ES3Saver.CurrentStageLevel];
            ParsedListQuestData currentListQuestData = currentStageQuestData.ParsedListQuestDatas[GameManager.Instance.ES3Saver.CurrentListQuestIndex];
            
            var currentQuestData = currentListQuestData.ParsedQuestDataIndex
                .Select(index => currentStageQuestData.ParsedQuestDatas
                    .FirstOrDefault(quest => quest.QuestIndex == index)) // QuestIndex로 필터링
                .Where(quest => quest != null) 
                .OrderBy(q => q.IsClear) // IsClear가 false인 항목이 상위로
                .ThenByDescending(q => q.CurrentGoal >= q.MaxGoal) // IsClear가 false이면서 목표를 달성한 항목이 더 상단으로
                .ThenBy(q => q.QuestIndex) // QuestIndex 기준으로 오름차순 정렬
                .ToList();

            if (currentQuestData.Count > 0)
            {
                ParsedQuestData trackingTargetQuest = currentQuestData.First();

                ActivateTutorialPanel(trackingTargetQuest.QuestIndex);
                
                if (trackingTargetQuest.CurrentGoal >= currentQuestData.First().MaxGoal)
                {
                    StopTargetTracking();
                }
                else
                {
                    StartTrackingCurrentQuestTarget(trackingTargetQuest.TrackingTarget);
                }
            }

            var stageIcon = currentStageQuestData.StageIcon;
            var stageDescription = currentStageQuestData.StageName;
            var listQuestCurrentIndex = GameManager.Instance.ES3Saver.CurrentListQuestIndex + 1;
            var listQuestMaxIndex = currentStageQuestData.ParsedListQuestDatas.Count;
      
            var questClearCount = currentQuestData.Count(obj => obj.IsClear);
            var questTotalCount = currentQuestData.Count;
            var progressRatio = (float)questClearCount / questTotalCount;
            
            var rewardSprite = DataManager.Instance.GetCurrencyIcon(currentListQuestData.ListRewardType);
            var rewardCount = currentListQuestData.ListRewardCount;
            
            List<UIQuestInfoItem> questInfoItems = currentQuestData
                .Select(data => new UIQuestInfoItem
                {
                    QuestIndex = data.QuestIndex,
                    QuestIconImage = SetQuestIcon(data.QuestType2),
                    QuestDescriptionText = data.QuestDescription,
                    Reward1IconImage = SetRewardIcon(data.RewardType1),
                    Reward2IconImage = SetRewardIcon(data.RewardType2),
                    Reward1CountText = $"{data.RewardType1Count}",
                    Reward2CountText = $"{data.RewardType2Count}",
                    QuestProgressText = $"{data.CurrentGoal} / {data.MaxGoal}",
                    CurrentProgressCount = data.CurrentGoal,
                    MaxProgressCount = data.MaxGoal
                }).ToList();

            var listQuestInfoItem = new UIListQuestInfoItem
            {
                StageIcon = stageIcon,
                StageDescription = stageDescription,
                ListQuestCurrentIndex = listQuestCurrentIndex,
                ListQuestMaxIndex = listQuestMaxIndex,
                QuestClearCount = questClearCount,
                QuestTotalCount = questTotalCount,
                ProgressRatio = progressRatio,
                RewardSprite = rewardSprite,
                RewardCount = rewardCount,
                UiQuestInfoItems = questInfoItems
            };

            _uiPanelQuest.UpdateQuestPanel(listQuestInfoItem);
        }

        private void ActivateTutorialPanel(int questIndex)
        {
            TutorialManager.Instance.ActivePopUpTutorialPanel(questIndex);
        }

        private void StartTrackingCurrentQuestTarget(EQuestTarget questTarget)
        {
            if (ObjectTrackerManager.Instance.IsTracking) return;

            Transform target = questTarget switch
            {
                EQuestTarget.HuntingZone_A_Unlock => _stageController.HuntingZoneController.HuntingZoneSpawnData
                    .HuntingZoneSpawners[0]
                    .Target[0],
                EQuestTarget.HuntingZone_A_Hunt => _stageController.HuntingZoneController.HuntingZoneSpawnData
                    .HuntingZoneSpawners[0]
                    .Target[1],
                EQuestTarget.HuntingZone_B_Unlock => _stageController.HuntingZoneController.HuntingZoneSpawnData
                    .HuntingZoneSpawners[1]
                    .Target[0],
                EQuestTarget.HuntingZone_B_Hunt => _stageController.HuntingZoneController.HuntingZoneSpawnData
                    .HuntingZoneSpawners[1]
                    .Target[1],
                EQuestTarget.HuntingZone_C_Unlock => _stageController.HuntingZoneController.HuntingZoneSpawnData
                    .HuntingZoneSpawners[2]
                    .Target[0],
                EQuestTarget.HuntingZone_C_Hunt => _stageController.HuntingZoneController.HuntingZoneSpawnData
                    .HuntingZoneSpawners[2]
                    .Target[1],
                EQuestTarget.KitchenA_A_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[0]
                    .Target[0],
                EQuestTarget.KitchenA_A_Product => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[0]
                    .Target[1],
                EQuestTarget.KitchenA_A_Upgrade => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[0]
                    .Target[2],
                EQuestTarget.KitchenA_B_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[1]
                    .Target[0],
                EQuestTarget.KitchenA_B_Product => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[1]
                    .Target[1],
                EQuestTarget.KitchenA_B_Upgrade => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[1]
                    .Target[2],
                EQuestTarget.KitchenA_C_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[2]
                    .Target[0],
                EQuestTarget.KitchenA_C_Product => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[2]
                    .Target[1],
                EQuestTarget.KitchenA_C_Upgrade => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[2]
                    .Target[2],
                EQuestTarget.KitchenB_A_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[3]
                    .Target[0],
                EQuestTarget.KitchenB_A_Product => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[3]
                    .Target[1],
                EQuestTarget.KitchenB_A_Upgrade => _stageController.BuildingController.BuildingSpawnData
                    .KitchenSpawner[3]
                    .Target[2],
                EQuestTarget.StandA_A_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[0]
                    .Target[0],
                EQuestTarget.StandA_B_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[1]
                    .Target[0],
                EQuestTarget.StandA_C_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[2]
                    .Target[0],
                EQuestTarget.StandB_A_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[3]
                    .Target[0],
                EQuestTarget.ManagementDesk_Upgrade => _stageController.BuildingController.BuildingSpawnData
                    .ManagementDeskSpawner
                    .Target[0],
                EQuestTarget.ManagementDesk_GetMoney => _stageController.BuildingController.BuildingSpawnData
                    .ManagementDeskSpawner
                    .Target[1],
                EQuestTarget.WareHouse_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .WareHouseSpawner
                    .Target[0],
                EQuestTarget.WareHouse_Upgrade => _stageController.BuildingController.BuildingSpawnData
                    .WareHouseSpawner
                    .Target[1],
                EQuestTarget.DeliveryLodging_Unlock => _stageController.BuildingController.BuildingSpawnData
                    .DeliveryLodgingSpawner
                    .Target[0],
                EQuestTarget.DeliveryLodging_Upgrade => _stageController.BuildingController.BuildingSpawnData
                    .DeliveryLodgingSpawner
                    .Target[1],
                EQuestTarget.ProductA_A_Sell => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[0]
                    .Target[1],
                EQuestTarget.ProductA_B_Sell => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[1]
                    .Target[1],
                EQuestTarget.ProductA_C_Sell => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[2]
                    .Target[1],
                EQuestTarget.ProductB_A_Sell => _stageController.BuildingController.BuildingSpawnData
                    .StandSpawner[3]
                    .Target[1],
                EQuestTarget.Temp_ManagementDesk => _stageController.BuildingController.BuildingSpawnData
                    .ManagementDeskSpawner
                    .Target[2],
                _ => null
            };

            if (target != null) ObjectTrackerManager.Instance.StartTargetTracking(target);
        }

        private void StopTargetTracking()
        {
            ObjectTrackerManager.Instance.StopTargetTracking();
        }

        private void HandleOnUpdateCurrentQuestProgress(EQuestType1 questType1, string questType2, int value)
        {
            var currentStageIndex = GameManager.Instance.ES3Saver.CurrentStageLevel;
            var currentListIndex = GameManager.Instance.ES3Saver.CurrentListQuestIndex;
            
            if (!(currentListIndex + 1 > _parsedStageQuestDatas[currentStageIndex].ParsedListQuestDatas.Count))
            {
                EQuestType2? parsedQuestType = ParserModule.ParseStringToEnum<EQuestType2>(questType2);

                if (parsedQuestType != null)
                {
                    var questUpdated = false;
                    ParsedStageQuestData targetStageQuestData = _parsedStageQuestDatas[GameManager.Instance.ES3Saver.CurrentStageLevel];
                
                    List<int> targetQuestIndex = targetStageQuestData.ParsedListQuestDatas[GameManager.Instance.ES3Saver.CurrentListQuestIndex].ParsedQuestDataIndex;

                    IEnumerable<ParsedQuestData> targetQuests = targetQuestIndex
                        .Select(index => targetStageQuestData.ParsedQuestDatas[index])
                        .Where(t => t.QuestType1 == questType1 && t.QuestType2 == parsedQuestType);
                    
                    foreach (var quest in targetQuests)
                    {
                        quest.CurrentGoal += value;

                        if (quest.CurrentGoal >= quest.MaxGoal)
                        {
                            quest.CurrentGoal = quest.MaxGoal;
                            UIManager.Instance.UI_Panel_Main.OnInactivateUIByCurrentTutorialIndex(quest.QuestIndex);
                        }

                        // 저장된 퀘스트 진행 상태 업데이트
                        GameManager.Instance.ES3Saver.QuestCurrentGoalCounts[GameManager.Instance.ES3Saver.CurrentStageLevel][quest.QuestIndex] = quest.CurrentGoal;

                        questUpdated = true;
                    }

                    if (questUpdated)
                    {
                        UpdateUI();
                    }
                }
            }
        }

        public void MarkQuestAsCleared(int questIndex)
        {
            var currentStageIndex = GameManager.Instance.ES3Saver.CurrentStageLevel;
            var currentListIndex = GameManager.Instance.ES3Saver.CurrentListQuestIndex;

            // 현재 리스트와 퀘스트 데이터 가져오기
            var listData = _parsedStageQuestDatas[currentStageIndex].ParsedListQuestDatas[currentListIndex];
            var questData = _parsedStageQuestDatas[currentStageIndex].ParsedQuestDatas[questIndex];
            
            AddQuestReward(questData.RewardType1, questData.RewardType1Count);
            AddQuestReward(questData.RewardType2, questData.RewardType2Count);

            // 퀘스트 상태 업데이트
            questData.IsClear = true;
            GameManager.Instance.ES3Saver.QuestClearStatuses[currentStageIndex][questIndex] = true;
            
            // UI 갱신
            UpdateUI();

            // 현재 리스트의 모든 퀘스트가 클리어되었는지 확인
            var allQuestsCleared = listData.ParsedQuestDataIndex
                .All(index => GameManager.Instance.ES3Saver.QuestClearStatuses[currentStageIndex]
                    .TryGetValue(index, out var isClear) && isClear);
            
            if (allQuestsCleared)
            {
                _uiPanelQuest.MainQuest.EnableRewardButton();
            }
        }

        public void GetNextQuest()
        {
            var currentStageIndex = GameManager.Instance.ES3Saver.CurrentStageLevel;
            var currentListIndex = GameManager.Instance.ES3Saver.CurrentListQuestIndex;
            var listData = _parsedStageQuestDatas[currentStageIndex].ParsedListQuestDatas[currentListIndex];
            
            _uiPanelReward.Activate(listData.ListRewardType, listData.ListRewardCount);
        }

        public void CheckNextQuestData()
        {
            var currentStageIndex = GameManager.Instance.ES3Saver.CurrentStageLevel;
            var currentListIndex = GameManager.Instance.ES3Saver.CurrentListQuestIndex;
            
            if (currentListIndex + 1 >= _parsedStageQuestDatas[currentStageIndex].ParsedListQuestDatas.Count)
            {
                GameManager.Instance.ES3Saver.ListQuestClearStatuses[currentStageIndex][currentListIndex] = true;
                GameManager.Instance.ES3Saver.CurrentListQuestIndex++;
                
                _uiPanelQuest.MainQuest.gameObject.SetActive(false);
                Debug.Log("모든 스테이지 퀘스트가 완료되었습니다.");
            }
            else
            {
                GameManager.Instance.ES3Saver.ListQuestClearStatuses[currentStageIndex][currentListIndex] = true;
                GameManager.Instance.ES3Saver.CurrentListQuestIndex++;

                // 다음 퀘스트 초기화
                InitializeNextQuests(currentStageIndex);   
            }

            UpdateUI();
        }

        private void InitializeNextQuests(int stageIndex)
        {
            var currentListIndex = GameManager.Instance.ES3Saver.CurrentListQuestIndex;

            if (currentListIndex >= _parsedStageQuestDatas[stageIndex].ParsedListQuestDatas.Count) return;

            var nextQuestIndices = _parsedStageQuestDatas[stageIndex].ParsedListQuestDatas[currentListIndex].ParsedQuestDataIndex;

            foreach (var questIndex in nextQuestIndices)
            {
                var questData = _parsedStageQuestDatas[stageIndex].ParsedQuestDatas[questIndex];
                if (questData.QuestType1 is EQuestType1.LevelUpOption1 or EQuestType1.Build)
                {
                    questData.CurrentGoal = SetLevelUpOption1Goal(questData);
                }
            }
        }
        
        private T GetOrAdd<T>(Dictionary<int, T> dictionary, int key, T defaultValue)
        {
            if (!dictionary.TryGetValue(key, out T value))
            {
                dictionary[key] = defaultValue;
                return defaultValue;
            }

            return value;
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

        public Sprite SetQuestIcon(EQuestType2 questType2)
        {
            Sprite questIcon = null;

            if (questType2 == EQuestType2.KitchenA_A) questIcon = DataManager.Instance.SpriteDatas[50];
            else if (questType2 == EQuestType2.KitchenA_B) questIcon = DataManager.Instance.SpriteDatas[50];
            else if (questType2 == EQuestType2.KitchenA_C) questIcon = DataManager.Instance.SpriteDatas[50];
            else if (questType2 == EQuestType2.KitchenB_A) questIcon = DataManager.Instance.SpriteDatas[51];
            else if (questType2 == EQuestType2.Material_A) questIcon = DataManager.Instance.SpriteDatas[5];
            else if (questType2 == EQuestType2.Material_B) questIcon = DataManager.Instance.SpriteDatas[6];
            else if (questType2 == EQuestType2.Material_C) questIcon = DataManager.Instance.SpriteDatas[7];
            else if (questType2 == EQuestType2.ProductA_A) questIcon = DataManager.Instance.SpriteDatas[8];
            else if (questType2 == EQuestType2.ProductA_B) questIcon = DataManager.Instance.SpriteDatas[9];
            else if (questType2 == EQuestType2.ProductA_C) questIcon = DataManager.Instance.SpriteDatas[10];
            else if (questType2 == EQuestType2.ProductB_A) questIcon = DataManager.Instance.SpriteDatas[11];
            else if (questType2 == EQuestType2.StandA_A) questIcon = DataManager.Instance.SpriteDatas[52];
            else if (questType2 == EQuestType2.StandA_B) questIcon = DataManager.Instance.SpriteDatas[52];
            else if (questType2 == EQuestType2.StandA_C) questIcon = DataManager.Instance.SpriteDatas[52];
            else if (questType2 == EQuestType2.StandB_A) questIcon = DataManager.Instance.SpriteDatas[52];
            else if (questType2 == EQuestType2.HuntingZone_A) questIcon = DataManager.Instance.SpriteDatas[12];
            else if (questType2 == EQuestType2.HuntingZone_B) questIcon = DataManager.Instance.SpriteDatas[13];
            else if (questType2 == EQuestType2.WareHouse) questIcon = DataManager.Instance.SpriteDatas[46];
            else if (questType2 == EQuestType2.ManagementDesk) questIcon = DataManager.Instance.SpriteDatas[38];
            else if (questType2 == EQuestType2.DeliveryLodging) questIcon = DataManager.Instance.SpriteDatas[42];
            else if (questType2 == EQuestType2.Money) questIcon = DataManager.Instance.SpriteDatas[0];
            else if (questType2 == EQuestType2.Zone_Open) questIcon = DataManager.Instance.SpriteDatas[67];
            else if (questType2 == EQuestType2.HuntingZone_C) questIcon = DataManager.Instance.SpriteDatas[14];
            else if (questType2 == EQuestType2.Quest) questIcon = DataManager.Instance.SpriteDatas[57];

            return questIcon;
        }
        
        private Sprite SetRewardIcon(ECurrencyType rewardType)
        {
            return DataManager.Instance.GetCurrencyIcon(rewardType);
        }
    }
}