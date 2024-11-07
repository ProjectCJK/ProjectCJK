using System;
using Interfaces;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public interface IUI_BuildingEnhancement
    {
        
    }
    
    public class UI_BuildingEnhancement : MonoBehaviour, IUI_BuildingEnhancement
    {
        [Header("=== 강화 패널 타이틀 ===")]
        [SerializeField] private TextMeshProUGUI _text_Title;
        
        [Header("=== 슬롯 1 ===")]
        [SerializeField] private Image _image_Slot1_Icon;
        [SerializeField] private TextMeshProUGUI _text_Slot1_Title;
        [SerializeField] private TextMeshProUGUI _text_Slot1_Category1;
        [SerializeField] private TextMeshProUGUI _text_Slot1_Category2;
        [SerializeField] private TextMeshProUGUI _text_Slot1_Category1Value;
        [SerializeField] private TextMeshProUGUI _text_Slot1_Category2Value;
        [SerializeField] private TextMeshProUGUI _text_Slot1_Level;
        
        [Header("=== 슬롯 2 ===")]
        [SerializeField] private Image _image_Slot2_Icon;
        [SerializeField] private TextMeshProUGUI _text_Slot2_Title;
        [SerializeField] private TextMeshProUGUI _text_Slot2_Level;
        [SerializeField] private TextMeshProUGUI _text_Slot2_CurrentValue;
        [SerializeField] private TextMeshProUGUI _text_Slot2_NextValue;
        [SerializeField] private Button _button_Slot2_UpgradeButton;
        [SerializeField] private TextMeshProUGUI _text_Slot2_UpgradeButton_RequiredValue;
        [SerializeField] private Button _button_Slot2_NotEnoughButton;
        [SerializeField] private TextMeshProUGUI _text_Slot2_NotEnoughButton_RequiredValue;
        [SerializeField] private Button _button_Slot2_MaxLevelButton;
        
        [Header("빌딩 쿠키 강화")]
        [SerializeField] private Image _image_Slot3_Icon;
        [SerializeField] private TextMeshProUGUI _text_Slot3_Title;
        [SerializeField] private TextMeshProUGUI _text_Slot3_Level;
        [SerializeField] private TextMeshProUGUI _text_Slot3_CurrentValue;
        [SerializeField] private TextMeshProUGUI _text_Slot3_NextValue;
        [SerializeField] private Button _button_Slot3_UpgradeButton;
        [SerializeField] private TextMeshProUGUI _text_Slot3_UpgradeButton_RequiredValue;
        [SerializeField] private Button _button_Slot3_NotEnoughButton;
        [SerializeField] private TextMeshProUGUI _text_Slot3_NotEnoughButton_RequiredValue;
        [SerializeField] private Button _button_Slot3_NotEnoughLevelButton;
        [SerializeField] private TextMeshProUGUI _text_Slot3_NotEnoughLevelButton_RequiredValue;
        [SerializeField] private Button _button_Slot3_MaxLevelButton;
        
        public void Activate(
            string buildingName,
            Sprite buildingProductSprite,
            string buildingProductName,
            float currentBuildingOption1Value,
            float currentBuildingOption2Value,
            float nextBuildingOption1Value,
            float nextBuildingOption2Value,
            int currentBuildingLevel,
            int currentBuildingOption1Level,
            int currentBuildingOption2Level,
            int maxBuildingOption1Level,
            int maxBuildingOption2Level,
            int requiredGoldToUpgradeBuildingOption1,
            int requiredGoldToUpgradeBuildingOption2,
            int requiredBuildingLevelToUpgradeOption2,
            Action onClickUpgradeButtonForBuildingOption1,
            Action onClickUpgradeButtonForBuildingOption2)
        {
            _button_Slot2_UpgradeButton.gameObject.SetActive(false);
            _button_Slot2_NotEnoughButton.gameObject.SetActive(false);
            _button_Slot2_MaxLevelButton.gameObject.SetActive(false);
            _button_Slot3_UpgradeButton.gameObject.SetActive(false);
            _button_Slot3_NotEnoughButton.gameObject.SetActive(false);
            _button_Slot3_NotEnoughLevelButton.gameObject.SetActive(false);
            _button_Slot3_MaxLevelButton.gameObject.SetActive(false);
            
            _button_Slot2_UpgradeButton.onClick.RemoveAllListeners();
            _button_Slot3_UpgradeButton.onClick.RemoveAllListeners();
            
            _text_Title.text = $"{buildingName}";
            _image_Slot1_Icon.sprite = buildingProductSprite;
            _text_Slot1_Title.text = $"{buildingProductName}";
            
            _text_Slot1_Category1Value.text = $"{_text_Slot1_Category1Value.text.Substring(0, _text_Slot1_Category1Value.text.IndexOf(' ') + 1)}{currentBuildingOption1Value}";
            _text_Slot1_Category2Value.text = $"{_text_Slot1_Category2Value.text.Substring(0, _text_Slot1_Category2Value.text.IndexOf(' ') + 1)}{currentBuildingOption2Value}";
            _text_Slot1_Level.text = $"{_text_Slot1_Level.text.Substring(0, _text_Slot1_Level.text.IndexOf(' ') + 1)}{currentBuildingLevel}";

            _text_Slot2_Level.text = $"{_text_Slot2_Level.text.Substring(0, _text_Slot2_Level.text.IndexOf(' ') + 1)}{currentBuildingOption1Level}";
            _text_Slot2_CurrentValue.text = $"{_text_Slot2_CurrentValue.text.Substring(0, _text_Slot2_CurrentValue.text.IndexOf(' ') + 1)}{currentBuildingOption1Value}";
            _text_Slot2_NextValue.text = $"{_text_Slot2_NextValue.text.Substring(0, _text_Slot2_NextValue.text.IndexOf(' ') + 1)}{nextBuildingOption1Value}";
            _text_Slot2_UpgradeButton_RequiredValue.text = $"{_text_Slot2_UpgradeButton_RequiredValue.text.Substring(0, _text_Slot2_UpgradeButton_RequiredValue.text.IndexOf(' ') + 1)}{requiredGoldToUpgradeBuildingOption1}";

            _text_Slot3_Level.text = $"{_text_Slot3_Level.text.Substring(0, _text_Slot3_Level.text.IndexOf(' ') + 1)}{currentBuildingOption2Level}";
            _text_Slot3_CurrentValue.text = $"{_text_Slot3_CurrentValue.text.Substring(0, _text_Slot3_CurrentValue.text.IndexOf(' ') + 1)}{currentBuildingOption2Value}";
            _text_Slot3_NextValue.text = $"{_text_Slot3_NextValue.text.Substring(0, _text_Slot3_NextValue.text.IndexOf(' ') + 1)}{nextBuildingOption2Value}";

            if (currentBuildingOption1Level >= maxBuildingOption1Level)
            {
                _button_Slot2_MaxLevelButton.gameObject.SetActive(true);
            }
            else
            {
                if (CurrencyManager.Instance.Gold >= requiredGoldToUpgradeBuildingOption1)
                {
                    _text_Slot2_UpgradeButton_RequiredValue.text = $"{_text_Slot2_UpgradeButton_RequiredValue.text.Split(' ')[0]} {requiredGoldToUpgradeBuildingOption1}";
                    _button_Slot2_UpgradeButton.gameObject.SetActive(true);
                }
                else
                {
                    _text_Slot2_NotEnoughButton_RequiredValue.text = $"{_text_Slot2_NotEnoughButton_RequiredValue.text.Split(' ')[0]} {requiredGoldToUpgradeBuildingOption1}";
                    _button_Slot2_NotEnoughButton.gameObject.SetActive(true);
                }
            }

            if (currentBuildingOption2Level >= maxBuildingOption2Level)
            {
                _button_Slot3_MaxLevelButton.gameObject.SetActive(true);
            }
            else
            {
                if (requiredBuildingLevelToUpgradeOption2 <= currentBuildingLevel)
                {
                    if (CurrencyManager.Instance.Gold >= requiredGoldToUpgradeBuildingOption2)
                    {
                        _text_Slot3_UpgradeButton_RequiredValue.text = $"{_text_Slot3_UpgradeButton_RequiredValue.text.Split(' ')[0]} {requiredGoldToUpgradeBuildingOption2}";
                        _button_Slot3_UpgradeButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        _text_Slot3_NotEnoughButton_RequiredValue.text = $"{_text_Slot3_NotEnoughButton_RequiredValue.text.Split(' ')[0]} {requiredGoldToUpgradeBuildingOption2}";
                        _button_Slot3_NotEnoughButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _text_Slot3_NotEnoughLevelButton_RequiredValue.text = $"Required Building Level {requiredBuildingLevelToUpgradeOption2}";
                    _button_Slot3_NotEnoughLevelButton.gameObject.SetActive(true);
                }
            }
            
            _button_Slot2_UpgradeButton.onClick.AddListener(() => onClickUpgradeButtonForBuildingOption1?.Invoke());
            _button_Slot3_UpgradeButton.onClick.AddListener(() => onClickUpgradeButtonForBuildingOption2?.Invoke());
            
            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        }

        public void Inactivate()
        {
            _button_Slot2_UpgradeButton.onClick.RemoveAllListeners();
            _button_Slot3_UpgradeButton.onClick.RemoveAllListeners();
            
            if (gameObject.activeInHierarchy) gameObject.SetActive(false);
        }
    }
}