using System;
using System.Collections.Generic;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeUpgradeItem : MonoBehaviour, IPoolable
    {
        [SerializeField] private Image costumeBackground;
        [SerializeField] private Image costumeIcon;
        [SerializeField] private TextMeshProUGUI costumeLevel;
        [SerializeField] private Image selectEffect;
        
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private CostumeItemData _costumeItemData;
        
        private bool isSelected;
        
        public void RegisterReference(Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache)
        {
            _frontGroundImageCache = frontGroundImageCache;
        }
        
        public void Activate(CostumeItemData costumeItem, Action<CostumeItemData> OnClick)
        {
            isSelected = false;
            
            _costumeItemData = costumeItem;
            
            costumeIcon.sprite = costumeItem.CostumeSprites[0];
            costumeBackground.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(costumeItem.CostumeType, costumeItem.CostumeGrade)];
            costumeLevel.text = $"Lv.{costumeItem.CurrentLevel}";
            
            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() =>
            {
                isSelected = !isSelected;
                
                UpdateUI();
                OnClick?.Invoke(costumeItem);
            });
        }

        private void UpdateUI()
        {
            selectEffect.gameObject.SetActive(isSelected);
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
    }
}