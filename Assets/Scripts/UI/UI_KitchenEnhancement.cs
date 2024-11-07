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
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenProductPrice;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenProductTime;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenLevel;
        
        [Header("빌딩 기본 강화")]
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenProductPriceLevel;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenProductPrice2;
        [SerializeField] private TextMeshProUGUI _text_NextKitchenProductPrice;
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeProductPrice;
        
        [Header("빌딩 쿠키 강화")]
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenProductTimeLevel;
        [SerializeField] private TextMeshProUGUI _text_CurrentKitchenProductTime2;
        [SerializeField] private TextMeshProUGUI _text_NextKitchenProductTime;
        [SerializeField] private TextMeshProUGUI _text_RequiredGoldToUpgradeProductTime;
        
        public void Initialize(
            string textKitchen,
            Sprite imageKitchenProduct,
            string textKitchenProduct,
            string textCurrentKitchenProductPrice,
            string textCurrentKitchenProductTime,
            string textCurrentKitchenLevel,
            string textCurrentKitchenProductPriceLevel,
            string textCurrentKitchenProductPrice2,
            string textNextKitchenProductPrice,
            string textRequiredGoldToUpgradeProductPrice,
            string textCurrentKitchenProductTimeLevel,
            string textCurrentKitchenProductTime2,
            string textNextKitchenProductTime,
            string textRequiredGoldToUpgradeProductTime
        )
        {
            _text_Kitchen.text = textKitchen;
            _image_KitchenProduct.sprite = imageKitchenProduct;
            _text_KitchenProduct.text = textKitchenProduct;
            _text_CurrentKitchenProductPrice.text = textCurrentKitchenProductPrice;
            _text_CurrentKitchenProductTime.text = textCurrentKitchenProductTime;
            _text_CurrentKitchenLevel.text = "Lv. " + textCurrentKitchenLevel;

            _text_CurrentKitchenProductPriceLevel.text = "Lv. " + textCurrentKitchenProductPriceLevel;
            _text_CurrentKitchenProductPrice2.text = textCurrentKitchenProductPrice2;
            _text_NextKitchenProductPrice.text = textNextKitchenProductPrice;
            _text_RequiredGoldToUpgradeProductPrice.text = textRequiredGoldToUpgradeProductPrice;

            _text_CurrentKitchenProductTimeLevel.text = "Lv. " + textCurrentKitchenProductTimeLevel;
            _text_CurrentKitchenProductTime2.text = textCurrentKitchenProductTime2;
            _text_NextKitchenProductTime.text = textNextKitchenProductTime;
            _text_RequiredGoldToUpgradeProductTime.text = textRequiredGoldToUpgradeProductTime;
        }
    }
}