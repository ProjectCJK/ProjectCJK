using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeInventoryItem : MonoBehaviour, IPoolable
    {
        [SerializeField] private Image costumeBackground;
        [SerializeField] private Image costumeIcon;
        [SerializeField] private TextMeshProUGUI costumeLevel;
        [SerializeField] private Image equipmentEffect;
        
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private UI_Panel_CostumeInfo _uiPanelCostumeInfo;
        private CostumeItemData _costumeItemData;
        
        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
            UI_Panel_CostumeInfo uiPanelCostumeInfo)
        {
            _frontGroundImageCache = frontGroundImageCache;
            _uiPanelCostumeInfo = uiPanelCostumeInfo;

            GetComponent<Button>().onClick.AddListener(OnClickItem);
        }
        
        public void Activate(CostumeItemData costumeItem)
        {
            _costumeItemData = costumeItem;
            
            equipmentEffect.gameObject.SetActive(costumeItem.IsEquipped);
            costumeIcon.sprite = costumeItem.CostumeSprites[0];
            costumeBackground.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(costumeItem.CostumeType, costumeItem.CostumeGrade)];
            costumeLevel.text = $"Lv.{costumeItem.CurrentLevel}";
        }
        
        public void Create()
        {
            gameObject.SetActive(false);
        }

        public void GetFromPool()
        {
            gameObject.SetActive(true);
        }

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }

        private void OnClickItem()
        {
            _uiPanelCostumeInfo.Activate(_costumeItemData);
        }
    }
}