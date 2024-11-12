using System;
using Managers;
using TMPro;
using UI.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    using System;
    using TMPro;
    using UnityEngine;

    namespace UI
    {
        [Serializable]
        public class UIBuildingEnhancementData
        {
            public string PanelTitle { get; set; }
            public Sprite Slot1Icon { get; set; }
            public Sprite Slot2Icon { get; set; }
            public Sprite Slot3Icon { get; set; }
            public string Slot1Title { get; set; }
            public string Slot2Title { get; set; }
            public string Slot3Title { get; set; }
            public string Slot1Category1Title { get; set; }
            public string Slot1Category2Title { get; set; }
            public int Option1TextIconIndex { get; set; }
            public int Option2TextIconIndex { get; set; }
            public int RequiredGoldForUpgradeOption1IconIndex { get; set; }
            public int RequiredRedGemForUpgradeOption2IconIndex { get; set; }
            public int RequiredBuildingLevelToUpgradeOption2LevelIndex { get; set; }
            public float CurrentBuildingOption1Value { get; set; }
            public float CurrentBuildingOption2Value { get; set; }
            public float NextBuildingOption1Value { get; set; }
            public float NextBuildingOption2Value { get; set; }
            public int CurrentBuildingLevel { get; set; }
            public int CurrentBuildingOption1Level { get; set; }
            public int CurrentBuildingOption2Level { get; set; }
            public int MaxBuildingOption1Level { get; set; }
            public int MaxBuildingOption2Level { get; set; }
            public int RequiredGoldToUpgradeOption1Level { get; set; }
            public int RequiredRedGemToUpgradeOption2Level { get; set; }
            public int RequiredBuildingLevelToUpgradeOption2Level { get; set; }
            public Action OnClickUpgradeButtonForBuildingOption1 { get; set; }
            public Action OnClickUpgradeButtonForBuildingOption2 { get; set; }
        }
    }

    
    public class UI_Panel_BuildingEnhancement : MonoBehaviour
    {
        [Header("=== 강화 패널 타이틀 ===")]
        [SerializeField] private Button _background;
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

        [Header("=== 슬롯 3 ===")]
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

        public void Activate(UIBuildingEnhancementData data)
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

            _text_Title.text = data.PanelTitle;
            
            _image_Slot1_Icon.sprite = data.Slot1Icon;
            _image_Slot2_Icon.sprite = data.Slot2Icon;
            _image_Slot3_Icon.sprite = data.Slot3Icon;
            
            _text_Slot1_Title.text = data.Slot1Title;
            _text_Slot2_Title.text = data.Slot2Title;
            _text_Slot3_Title.text = data.Slot3Title;
            
            _text_Slot1_Category1.text = data.Slot1Category1Title;
            _text_Slot1_Category2.text = data.Slot1Category2Title;

            _text_Slot1_Category1Value.text = $"<sprite={data.Option1TextIconIndex}> {data.CurrentBuildingOption1Value}";
            
            _text_Slot1_Category2Value.text = $"<sprite={data.Option2TextIconIndex}> {data.CurrentBuildingOption2Value}";

            _text_Slot1_Level.text = $"Lv. {data.CurrentBuildingLevel}";
            _text_Slot2_Level.text = $"Lv. {data.CurrentBuildingOption1Level}";
            _text_Slot3_Level.text = $"Lv. {data.CurrentBuildingOption2Level}";

            _text_Slot2_CurrentValue.text = $"<sprite={data.Option1TextIconIndex}> {data.CurrentBuildingOption1Value}";
            _text_Slot3_CurrentValue.text = $"<sprite={data.Option2TextIconIndex}> {data.CurrentBuildingOption2Value}";
            
            _text_Slot2_NextValue.text = $"<sprite={data.Option1TextIconIndex}> {data.NextBuildingOption1Value}";
            _text_Slot3_NextValue.text = $"<sprite={data.Option2TextIconIndex}> {data.NextBuildingOption2Value}";
            
            _text_Slot2_UpgradeButton_RequiredValue.text = $"<sprite={data.RequiredGoldForUpgradeOption1IconIndex}> {data.RequiredGoldToUpgradeOption1Level}";
            _text_Slot3_UpgradeButton_RequiredValue.text = $"<sprite={data.RequiredRedGemForUpgradeOption2IconIndex}> {data.RequiredRedGemToUpgradeOption2Level}";

            _text_Slot2_NotEnoughButton_RequiredValue.text = $"<sprite={data.RequiredGoldForUpgradeOption1IconIndex}> {data.RequiredGoldToUpgradeOption1Level}";
            _text_Slot3_NotEnoughButton_RequiredValue.text = $"<sprite={data.RequiredRedGemForUpgradeOption2IconIndex}> {data.RequiredRedGemToUpgradeOption2Level}";
            
            _text_Slot3_NotEnoughLevelButton_RequiredValue.text = $"<sprite={data.RequiredBuildingLevelToUpgradeOption2LevelIndex}>\nRequired\nBuilding Level {data.RequiredBuildingLevelToUpgradeOption2Level}";

            if (data.CurrentBuildingOption1Level >= data.MaxBuildingOption1Level)
            {
                _button_Slot2_MaxLevelButton.gameObject.SetActive(true);
            }
            else
            {
                if (CurrencyManager.Instance.Gold >= data.RequiredGoldToUpgradeOption1Level)
                {
                    _button_Slot2_UpgradeButton.gameObject.SetActive(true);
                }
                else
                {
                    _button_Slot2_NotEnoughButton.gameObject.SetActive(true);
                }
            }

            if (data.CurrentBuildingOption2Level >= data.MaxBuildingOption2Level)
            {
                _button_Slot3_MaxLevelButton.gameObject.SetActive(true);
            }
            else
            {
                if (data.RequiredBuildingLevelToUpgradeOption2Level <= data.CurrentBuildingLevel)
                {
                    if (CurrencyManager.Instance.RedGem >= data.RequiredRedGemToUpgradeOption2Level)
                    {
                        _button_Slot3_UpgradeButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        _button_Slot3_NotEnoughButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _button_Slot3_NotEnoughLevelButton.gameObject.SetActive(true);
                }
            }

            _button_Slot2_UpgradeButton.onClick.AddListener(() => data.OnClickUpgradeButtonForBuildingOption1?.Invoke());
            _button_Slot3_UpgradeButton.onClick.AddListener(() => data.OnClickUpgradeButtonForBuildingOption2?.Invoke());

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