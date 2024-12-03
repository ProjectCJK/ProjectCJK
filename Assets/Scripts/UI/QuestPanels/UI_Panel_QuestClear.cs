using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestPanels
{
    public class UI_Panel_QuestClear : MonoBehaviour
    {
        [SerializeField] private List<Color> _colors;
        [SerializeField] private List<Sprite> _backgroundSprites;
        [SerializeField] private List<Sprite> _iconSprites;
        
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _iconBackgroundImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _rewardTypeText;
        [SerializeField] private TextMeshProUGUI _rewardCountText;
        
        [SerializeField] private Button _button;
        
        private ECurrencyType _listDataListRewardType;
        private int _listDataListRewardCount;

        private static readonly int Open = Animator.StringToHash("Open");
        
        public void RegisterReference()
        {
            _button.onClick.AddListener(OnClickPanel);
        }

        public void Activate(ECurrencyType listDataListRewardType, int listDataListRewardCount)
        {
            _listDataListRewardType = listDataListRewardType;
            _listDataListRewardCount = listDataListRewardCount;
            
            _rewardTypeText.text = $"{listDataListRewardType}";
            
            switch (listDataListRewardType)
            {
                case ECurrencyType.Diamond:
                    _iconImage.sprite = _iconSprites[0];
                    _iconImage.SetNativeSize();
                    _iconImage.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    _iconImage.transform.localPosition = new Vector3(0, 40, 0);
                    _iconBackgroundImage.color = _colors[0];
                    _backgroundImage.sprite = _backgroundSprites[0];
                    _rewardCountText.text = $"<sprite={2}> {listDataListRewardCount}";
                    break;
                case ECurrencyType.RedGem:
                    _iconImage.sprite = _iconSprites[1];
                    _iconImage.SetNativeSize();
                    _iconImage.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    _iconImage.transform.localPosition = new Vector3(0, 40, 0);
                    _iconBackgroundImage.color = _colors[1];
                    _backgroundImage.sprite = _backgroundSprites[1];
                    _rewardCountText.text = $"<sprite={4}> {listDataListRewardCount}";
                    break;
                case ECurrencyType.Gold:
                    _iconImage.sprite = _iconSprites[2];
                    _iconImage.SetNativeSize();
                    _iconImage.transform.localScale = new Vector3(2f, 2f, 2f);
                    _iconImage.transform.localPosition = new Vector3(0, 40, 0);
                    _iconBackgroundImage.color = _colors[2];
                    _backgroundImage.sprite = _backgroundSprites[2];
                    _rewardCountText.text = $"<sprite={0}> {listDataListRewardCount}";
                    break;
            }

            gameObject.SetActive(true);
        }

        private void OnClickPanel()
        {
            CurrencyManager.Instance.AddCurrency(_listDataListRewardType, _listDataListRewardCount);
            QuestManager.Instance.CheckNextQuestData();
            gameObject.SetActive(false);
        }
    }
}