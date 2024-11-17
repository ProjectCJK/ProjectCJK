using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI costumeNameText;
        [SerializeField] private List<Image> costumeTypeImage;
        [SerializeField] private TextMeshProUGUI costumeTypeText;
        
        [Space(20), SerializeField] private TextMeshProUGUI costumeLevelText;
        [SerializeField] private Image costumeImageBackgroundImage;
        [SerializeField] private Image costumeIconImage;
        
        [Space(20), SerializeField] private TextMeshProUGUI costumeDescriptionText;
        [SerializeField] private Image costumeOption1DescriptionIconImage;
        [SerializeField] private TextMeshProUGUI costumeOption1DescriptionText;
        
        [Space(20), SerializeField] private Button enhancementButton;
        [SerializeField] private Button equipButton;

        [Space(20), SerializeField] private UI_Panel_CostumeUpgrade uiPanelCostumeUpgrade;
        
        private CostumeItemData _costumeItemData;
        
        public void RegisterReference(Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache, List<CostumeItemData> currentCostumeItemData)
        {
            gameObject.SetActive(false);
            
            uiPanelCostumeUpgrade.RegisterReference(frontGroundImageCache, currentCostumeItemData);
        }

        public void Activate(CostumeItemData costumeItemData)
        {
            _costumeItemData = costumeItemData;
            
            costumeNameText.text = costumeItemData.CostumeName;

            switch (costumeItemData.CostumeGrade)
            {
                case ECostumeGrade.Common:
                    costumeTypeImage[0].gameObject.SetActive(true);
                    costumeTypeImage[1].gameObject.SetActive(false);
                    break;
                case ECostumeGrade.Rare:
                    costumeTypeImage[0].gameObject.SetActive(false);
                    costumeTypeImage[1].gameObject.SetActive(true);
                    break;
            }
            
            costumeTypeText.text = $"{costumeItemData.CostumeType}";
            costumeLevelText.text = $"Lv. {costumeItemData.CurrentLevel}";
            costumeIconImage.sprite = costumeItemData.CostumeSprites[0];
            costumeDescriptionText.text = $"{costumeItemData.CostumeItemOptionDatas[0].OptionDescription} +{costumeItemData.CostumeItemOptionDatas[0].UpgradeOptionValue}";
            
            if (costumeItemData.CostumeItemOptionDatas.Count > 1)
            {
                costumeOption1DescriptionIconImage.gameObject.SetActive(true);
                costumeOption1DescriptionText.gameObject.SetActive(true);
                costumeOption1DescriptionText.text = $"{costumeItemData.CostumeItemOptionDatas[0].OptionDescription} +{costumeItemData.CostumeItemOptionDatas[0].UpgradeOptionValue}";
            }
            else
            {
                costumeOption1DescriptionIconImage.gameObject.SetActive(false);
                costumeOption1DescriptionText.gameObject.SetActive(false);
            }
            
            gameObject.SetActive(true);
        }

        public void OnClickEnhancementButton()
        {
            uiPanelCostumeUpgrade.Activate(_costumeItemData);
        }
        
        public void OnClickEquipmentButton()
        {
            VolatileDataManager.Instance.Player.PlayerCostumeModule.EquipCostume(_costumeItemData.CostumeType, _costumeItemData);
            CostumeManager.Instance.SortCostumeItems();
        }
    }
}