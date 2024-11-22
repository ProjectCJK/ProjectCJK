using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Level
{
    public class UI_Panel_LevelUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        
        [Space(20), SerializeField] private Image reward1Icon;
        [SerializeField] private TextMeshProUGUI reward1Count;
        [SerializeField] private Image reward2Icon;
        [SerializeField] private TextMeshProUGUI reward2Count;
        
        [Space(20), SerializeField] private Button skipButton;
        [SerializeField] private Button adButton;

        public void RegisterReference(Action action)
        {
            skipButton.onClick.AddListener(() =>
            {
                action?.Invoke();
                gameObject.SetActive(false);
            });
        }

        public void Activate(List<Tuple<ECurrencyType, int>> levelUpRewards)
        {
            levelText.text = $"{GameManager.Instance.ES3Saver.CurrentPlayerLevel}";
            reward1Icon.sprite = DataManager.Instance.GetCurrencyIcon(levelUpRewards[0].Item1);
            reward1Count.text = $"{levelUpRewards[0].Item2}";
            reward2Icon.sprite = DataManager.Instance.GetCurrencyIcon(levelUpRewards[1].Item1);
            reward2Count.text = $"{levelUpRewards[1].Item2}";
            
            gameObject.SetActive(true);
        }
    }
}