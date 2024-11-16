using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumeGachaPanels
{
    [Serializable]
    public struct GachaCostumeItemData
    {
        public Image backBackgroundImage;
        public Image frontBackgroundImage;
        public Image iconImage;
    }
    
    public class UI_Panel_Costume_Gacha : MonoBehaviour
    {
        private static readonly int OpenFirst = Animator.StringToHash("Open_First");
        private static readonly int OpenSecond = Animator.StringToHash("Open_Second");
        private static readonly int Result = Animator.StringToHash("Result");

        [SerializeField] private CostumeBoxController costumeBoxController;
        [SerializeField] private GachaCostumeItemData _currentGachaCostumeItemDatas;
        [SerializeField] private List<GachaCostumeItemData> _gachaCostumeItemDatas;
        
        private int currentIndex = 0;
        private int maxIndex;

        private List<CostumeItemData> _costumeItems;
        private Dictionary<ECostumeGrade, Sprite> _backgroundImageCache = new();
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache = new();

        private void OnEnable()
        {
            currentIndex = 0;
        }

        public void RegisterReference(Dictionary<ECostumeGrade, Sprite> backgroundImageCache, Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache)
        {
            _backgroundImageCache = backgroundImageCache;
            _frontGroundImageCache = frontGroundImageCache;
        }

        public void Activate(List<CostumeItemData> costumeItems)
        {
            _costumeItems = costumeItems;
            maxIndex = costumeItems.Count;
         
            UpdateEntireGachaCostumeItemData();
            
            gameObject.SetActive(true);
        }
        
        public void OnClick_Costume_Gacha()
        {
            if (currentIndex == 0 && costumeBoxController.IsEndedIdleAnimation)
            {
                UpdateGachaCostumeItemData(_currentGachaCostumeItemDatas, currentIndex);
                currentIndex++;
                costumeBoxController.Animator.SetTrigger(OpenFirst);
            }
            else if (currentIndex > 0 && currentIndex < maxIndex && costumeBoxController.IsEndedOpenIdleAnimation)
            {
                UpdateGachaCostumeItemData(_currentGachaCostumeItemDatas, currentIndex);
                currentIndex++;
                costumeBoxController.IsEndedOpenIdleAnimation = false;
                costumeBoxController.Animator.SetTrigger(OpenSecond);
            }
            else if (currentIndex == maxIndex && costumeBoxController.IsEndedOpenIdleAnimation)
            {
                currentIndex++;
                costumeBoxController.Animator.SetTrigger(Result);
            }
            else if (currentIndex == maxIndex + 1 && costumeBoxController.IsEndedResultAnimation)
            {
                _gachaCostumeItemDatas.Clear();
                gameObject.SetActive(false);
            }
        }

        private void UpdateGachaCostumeItemData(GachaCostumeItemData gachaData, int index)
        {
            gachaData.backBackgroundImage.sprite = _backgroundImageCache[_costumeItems[index].CostumeGrade];
            gachaData.frontBackgroundImage.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(_costumeItems[index].CostumeType, _costumeItems[index].CostumeGrade)];
            gachaData.iconImage.sprite = _costumeItems[index].CostumeSprites[0];
        }

        private void UpdateEntireGachaCostumeItemData()
        {
            if (_gachaCostumeItemDatas.Count != _costumeItems.Count)
            {
                Debug.LogWarning("Mismatch in the number of Gacha Costume Item Data and Costume Items.");
                return;
            }

            for (var i = 0; i < _costumeItems.Count; i++)
            {
                UpdateGachaCostumeItemData(_gachaCostumeItemDatas[i], i);
            }
        }
    }
}