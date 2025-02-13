using System;
using System.Collections.Generic;
using Interfaces;
using Modules.DesignPatterns.Singletons;
using UI.CurrencyPanel;
using UI.Level;
using UI.LevelPanels;
using Units.Stages.UI.Level;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Managers
{
    public class LevelManager : Singleton<LevelManager>, IRegisterReference, IInitializable
    {
        private LevelView _levelView;
        private LevelModel _levelModel;
        private LevelViewModel _levelViewModel;
        
        private UI_Panel_LevelUp _ui_Panel_LevelUp;

        private int _currentLevel;
        private int _currentExp;
        private int _maxExp;
        
        private string[,] _levelData;

        private List<Tuple<ECurrencyType, int>> _levelUpRewards = new();
        
        public int CurrentLevel
        {
            get => _currentLevel;
            private set
            {
                _currentLevel = value;
                UpdateViewModel();
            }
        }
        
        public int CurrentExp
        {
            get => _currentExp;
            private set
            {
                _currentExp = value;
                UpdateViewModel();
            }
        }
        
        public int MaxExp
        {
            get => _maxExp;
            private set
            {
                _maxExp = value;
                UpdateViewModel();
            }
        }

        public void RegisterReference()
        {
            _levelView = UIManager.Instance.UI_Panel_Main.LevelView;
            _ui_Panel_LevelUp = UIManager.Instance.UI_Panel_Main.UI_Panel_LevelUp;

            // 보상 지급 콜백 등록
            _ui_Panel_LevelUp.RegisterReference(
                // 스킵 버튼: 기본 보상 지급
                () =>
                {
                    foreach (Tuple<ECurrencyType, int> levelUpReward in _levelUpRewards)
                    {
                        CurrencyManager.Instance.AddCurrency(levelUpReward.Item1, levelUpReward.Item2);
                    }
                },
                // 광고 보기 버튼: 2배 보상 지급
                () =>
                {
                    foreach (Tuple<ECurrencyType, int> levelUpReward in _levelUpRewards)
                    {
                        CurrencyManager.Instance.AddCurrency(levelUpReward.Item1, levelUpReward.Item2 * 2);
                    }

                    Debug.Log("광고 시청 완료! 2배 보상이 지급되었습니다.");
                });

            _levelData = DataManager.Instance.LevelData.GetData();

            _levelModel = new LevelModel();
            _levelViewModel = new LevelViewModel(_levelModel);
            _levelView.BindViewModel(_levelViewModel);
        }

        public void Initialize()
        {
            _currentLevel = GameManager.Instance.ES3Saver.CurrentPlayerLevel;
            _currentExp = GameManager.Instance.ES3Saver.CurrentPlayerExp;

            UpdateLevelData();
        }
        
        private void UpdateViewModel()
        {
            _levelViewModel.UpdateValues(CurrentLevel, CurrentExp, MaxExp);
        }

        public void AddExp(int value)
        {
            CurrentExp += value;

            if (CurrentExp >= MaxExp)
            {
                CurrentLevel++;
                GameManager.Instance.ES3Saver.CurrentPlayerLevel = CurrentLevel;
                
                Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_player_level {CurrentLevel}");
                CurrentExp -= MaxExp;

                _ui_Panel_LevelUp.Activate(_levelUpRewards);
                
                UpdateLevelData();
            }
            
            GameManager.Instance.ES3Saver.CurrentPlayerExp = CurrentExp;
        }
        
        private void UpdateLevelData()
        {
            _levelUpRewards = new List<Tuple<ECurrencyType, int>>();

            for (var i = 0; i < _levelData.GetLength(0); i++)
            {
                if (int.TryParse(_levelData[i, 1], out var level) && level == CurrentLevel)
                {
                    if (int.TryParse(_levelData[i, 2], out var maxExp))
                    {
                        MaxExp = maxExp;
                    }

                    var reward1TypeString = _levelData[i, 3];
                    if (Enum.TryParse(reward1TypeString, out ECurrencyType reward1Type) &&
                        int.TryParse(_levelData[i, 4], out var reward1Count))
                    {
                        _levelUpRewards.Add(new Tuple<ECurrencyType, int>(reward1Type, reward1Count));
                    }

                    var reward2TypeString = _levelData[i, 5];
                    if (Enum.TryParse(reward2TypeString, out ECurrencyType reward2Type) &&
                        int.TryParse(_levelData[i, 6], out var reward2Count))
                    {
                        _levelUpRewards.Add(new Tuple<ECurrencyType, int>(reward2Type, reward2Count));
                    }

                    break;
                }
            }

            UpdateViewModel();
        }
    }
}