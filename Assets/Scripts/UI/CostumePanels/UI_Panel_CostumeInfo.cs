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

        [Space(20), SerializeField] private UI_Panel_CostumeUpgrade uiPanelCostumeUpgrade;

        public UI_Panel_CostumeUpgrade CostumeUpgradePanel => uiPanelCostumeUpgrade;

        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private CostumeItemData _costumeItemData;

        // public void RegisterReference(
        //     Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
        //     List<CostumeItemData> currentCostumeItemData)
        // {
        //     gameObject.SetActive(false);
        //     _frontGroundImageCache = frontGroundImageCache;
        //     _currentCostumeItemData = currentCostumeItemData;
        //
        //     upgradeButton.onClick.AddListener(OnClickUpgradeButton);
        //
        //     ObjectPoolManager.Instance.CreatePool(PoolKey, 5, 99999, true, () => InstantiateUpgradeItem(_costumeUpgradeItemPrefab), _itemPrefabInstancePosition);
        // }
        
        public void RegisterReference(Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache, List<CostumeItemData> currentCostumeItemData)
        {
            gameObject.SetActive(false);

            _frontGroundImageCache = frontGroundImageCache;

            uiPanelCostumeUpgrade.RegisterReference(frontGroundImageCache, currentCostumeItemData);

            // 의존성 주입으로 UI 업데이트 연결
            uiPanelCostumeUpgrade.RegisterUpdateActions(UpdateUI, null);
        }

        public void UpdateUI(CostumeItemData costumeItemData)
        {
            _costumeItemData = costumeItemData;

            UpdateUI();
        }

        public void Activate(CostumeItemData costumeItemData)
        {
            _costumeItemData = costumeItemData;
            UpdateUI();

            gameObject.SetActive(true);
        }

        public void UpdateUI()
        {
            costumeNameText.text = _costumeItemData.CostumeName;

            switch (_costumeItemData.CostumeGrade)
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

            costumeTypeText.text = $"{_costumeItemData.CostumeType}";
            costumeLevelText.text = $"Lv. {_costumeItemData.CurrentLevel}";
            costumeIconImage.sprite = _costumeItemData.CostumeSprites[0];
            costumeBackgroundImage.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(_costumeItemData.CostumeType, _costumeItemData.CostumeGrade)];
            costumeDescriptionText.text = $"{_costumeItemData.GetCurrentOptionValue()[0]}";

            if (_costumeItemData.CostumeItemOptionDatas.Count > 1)
            {
                costumeOption1DescriptionIconImage.gameObject.SetActive(true);
                costumeOption1DescriptionText.gameObject.SetActive(true);
                costumeOption1DescriptionText.text = $"{_costumeItemData.GetCurrentOptionValue()[1]}";
            }
            else
            {
                costumeOption1DescriptionIconImage.gameObject.SetActive(false);
                costumeOption1DescriptionText.gameObject.SetActive(false);
            }
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