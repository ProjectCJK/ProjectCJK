using System;
using System.Collections.Generic;
using System.Linq;
using Modules.DesignPatterns.Singletons;
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
        ProductB_A,
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
        Zone_Open,
        HuntingZone_C
    }

    public enum EQuestTarget
    {
        HuntingZone_A_Unlock,
        HuntingZone_A_Hunt,
        HuntingZone_B_Unlock,
        HuntingZone_B_Hunt,
        HuntingZone_C_Unlock,
        HuntingZone_C_Hunt,
        Kitchen_A_Unlock,
        Kitchen_A_Product,
        Kitchen_A_Upgrade,
        Kitchen_B_Unlock,
        Kitchen_B_Product,
        Kitchen_B_Upgrade,
        Kitchen_C_Unlock,
        Kitchen_C_Product,
        Kitchen_C_Upgrade,
        Kitchen_D_Unlock,
        Kitchen_D_Product,
        Kitchen_D_Upgrade,
        Stand_A_Unlock,
        Stand_B_Unlock,
        Stand_C_Unlock,
        Stand_D_Unlock,
        ManagementDesk_Upgrade,
        ManagementDesk_Sell,
        ManagementDesk_GetMoney,
        WareHouse_Unlock,
        WareHouse_Upgrade,
        DeliveryLodging_Unlock,
        DeliveryLodging_Upgrade
    }
    
    [Serializable]
    public class ParseQuestData
    {
        public int StageIndex;
        public int ListIndex;
        public int QuestIndex;
        public string Quest;
        public EQuestType1 QuestType1;
        public EQuestType2 QuestType2;
        public int Goal;
        public ECurrencyType RewardType1;
        public int RewardType1Count;
        public ECurrencyType RewardType2;
        public int RewardType2Count;
        public ECurrencyType ListRewardType;
        public int ListRewardCount;
        public EQuestTarget TrackingTarget;
    }

    [Serializable]
    public struct QuestDataBundle
    {
        public Sprite QuestIcon;
        public int ClearedCount;
        public int TotalCount;
        public float ProgressRatio;
        public int RewardCount;
        public string ThumbnailDescription;
        public int ThumbnailCurrentGoal;
        public int ThumbnailMaxGoal;
        public List<UIQuestInfoItem> QuestInfoItems;
        public Action AdvanceToNextQuestAction;
        public Sprite RewardSprite;
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
        public string TrackingTarget { get; set; }
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
        
        private StageController _stageController;

        public void RegisterReference(StageController stageController)
        {
            _stageController = stageController;
            
            _uiPanelQuest = UIManager.Instance.UI_Panel_Main.UI_Panel_Quest;
            _gameData = DataManager.Instance.QuestData.GetData();
            CurrentQuestSubIndex = GameManager.Instance.ES3Saver.CurrentQuestSubIndex;
            OnUpdateCurrentQuestProgress += HandleOnUpdateCurrentQuestProgress;
            
            Debug.Log("QuestManager: RegisterReference completed.");
        }

        public void Initialize()
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
                            Reward2Count = int.Parse(questData[i][12]),
                            TrackingTarget = questData[i][15]
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
            if (GameManager.Instance.ES3Saver.CurrentQuestSubIndex >= _maxSubIndexForStage)
            {
                _uiPanelQuest.UpdateStageLastQuestPanel();
            }
            else
            {
             KeyValuePair<int, Data> smallestUnclearedQuest = _questData.Datas
                .Where(kvp => !IsQuestClear[kvp.Key])
                .OrderBy(kvp => kvp.Key)
                .FirstOrDefault();

            string thumbnailDescription = null;
            var thumbnailCurrentGoal = 0;
            var thumbnailMaxGoal = 0;
            Sprite questIcon = null;
            
            if (smallestUnclearedQuest.Value != null)
            {
                questIcon = SetQuestIcon(smallestUnclearedQuest.Value.QuestType2);
                thumbnailDescription = smallestUnclearedQuest.Value.Description;
                thumbnailCurrentGoal = smallestUnclearedQuest.Value.CurrentTargetGoal;
                thumbnailMaxGoal = smallestUnclearedQuest.Value.MaxTargetGoal;
                
                // 현재 퀘스트 완료 여부 확인
                if (thumbnailCurrentGoal >= thumbnailMaxGoal)
                {
                    // 퀘스트가 완료된 경우 추적 멈춤
                    StopTargetTracking();
                }
                else
                {
                    // 퀘스트가 완료되지 않은 경우 추적 시작
                    StartTrackingCurrentQuestTarget(smallestUnclearedQuest.Value);
                }
            }
      
            var clearedCount = IsQuestClear.Values.Count(cleared => cleared);
            var totalCount = IsQuestClear.Count;
            var progressRatio = (float)clearedCount / totalCount;

            List<UIQuestInfoItem> questInfoItems = _questData.Datas
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => new UIQuestInfoItem
                {
                    QuestIconImage = SetQuestIcon(kvp.Value.QuestType2),
                    QuestDescriptionText = kvp.Value.Description,
                    Reward1IconImage = SetRewardIcon(kvp.Value.Reward1Type),
                    Reward2IconImage = SetRewardIcon(kvp.Value.Reward2Type),
                    Reward1CountText = kvp.Value.Reward1Count.ToString(),
                    Reward2CountText = kvp.Value.Reward2Count.ToString(),
                    QuestProgressText = $"{kvp.Value.CurrentTargetGoal} / {kvp.Value.MaxTargetGoal}",
                    CurrentProgressCount = kvp.Value.CurrentTargetGoal,
                    MaxProgressCount = kvp.Value.MaxTargetGoal
                }).ToList();

            Sprite rewardSprite = null;

            if (_questData.ListRewardType != null) rewardSprite = DataManager.Instance.GetCurrencyIcon(_questData.ListRewardType.Value);
            
            var questDataBundle = new QuestDataBundle
            {
                QuestIcon = questIcon,
                ClearedCount = clearedCount,
                TotalCount = totalCount,
                ProgressRatio = progressRatio,
                RewardCount = _questData.ListRewardCount,
                ThumbnailDescription = thumbnailDescription,
                ThumbnailCurrentGoal = thumbnailCurrentGoal,
                ThumbnailMaxGoal = thumbnailMaxGoal,
                QuestInfoItems = questInfoItems,
                RewardSprite = rewardSprite,
                AdvanceToNextQuestAction = AdvanceToNextQuest
            };

            _uiPanelQuest.UpdateQuestPanel(questDataBundle);
            }
        }

        private void StartTrackingCurrentQuestTarget(Data data)
        {
            if (ObjectTrackerManager.Instance.IsTracking) return;
            
            EQuestTarget? trackingTarget = ParserModule.ParseStringToEnum<EQuestTarget>(data.TrackingTarget);

            if (trackingTarget == null) return;

            Transform target = null;

            switch (trackingTarget)
            {
                case EQuestTarget.HuntingZone_A_Unlock:
                    target = _stageController.HuntingZoneController.HuntingZoneSpawnData.HuntingZoneSpawners[0].Target[0];
                    break;
                case EQuestTarget.HuntingZone_A_Hunt:
                    target = _stageController.HuntingZoneController.HuntingZoneSpawnData.HuntingZoneSpawners[0].Target[1];
                    break;
                case EQuestTarget.HuntingZone_B_Unlock:
                    target = _stageController.HuntingZoneController.HuntingZoneSpawnData.HuntingZoneSpawners[1].Target[0];
                    break;
                case EQuestTarget.HuntingZone_B_Hunt:
                    target = _stageController.HuntingZoneController.HuntingZoneSpawnData.HuntingZoneSpawners[1].Target[1];
                    break;
                case EQuestTarget.HuntingZone_C_Unlock:
                    target = _stageController.HuntingZoneController.HuntingZoneSpawnData.HuntingZoneSpawners[2].Target[0];
                    break;
                case EQuestTarget.HuntingZone_C_Hunt:
                    target = _stageController.HuntingZoneController.HuntingZoneSpawnData.HuntingZoneSpawners[2].Target[1];
                    break;
                case EQuestTarget.Kitchen_A_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[0].Target[0];
                    break;
                case EQuestTarget.Kitchen_A_Product:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[0].Target[1];
                    break;
                case EQuestTarget.Kitchen_A_Upgrade:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[0].Target[2];
                    break;
                case EQuestTarget.Kitchen_B_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[1].Target[0];
                    break;
                case EQuestTarget.Kitchen_B_Product:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[1].Target[1];
                    break;
                case EQuestTarget.Kitchen_B_Upgrade:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[1].Target[2];
                    break;
                case EQuestTarget.Kitchen_C_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[2].Target[0];
                    break;
                case EQuestTarget.Kitchen_C_Product:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[2].Target[1];
                    break;
                case EQuestTarget.Kitchen_C_Upgrade:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[2].Target[2];
                    break;
                case EQuestTarget.Kitchen_D_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[3].Target[0];
                    break;
                case EQuestTarget.Kitchen_D_Product:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[3].Target[1];
                    break;
                case EQuestTarget.Kitchen_D_Upgrade:
                    target = _stageController.BuildingController.BuildingSpawnData.KitchenSpawner[3].Target[2];
                    break;
                case EQuestTarget.Stand_A_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.StandSpawner[0].Target[0];
                    break;
                case EQuestTarget.Stand_B_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.StandSpawner[1].Target[0];
                    break;
                case EQuestTarget.Stand_C_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.StandSpawner[2].Target[0];
                    break;
                case EQuestTarget.Stand_D_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.StandSpawner[3].Target[0];
                    break;
                case EQuestTarget.ManagementDesk_Upgrade:
                    target = _stageController.BuildingController.BuildingSpawnData.ManagementDeskSpawner.Target[0];
                    break;
                case EQuestTarget.ManagementDesk_Sell:
                    target = _stageController.BuildingController.BuildingSpawnData.ManagementDeskSpawner.Target[1];
                    break;
                case EQuestTarget.ManagementDesk_GetMoney:
                    target = _stageController.BuildingController.BuildingSpawnData.ManagementDeskSpawner.Target[2];
                    break;
                case EQuestTarget.WareHouse_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.WareHouseSpawner.Target[0];
                    break;
                case EQuestTarget.WareHouse_Upgrade:
                    target = _stageController.BuildingController.BuildingSpawnData.WareHouseSpawner.Target[1];
                    break;
                case EQuestTarget.DeliveryLodging_Unlock:
                    target = _stageController.BuildingController.BuildingSpawnData.DeliveryLodgingSpawner.Target[0];
                    break;
                case EQuestTarget.DeliveryLodging_Upgrade:
                    target = _stageController.BuildingController.BuildingSpawnData.DeliveryLodgingSpawner.Target[1];
                    break;
            }
            
            if (target != null) ObjectTrackerManager.Instance.StartTargetTracking(target);
        }

        private void StopTargetTracking()
        {
            ObjectTrackerManager.Instance.StopTargetTracking();
        }

        private void HandleOnUpdateCurrentQuestProgress(EQuestType1 questType1, string questType2, int value)
        {
            EQuestType2? parsedQuestType = ParserModule.ParseStringToEnum<EQuestType2>(questType2);

            if (parsedQuestType != null)
            {
                var questUpdated = false;

                foreach (KeyValuePair<int, Data> kvp in _questData.Datas.Where(kvp => kvp.Value.QuestType1 == questType1.ToString() && kvp.Value.QuestType2 == questType2).Where(kvp => kvp.Value.CurrentTargetGoal < kvp.Value.MaxTargetGoal))
                {
                    kvp.Value.CurrentTargetGoal += value;

                    if (kvp.Value.CurrentTargetGoal >= kvp.Value.MaxTargetGoal)
                    {
                        kvp.Value.CurrentTargetGoal = kvp.Value.MaxTargetGoal;
                    }
                            
                    GameManager.Instance.ES3Saver.QuestProgress[kvp.Key] = kvp.Value.CurrentTargetGoal;
                            
                    questUpdated = true;
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

                Initialize();
                UpdateUI();
            }
            else
            {
                if (_questData.ListRewardType != null)
                {
                    CurrencyManager.Instance.AddCurrency(_questData.ListRewardType.Value, _questData.ListRewardCount);
                }
                
                GameManager.Instance.ES3Saver.CurrentQuestSubIndex = _maxSubIndexForStage;
                
                UpdateUI();
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

        public Sprite SetQuestIcon(string questType2)
        {
            EQuestType2? parsedQuestType = ParserModule.ParseStringToEnum<EQuestType2>(questType2);
            Sprite questIcon = null;

            if (parsedQuestType != null)
            {
                switch (parsedQuestType.Value)
                {
                    case EQuestType2.Kitchen_A:
                        questIcon = DataManager.Instance.SpriteDatas[50];
                        break;
                    case EQuestType2.Kitchen_B:
                        questIcon = DataManager.Instance.SpriteDatas[50];
                        break;
                    case EQuestType2.Kitchen_C:
                        questIcon = DataManager.Instance.SpriteDatas[50];
                        break;
                    case EQuestType2.Kitchen_D:
                        questIcon = DataManager.Instance.SpriteDatas[51];
                        break;
                    case EQuestType2.Material_A:
                        questIcon = DataManager.Instance.SpriteDatas[5];
                        break;
                    case EQuestType2.Material_B:
                        questIcon = DataManager.Instance.SpriteDatas[6];
                        break;
                    case EQuestType2.Material_C:
                        questIcon = DataManager.Instance.SpriteDatas[7];
                        break;
                    case EQuestType2.ProductA_A:
                        questIcon = DataManager.Instance.SpriteDatas[8];
                        break;
                    case EQuestType2.ProductA_B:
                        questIcon = DataManager.Instance.SpriteDatas[9];
                        break;
                    case EQuestType2.ProductA_C:
                        questIcon = DataManager.Instance.SpriteDatas[10];
                        break;
                    case EQuestType2.ProductB_A:
                        questIcon = DataManager.Instance.SpriteDatas[11];
                        break;
                    case EQuestType2.Stand_A:
                        questIcon = DataManager.Instance.SpriteDatas[52];
                        break;
                    case EQuestType2.Stand_B:
                        questIcon = DataManager.Instance.SpriteDatas[52];
                        break;
                    case EQuestType2.Stand_C:
                        questIcon = DataManager.Instance.SpriteDatas[52];
                        break;
                    case EQuestType2.Stand_D:
                        questIcon = DataManager.Instance.SpriteDatas[52];
                        break;
                    case EQuestType2.HuntingZone_A:
                        questIcon = DataManager.Instance.SpriteDatas[12];
                        break;
                    case EQuestType2.HuntingZone_B:
                        questIcon = DataManager.Instance.SpriteDatas[13];
                        break;
                    case EQuestType2.WareHouse:
                        questIcon = DataManager.Instance.SpriteDatas[46];
                        break;
                    case EQuestType2.ManagementDesk:
                        questIcon = DataManager.Instance.SpriteDatas[38];
                        break;
                    case EQuestType2.DeliveryLodging:
                        questIcon = DataManager.Instance.SpriteDatas[42];
                        break;
                    case EQuestType2.Money:
                        questIcon = DataManager.Instance.SpriteDatas[0];
                        break;
                    case EQuestType2.Zone_Open:
                        questIcon = DataManager.Instance.SpriteDatas[67];
                        break;
                    case EQuestType2.HuntingZone_C:
                        questIcon = DataManager.Instance.SpriteDatas[14];
                        break;
                }   
            }
            
            return questIcon;
        }
        
        private Sprite SetRewardIcon(string rewardType)
        {
            ECurrencyType? parsedRewardType = ParserModule.ParseStringToEnum<ECurrencyType>(rewardType);
            Sprite rewardIcon = null;

            if (parsedRewardType != null) rewardIcon = DataManager.Instance.GetCurrencyIcon(parsedRewardType.Value);
            
            return rewardIcon;
        }
    }
}