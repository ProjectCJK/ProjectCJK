using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainPanels
{
    public enum EPopUpPanelType
    {
        CostumeGacha,
        CustomerRush,
        SuperHunter
    }
    public class UI_Panel_PopUp_Reward : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI costumeGachaTitle;
        [SerializeField] private TextMeshProUGUI customerRushTitle;
        [SerializeField] private TextMeshProUGUI superHunterTitle;
        
        [SerializeField] private Image costumeGachaImage;
        [SerializeField] private Image customerRushImage;
        [SerializeField] private Image superHunterImage;

        [SerializeField] private Button adButton;
        [SerializeField] private TextMeshProUGUI AdButtonText;
        
        [SerializeField] private Button diaButton;
        [SerializeField] private Button diaButton_NotEnough;
        [SerializeField] private TextMeshProUGUI diaButtonText;
        public void Activate(EPopUpPanelType popUpPanelType, int diaCount, Action onClickAdButton, Action onClickDiaButton)
        {
            adButton.onClick.RemoveAllListeners();
            diaButton.onClick.RemoveAllListeners();
            
            adButton.onClick.AddListener(() =>
            {
                AdsManager.Instance.ShowRewardedAd($"{popUpPanelType} Reward", (_, _) =>
                {
                    onClickAdButton?.Invoke();
                    gameObject.SetActive(false);
                });
      
            });
            
            diaButton.onClick.AddListener(() =>
            {
                onClickDiaButton?.Invoke();
                gameObject.SetActive(false);
            });
            
            switch (popUpPanelType)
            {
                case EPopUpPanelType.CostumeGacha:
                    costumeGachaTitle.gameObject.SetActive(true);
                    customerRushTitle.gameObject.SetActive(false);
                    superHunterTitle.gameObject.SetActive(false);
                    
                    costumeGachaImage.gameObject.SetActive(true);
                    customerRushImage.gameObject.SetActive(false);
                    superHunterImage.gameObject.SetActive(false);
                    
                    adButton.gameObject.SetActive(false);
                    
                    var buttonTrigger = GameManager.Instance.ES3Saver.Gold >= diaCount;
                    diaButton.gameObject.SetActive(buttonTrigger);
                    diaButton_NotEnough.gameObject.SetActive(!buttonTrigger);
                    
                    break;
                case EPopUpPanelType.CustomerRush:
                    costumeGachaTitle.gameObject.SetActive(false);
                    customerRushTitle.gameObject.SetActive(true);
                    superHunterTitle.gameObject.SetActive(false);
                    
                    costumeGachaImage.gameObject.SetActive(false);
                    customerRushImage.gameObject.SetActive(true);
                    superHunterImage.gameObject.SetActive(false);
                    
                    adButton.gameObject.SetActive(true);
                    diaButton.gameObject.SetActive(false);
                    diaButton_NotEnough.gameObject.SetActive(false);
                    
                    break;
                case EPopUpPanelType.SuperHunter:
                    costumeGachaTitle.gameObject.SetActive(false);
                    customerRushTitle.gameObject.SetActive(false);
                    superHunterTitle.gameObject.SetActive(true);
                    
                    costumeGachaImage.gameObject.SetActive(false);
                    customerRushImage.gameObject.SetActive(false);
                    superHunterImage.gameObject.SetActive(true);
                    
                    adButton.gameObject.SetActive(false);
                    diaButton.gameObject.SetActive(false);
                    diaButton_NotEnough.gameObject.SetActive(false);
                    
                    break;
            }
            
            AdButtonText.text = "<sprite=82> AD";
            diaButtonText.text = $"<sprite=2> {diaCount}";
            
            gameObject.SetActive(true);
        }
    }
}