using System;
using Interfaces;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public interface IUI_KitchenEnhancement
    {
        
    }
    
    public class UI_KitchenEnhancement : MonoBehaviour, IUI_KitchenEnhancement
    {
        [Header("빌딩 정보")]
        [SerializeField] private TextMeshProUGUI _text_Kitchen;
        [SerializeField] private Image _image_KitchenProduct;
        [SerializeField] private TextMeshProUGUI _text_KitchenProduct;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption1Value;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption2Value;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenLevel;
        
        [Header("빌딩 기본 강화")]
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption1Level;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption1Value2;
        [SerializeField] private TextMeshProUGUI _text_NextKitchenOption1Value;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption1;
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeKitchenOption1;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption1_NotEnoughMoney;
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeKitchenOption1_NotEnoughMoney;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption1_MaxLevel;
        
        [Header("빌딩 쿠키 강화")]
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption2Level;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption2Value2;
        [SerializeField] private TextMeshProUGUI _text_NextKitchenOption2Value;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption2;
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeKitchenOption2;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption2_NotEnoughMoney;
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeKitchenOption2_NotEnoughMoney;
        [SerializeField] private Button _button_RequiredKitchenLevelToUpgradeKitchenOption2_NotEnoughLevel;
        [SerializeField] private TextMeshProUGUI _text_RequiredKitchenLevelToUpgradeKitchenOption2_NotEnoughLevel;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption2_MaxLevel;
        
        public void Activate(
            string kitchenName,
            Sprite kitchenProductSprite,
            string kitchenProductName,
            int currentKitchenOption1Value,
            float currentKitchenOption2Value,
            int nextKitchenOption1Value,
            float nextKitchenOption2Value,
            int currentKitchenLevel,
            int currentKitchenOption1Level,
            int currentKitchenOption2Level,
            int maxKitchenOption1Level,
            int maxKitchenOption2Level,
            int requiredGoldToUpgradeKitchenOption1,
            int requiredGoldToUpgradeKitchenOption2,
            int requiredKitchenLevelToUpgradeOption2,
            Action onClickUpgradeButtonForKitchenOption1,
            Action onClickUpgradeButtonForKitchenOption2)
        {
            _button_RequiredGoldToUpgradeKitchenOption1.gameObject.SetActive(false);
            _button_RequiredGoldToUpgradeKitchenOption1_NotEnoughMoney.gameObject.SetActive(false);
            _button_RequiredGoldToUpgradeKitchenOption1_MaxLevel.gameObject.SetActive(false);
            _button_RequiredGoldToUpgradeKitchenOption2.gameObject.SetActive(false);
            _button_RequiredGoldToUpgradeKitchenOption2_NotEnoughMoney.gameObject.SetActive(false);
            _button_RequiredKitchenLevelToUpgradeKitchenOption2_NotEnoughLevel.gameObject.SetActive(false);
            _button_RequiredGoldToUpgradeKitchenOption2_MaxLevel.gameObject.SetActive(false);
            
            _button_RequiredGoldToUpgradeKitchenOption1.onClick.RemoveAllListeners();
            _button_RequiredGoldToUpgradeKitchenOption2.onClick.RemoveAllListeners();
            
            _text_Kitchen.text = $"{kitchenName}";
            _image_KitchenProduct.sprite = kitchenProductSprite;
            _text_KitchenProduct.text = $"{kitchenProductName}";
            
            _text_CurrentKitchenOption1Value.text = $"{_text_CurrentKitchenOption1Value.text.Substring(0, _text_CurrentKitchenOption1Value.text.IndexOf(' ') + 1)}{currentKitchenOption1Value}";
            _text_CurrentKitchenOption2Value.text = $"{_text_CurrentKitchenOption2Value.text.Substring(0, _text_CurrentKitchenOption2Value.text.IndexOf(' ') + 1)}{currentKitchenOption2Value}";
            _text_CurrentKitchenLevel.text = $"{_text_CurrentKitchenLevel.text.Substring(0, _text_CurrentKitchenLevel.text.IndexOf(' ') + 1)}{currentKitchenLevel}";

            _text_CurrentKitchenOption1Level.text = $"{_text_CurrentKitchenOption1Level.text.Substring(0, _text_CurrentKitchenOption1Level.text.IndexOf(' ') + 1)}{currentKitchenOption1Level}";
            _text_CurrentKitchenOption1Value2.text = $"{_text_CurrentKitchenOption1Value2.text.Substring(0, _text_CurrentKitchenOption1Value2.text.IndexOf(' ') + 1)}{currentKitchenOption1Value}";
            _text_NextKitchenOption1Value.text = $"{_text_NextKitchenOption1Value.text.Substring(0, _text_NextKitchenOption1Value.text.IndexOf(' ') + 1)}{nextKitchenOption1Value}";
            _text_RequiredGoldToUpgradeKitchenOption1.text = $"{_text_RequiredGoldToUpgradeKitchenOption1.text.Substring(0, _text_RequiredGoldToUpgradeKitchenOption1.text.IndexOf(' ') + 1)}{requiredGoldToUpgradeKitchenOption1}";

            _text_CurrentKitchenOption2Level.text = $"{_text_CurrentKitchenOption2Level.text.Substring(0, _text_CurrentKitchenOption2Level.text.IndexOf(' ') + 1)}{currentKitchenOption2Level}";
            _text_CurrentKitchenOption2Value2.text = $"{_text_CurrentKitchenOption2Value2.text.Substring(0, _text_CurrentKitchenOption2Value2.text.IndexOf(' ') + 1)}{currentKitchenOption2Value}";
            _text_NextKitchenOption2Value.text = $"{_text_NextKitchenOption2Value.text.Substring(0, _text_NextKitchenOption2Value.text.IndexOf(' ') + 1)}{nextKitchenOption2Value}";

            if (currentKitchenOption1Level >= maxKitchenOption1Level)
            {
                _button_RequiredGoldToUpgradeKitchenOption1_MaxLevel.gameObject.SetActive(true);
            }
            else
            {
                if (CurrencyManager.Instance.Gold >= requiredGoldToUpgradeKitchenOption1)
                {
                    _text_RequiredGoldToUpgradeKitchenOption1.text = $"{_text_RequiredGoldToUpgradeKitchenOption1.text.Split(' ')[0]} {requiredGoldToUpgradeKitchenOption1}";
                    _button_RequiredGoldToUpgradeKitchenOption1.gameObject.SetActive(true);
                }
                else
                {
                    _text_RequiredGoldToUpgradeKitchenOption1_NotEnoughMoney.text = $"{_text_RequiredGoldToUpgradeKitchenOption1_NotEnoughMoney.text.Split(' ')[0]} {requiredGoldToUpgradeKitchenOption1}";
                    _button_RequiredGoldToUpgradeKitchenOption1_NotEnoughMoney.gameObject.SetActive(true);
                }
            }

            if (currentKitchenOption2Level >= maxKitchenOption2Level)
            {
                _button_RequiredGoldToUpgradeKitchenOption2_MaxLevel.gameObject.SetActive(true);
            }
            else
            {
                if (requiredKitchenLevelToUpgradeOption2 <= currentKitchenLevel)
                {
                    if (CurrencyManager.Instance.Gold >= requiredGoldToUpgradeKitchenOption2)
                    {
                        _text_RequiredGoldToUpgradeKitchenOption2.text = $"{_text_RequiredGoldToUpgradeKitchenOption2.text.Split(' ')[0]} {requiredGoldToUpgradeKitchenOption2}";
                        _button_RequiredGoldToUpgradeKitchenOption2.gameObject.SetActive(true);
                    }
                    else
                    {
                        _text_RequiredGoldToUpgradeKitchenOption2_NotEnoughMoney.text = $"{_text_RequiredGoldToUpgradeKitchenOption2_NotEnoughMoney.text.Split(' ')[0]} {requiredGoldToUpgradeKitchenOption2}";
                        _button_RequiredGoldToUpgradeKitchenOption2_NotEnoughMoney.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _button_RequiredKitchenLevelToUpgradeKitchenOption2_NotEnoughLevel.gameObject.SetActive(true);
                }
            }
            
            _button_RequiredGoldToUpgradeKitchenOption1.onClick.AddListener(() => onClickUpgradeButtonForKitchenOption1?.Invoke());
            _button_RequiredGoldToUpgradeKitchenOption2.onClick.AddListener(() => onClickUpgradeButtonForKitchenOption2?.Invoke());
            
            if (!gameObject.activeInHierarchy) gameObject.SetActive(true);
        }

        public void Inactivate()
        {
            _button_RequiredGoldToUpgradeKitchenOption1.onClick.RemoveAllListeners();
            _button_RequiredGoldToUpgradeKitchenOption2.onClick.RemoveAllListeners();
            
            if (gameObject.activeInHierarchy) gameObject.SetActive(false);
        }
    }
}