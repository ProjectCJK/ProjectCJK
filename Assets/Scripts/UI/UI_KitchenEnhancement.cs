using Interfaces;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeKitchenOption1Level;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption1;
        
        [Header("빌딩 쿠키 강화")]
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption2Level;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenOption2Value2;
        [SerializeField] private TextMeshProUGUI _text_NextKitchenOption2Value;
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeKitchenOption2Level;
        [SerializeField] private Button _button_RequiredGoldToUpgradeKitchenOption2;

        public void Initialize(
            string kitchenName,
            Sprite kitchenProductSprite,
            string kitchenProductName,
            int currentKitchenOption1Value,
            float currentKitchenOption2Value,
            int currentKitchenLevel,
            int currentKitchenOption1Level,
            int nextKitchenOption1Value,
            int requiredGoldToUpgradeKitchenOption1Level,
            int currentKitchenOption2Level,
            float nextKitchenOption2Value,
            int requiredGoldToUpgradeOption2Level)
        {
            _text_Kitchen.text = kitchenName;
            _image_KitchenProduct.sprite = kitchenProductSprite;
            _text_KitchenProduct.text = kitchenProductName;
            _text_CurrentKitchenOption1Value.text = $"{currentKitchenOption1Value}";
            _text_CurrentKitchenOption2Value.text = $"{currentKitchenOption2Value}";
            _text_CurrentKitchenLevel.text = "Lv. " + currentKitchenLevel;

            _text_CurrentKitchenOption1Level.text = $"Lv. {currentKitchenOption1Level}";
            _text_CurrentKitchenOption1Value2.text = $"{currentKitchenOption1Value}";
            _text_NextKitchenOption1Value.text = $"{nextKitchenOption1Value}";
            _text_RequiredGoldToUpgradeKitchenOption1Level.text = $"{requiredGoldToUpgradeKitchenOption1Level}";

            _text_CurrentKitchenOption2Level.text = "Lv. " + currentKitchenOption2Level;
            _text_CurrentKitchenOption2Value2.text = $"{currentKitchenOption2Value}";
            _text_NextKitchenOption2Value.text = $"{nextKitchenOption2Value}";
            _text_RequiredGoldToUpgradeKitchenOption2Level.text = $"{requiredGoldToUpgradeOption2Level}";
        }
    }
}