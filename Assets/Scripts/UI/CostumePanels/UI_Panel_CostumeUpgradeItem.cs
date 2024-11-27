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

        public CostumeItemData CostumeItemData { get; private set; }
        public bool IsSelected { get; private set; }

        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;

        // 선택/해제 시 상태 업데이트 콜백
        private Action<CostumeItemData, bool> _onSelectCallback;

        public void RegisterReference(Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCache)
        {
            _frontGroundImageCache = frontGroundImageCache;
        }

        public void Activate(CostumeItemData costumeItem, Action<CostumeItemData, bool> onSelectCallback)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            IsSelected = false;
            CostumeItemData = costumeItem;
            _onSelectCallback = onSelectCallback;

            costumeIcon.sprite = costumeItem.CostumeSprites[0];
            costumeBackground.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(costumeItem.CostumeType, costumeItem.CostumeGrade)];
            costumeLevel.text = $"Lv.{costumeItem.CurrentLevel}";

            UpdateUI();

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(ToggleSelection);
        }

        private void ToggleSelection()
        {
            IsSelected = !IsSelected;
            UpdateUI();

            // 선택/해제 상태에 따라 콜백 실행
            _onSelectCallback?.Invoke(CostumeItemData, IsSelected);
        }

        private void UpdateUI()
        {
            selectEffect.gameObject.SetActive(IsSelected);
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
            CostumeItemData = null; // 데이터 초기화
            _onSelectCallback = null; // 콜백 초기화
            IsSelected = false; // 선택 상태 초기화
        }
    }
}