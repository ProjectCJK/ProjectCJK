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
        [SerializeField] private Image costumeIconImage;
        [SerializeField] private Image costumeBackgroundImage;

        [Space(20), SerializeField] private TextMeshProUGUI costumeDescriptionText;
        [SerializeField] private Image costumeOption1DescriptionIconImage;
        [SerializeField] private TextMeshProUGUI costumeOption1DescriptionText;

        [Space(20), SerializeField] private Button enhancementButton;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button equipButtonNone;

        [Space(20), SerializeField] private UI_Panel_CostumeUpgrade uiPanelCostumeUpgrade;

        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private CostumeItemData _costumeItemData;
        private UI_Panel_CostumeInventory _uiPanelCostumeInventory;
        private UI_Panel_CurrentEquippedCostumeInfo _uiPanelCurrentEquippedCostumeInfo;

        public void RegisterReference(Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
            List<CostumeItemData> currentCostumeItemData,
            UI_Panel_CostumeInventory uiPanelCostumeInventory,
            UI_Panel_CurrentEquippedCostumeInfo uiPanelCurrentEquippedCostumeInfo)
        {
            _frontGroundImageCache = frontGroundImageCache;
            _uiPanelCostumeInventory = uiPanelCostumeInventory;
            _uiPanelCurrentEquippedCostumeInfo = uiPanelCurrentEquippedCostumeInfo;

            uiPanelCostumeUpgrade.RegisterReference(frontGroundImageCache, currentCostumeItemData);
            uiPanelCostumeUpgrade.RegisterUpdateActions(UpdateUI, _uiPanelCostumeInventory.UpdateItems, uiPanelCurrentEquippedCostumeInfo.Activate);

            // 버튼 이벤트 등록
            enhancementButton.onClick.AddListener(OnClickEnhancementButton);
            equipButton.onClick.AddListener(OnClickEquipmentButton);
        }

        public void UpdateUI(CostumeItemData costumeItemData)
        {
            _costumeItemData = costumeItemData;

            // 텍스트 및 이미지 업데이트
            costumeNameText.text = costumeItemData.CostumeName;

            for (int i = 0; i < costumeTypeImage.Count; i++)
                costumeTypeImage[i].gameObject.SetActive((int)costumeItemData.CostumeType == i);

            costumeTypeText.text = $"{costumeItemData.CostumeType}";
            costumeLevelText.text = $"Lv. {costumeItemData.CurrentLevel}";
            costumeIconImage.sprite = costumeItemData.CostumeSprites[0];
            costumeBackgroundImage.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(costumeItemData.CostumeType, costumeItemData.CostumeGrade)];

            costumeDescriptionText.text = $"{costumeItemData.CostumeItemOptionDatas[0].OptionDescription} +{costumeItemData.GetCurrentOptionValue()[0]}";

            if (costumeItemData.CostumeItemOptionDatas.Count > 1)
            {
                costumeOption1DescriptionIconImage.gameObject.SetActive(true);
                costumeOption1DescriptionText.gameObject.SetActive(true);
                costumeOption1DescriptionText.text = $"{costumeItemData.CostumeItemOptionDatas[1].OptionDescription} +{costumeItemData.GetCurrentOptionValue()[1]}";
            }
            else
            {
                costumeOption1DescriptionIconImage.gameObject.SetActive(false);
                costumeOption1DescriptionText.gameObject.SetActive(false);
            }

            if (costumeItemData.IsEquipped)
            {
                equipButton.gameObject.SetActive(false);
                equipButtonNone.gameObject.SetActive(true);
            }
            else
            {
                equipButton.gameObject.SetActive(true);
                equipButtonNone.gameObject.SetActive(false);
            }
        }

        public void Activate(CostumeItemData costumeItemData)
        {
            UpdateUI(costumeItemData);
            gameObject.SetActive(true);
        }

        public void OnClickEnhancementButton()
        {
            uiPanelCostumeUpgrade.Activate(_costumeItemData);
        }

        public void OnClickEquipmentButton()
        {
            // 기존 장착 장비 해제
            if (VolatileDataManager.Instance.EquippedCostumes.TryGetValue(_costumeItemData.CostumeType, out var equippedCostume))
            {
                equippedCostume.IsEquipped = false;
                VolatileDataManager.Instance.Player.PlayerCostumeModule.UpdateEquippedCostumeStats(equippedCostume, true); // 스탯 감소
            }

            // 새 장비 장착
            _costumeItemData.IsEquipped = true;
            VolatileDataManager.Instance.EquippedCostumes[_costumeItemData.CostumeType] = _costumeItemData;
            VolatileDataManager.Instance.Player.PlayerCostumeModule.UpdateEquippedCostumeStats(_costumeItemData, false); // 스탯 증가

            // 인벤토리와 UI 업데이트
            _uiPanelCostumeInventory.UpdateItems();
            _uiPanelCurrentEquippedCostumeInfo.Activate();
            CostumeManager.Instance.SortCostumeItems();
            gameObject.SetActive(false);
        }
    }
}