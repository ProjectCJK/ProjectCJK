using System.Collections.Generic;
using Modules.DesignPatterns.Singletons;
using Units.Stages.Managers;
using Units.Stages.Units.Items.Enums;

namespace Managers
{
    public struct StageData
    {
        public readonly int StageLevel;
        public readonly int RequiredLevel;
        public readonly int UnlockCost;
        public readonly int RevenueMultiplier;

        public StageData(int stageLevel, int requiredLevel, int unlockCost, int revenueMultiplier)
        {
            StageLevel = stageLevel;
            RequiredLevel = requiredLevel;
            UnlockCost = unlockCost;
            RevenueMultiplier = revenueMultiplier;
        }
    }
    
    public class StageManager : Singleton<StageManager>
    {
        private string[,] _stageData;
        private List<StageData> _stageDataList = new();
        
        public void RegisterReference()
        {
            _stageData = DataManager.Instance.StageData.GetData();
            _stageDataList = new List<StageData>();
            
            ExtractStageData();
            
            UIManager.Instance.UI_Panel_Main.UI_Panel_StageMap.RegisterReference(_stageDataList, ChangeStage);
            UIManager.Instance.UI_Panel_Main.UI_Panel_MainButtons.UI_Button_StageMap.onClick.RemoveAllListeners();
            UIManager.Instance.UI_Panel_Main.UI_Panel_MainButtons.UI_Button_StageMap.onClick.AddListener(() => UIManager.Instance.UI_Panel_Main.UI_Panel_StageMap.Activate());
        }

        private void ExtractStageData()
        {
            for (var i = 2; i < _stageData.GetLength(0); i++)
            {
                var stageLevel = int.Parse(_stageData[i, 2]);
                var requiredLevel = int.Parse(_stageData[i, 4]);
                var unlockCost = int.Parse(_stageData[i, 5]);
                var revenueMultiplier = int.Parse(_stageData[i, 6]);
                
                _stageDataList.Add(new StageData(stageLevel, requiredLevel, unlockCost, revenueMultiplier));
            }
        }
        
        private void ChangeStage(int index, int cost)
        {
            GameManager.Instance.ES3Saver.CurrentStageLevel = index;
            GameManager.Instance.ES3Saver.ResetStageData();
            CurrencyManager.Instance.RemoveCurrency(ECurrencyType.Gold, cost);
            LoadingSceneManager.Instance.LoadSceneWithLoadingScreen(ESceneName.MainScene);
        }
    }
}