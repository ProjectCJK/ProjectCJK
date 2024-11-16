using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeItem : MonoBehaviour, IPoolable
    {
        [SerializeField] private Image costumeBackground;
        [SerializeField] private Image costumeIcon;
        [SerializeField] private TextMeshProUGUI costumeLevel;
        [SerializeField] private Image equipmentEffect;
        
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private UI_Panel_Popup _uiPanelPopup;
        
        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache,
            UI_Panel_Popup uiPanelPopup)
        {
            _frontGroundImageCache = frontGroundImageCache;
            _uiPanelPopup = uiPanelPopup;

            GetComponent<Button>().onClick.AddListener(OnClickItem);
        }
        
        public void Initialize(CostumeItemData costumeItem)
        {
            equipmentEffect.gameObject.SetActive(costumeItem.IsEquipped);

            costumeIcon.sprite = costumeItem.CostumeSprites[0];
            costumeBackground.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(costumeItem.CostumeType, costumeItem.CostumeGrade)];
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
            _uiPanelPopup.RegisterReference();
            _uiPanelPopup.gameObject.SetActive(true);
        }
    }
}