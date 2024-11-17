using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_CostumeUpgrade : MonoBehaviour
    {
        [SerializeField] private GameObject _costumeUpgradeItemPrefab;
        [SerializeField] private Transform _itemPrefabInstancePosition;

        [Space(20), SerializeField] private Image costumeBackground;
        [SerializeField] private Image costumeIcon;
        [SerializeField] private TextMeshProUGUI costumeCurrentLevel;
        [SerializeField] private TextMeshProUGUI costumeNextLevel;
        [SerializeField] private Slider costumeCurrentExp;

        [Space(20), SerializeField] private TextMeshProUGUI costumeDescription;
        [SerializeField] private Image costumeOption1Icon;
        [SerializeField] private TextMeshProUGUI costumeOption1Description;

        [Space(20), SerializeField] private Button upgradeButton;

        private static string PoolKey => "CostumeUpgradeItemPool";
        private readonly List<UI_Panel_CostumeUpgradeItem> _ui_Panel_CostumeUpgradeItems = new();
        private Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache;
        private List<CostumeItemData> _currentCostumeItemData;
        private CostumeItemData _costumeItemData;
        private Coroutine _sliderAnimationCoroutine;

        private int _tempCostumeLevel;
        private int _tempCostumeExp;
        private int _tempCostumeMaxExp;
        private int _finalExp; // 애니메이션 도중 목표 경험치
        private float _cachedSliderValue;
        
        private const float duration = 0.5f;
                
        private Action<CostumeItemData> _updateCostumeInfoAction;
        private Action _updateCostumeInventoryAction;

        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCachePara,
            List<CostumeItemData> currentCostumeItemData)
        {
            gameObject.SetActive(false);

            _frontGroundImageCache = frontGroundImageCachePara;
            _currentCostumeItemData = currentCostumeItemData;

            upgradeButton.onClick.AddListener(OnClickUpgradeButton);

            ObjectPoolManager.Instance.CreatePool(PoolKey, 5, 99999, true, () => InstantiateCostumeUpgradeItem(_costumeUpgradeItemPrefab), _itemPrefabInstancePosition);
        }
        
        public void RegisterUpdateActions(Action<CostumeItemData> updateCostumeInfo, Action updateCostumeInventory)
        {
            _updateCostumeInfoAction = updateCostumeInfo;
            _updateCostumeInventoryAction = updateCostumeInventory;
        }

        private UI_Panel_CostumeUpgradeItem InstantiateCostumeUpgradeItem(GameObject costumeItemPrefab)
        {
            GameObject obj = Instantiate(costumeItemPrefab, _itemPrefabInstancePosition, true);
            obj.transform.localScale = Vector3.one;
            var costumeItem = obj.GetComponent<UI_Panel_CostumeUpgradeItem>();
            costumeItem.RegisterReference(_frontGroundImageCache);

            return costumeItem;
        }

        public void Activate(CostumeItemData costumeItemData)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            _costumeItemData = costumeItemData;

            ClearUpgradeItems(); // 기존 아이템 제거
            PopulateUpgradeItems();

            InitializeTemporaryValues();
            UpdateUI();
        }

        private void ClearUpgradeItems()
        {
            foreach (UI_Panel_CostumeUpgradeItem item in _ui_Panel_CostumeUpgradeItems)
            {
                ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
            }

            _ui_Panel_CostumeUpgradeItems.Clear();
        }

        private void PopulateUpgradeItems()
        {
            var filteredCostumes = _currentCostumeItemData
                .Where(item => item.CostumeType == _costumeItemData.CostumeType && item != _costumeItemData && item.IsEquipped == false)
                .ToList();

            foreach (CostumeItemData costumeItem in filteredCostumes)
            {
                var item = ObjectPoolManager.Instance.GetObject<UI_Panel_CostumeUpgradeItem>(PoolKey, null);
                _ui_Panel_CostumeUpgradeItems.Add(item);
                item.Activate(costumeItem, AddMaterialForUpgrade);
            }
        }

        private void AddMaterialForUpgrade(CostumeItemData materialItem, bool isAdding)
        {
            int materialValue = materialItem.MaterialValues[_tempCostumeLevel - 1];
            _finalExp += isAdding ? materialValue : -materialValue;

            // 기존 코루틴 중단 및 슬라이더 값 캐싱
            if (_sliderAnimationCoroutine != null)
            {
                StopCoroutine(_sliderAnimationCoroutine);
                _cachedSliderValue = costumeCurrentExp.value; // 현재 슬라이더 값을 캐싱
            }
            else
            {
                // 슬라이더 애니메이션이 없는 경우, 캐싱 값을 현재 상태로 초기화
                _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;
            }

            // 새로운 코루틴 시작
            _sliderAnimationCoroutine = StartCoroutine(AnimateSliderAndLevelChange());
        }

        private IEnumerator AnimateSliderAndLevelChange()
        {
            float totalDuration = duration; // 전체 애니메이션 시간
            while (true)
            {
                if (_finalExp < 0 && _tempCostumeLevel > 1)
                {
                    // 레벨 다운
                    int previousMaxExp = _costumeItemData.MaxExps[_tempCostumeLevel - 2];
                    float firstPhaseDuration = totalDuration * (float)(_tempCostumeExp) / (_tempCostumeMaxExp + _tempCostumeExp);
                    float secondPhaseDuration = totalDuration - firstPhaseDuration;

                    yield return AnimateSlider(_cachedSliderValue, 0f, firstPhaseDuration); // 1단계: 현재 → 0%
                    _tempCostumeLevel--;
                    _tempCostumeMaxExp = previousMaxExp;

                    // UI 즉시 업데이트
                    UpdateLevelText();
                    UpdateCostumeDescriptions();

                    _tempCostumeExp = _finalExp + _tempCostumeMaxExp;
                    _finalExp = _tempCostumeExp;
                    _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;

                    yield return AnimateSlider(1f, _cachedSliderValue, secondPhaseDuration); // 2단계: 100% → 남은 값
                }
                else if (_finalExp >= _tempCostumeMaxExp && _tempCostumeLevel < _costumeItemData.MaxLevel)
                {
                    // 레벨 업
                    int overflowExp = _finalExp - _tempCostumeMaxExp;
                    float firstPhaseDuration = totalDuration * (float)(_tempCostumeMaxExp - _tempCostumeExp) / (_tempCostumeMaxExp + overflowExp);
                    float secondPhaseDuration = totalDuration - firstPhaseDuration;

                    yield return AnimateSlider(_cachedSliderValue, 1f, firstPhaseDuration); // 1단계: 현재 → 100%
                    _tempCostumeLevel++;
                    _tempCostumeMaxExp = _costumeItemData.MaxExps[_tempCostumeLevel - 1];

                    // UI 즉시 업데이트
                    UpdateLevelText();
                    UpdateCostumeDescriptions();

                    _tempCostumeExp = overflowExp;
                    _finalExp = _tempCostumeExp;
                    _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;

                    yield return AnimateSlider(0f, _cachedSliderValue, secondPhaseDuration); // 2단계: 0% → 남은 값
                }
                else
                {
                    // 일반적인 경우
                    yield return AnimateSlider(_cachedSliderValue, (float)_finalExp / _tempCostumeMaxExp, totalDuration);
                    _tempCostumeExp = _finalExp;
                    _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;
                    break;
                }
            }

            // 최종 UI 동기화
            UpdateUI();
        }

        private IEnumerator AnimateSlider(float startValue, float endValue, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                costumeCurrentExp.value = Mathf.Lerp(startValue, endValue, t);
                yield return null;
            }

            costumeCurrentExp.value = endValue; // 최종 값 설정
        }

        private void InitializeTemporaryValues()
        {
            _tempCostumeLevel = _costumeItemData.CurrentLevel;
            _tempCostumeExp = _costumeItemData.CurrentExp;
            _tempCostumeMaxExp = _costumeItemData.MaxExps[_tempCostumeLevel - 1];
            _finalExp = _tempCostumeExp;
            
            costumeCurrentExp.value = (float)_tempCostumeExp / _tempCostumeMaxExp;
        }

        private void UpdateLevelText()
        {
            costumeCurrentLevel.text = $"Lv. {_costumeItemData.CurrentLevel}";
            costumeNextLevel.text = $"Lv. {_tempCostumeLevel}";
        }

        private void UpdateUI()
        {
            if (!gameObject.activeSelf) return;

            UpdateLevelText();
            UpdateCostumeDescriptions();
            
            costumeBackground.sprite = _frontGroundImageCache[new Tuple<ECostumeType, ECostumeGrade>(_costumeItemData.CostumeType, _costumeItemData.CostumeGrade)];
            costumeIcon.sprite = _costumeItemData.CostumeSprites[0];
        }

        private void UpdateCostumeDescriptions()
        {
            costumeDescription.text = $"{_costumeItemData.CostumeItemOptionDatas[0].OptionDescription} +{_costumeItemData.GetCurrentOptionValue()[0]} \u25b6 {_costumeItemData.CostumeItemOptionDatas[0].BaseOptionValue + (_tempCostumeLevel - 1) * _costumeItemData.CostumeItemOptionDatas[0].UpgradeOptionValue}";

            if (_costumeItemData.CostumeItemOptionDatas.Count > 1)
            {
                costumeOption1Description.text = $"{_costumeItemData.CostumeItemOptionDatas[1].OptionDescription} +{_costumeItemData.GetCurrentOptionValue()[1]} \u25b6 {_costumeItemData.CostumeItemOptionDatas[1].BaseOptionValue + (_tempCostumeLevel - 1) * _costumeItemData.CostumeItemOptionDatas[1].UpgradeOptionValue}";

                costumeOption1Icon.gameObject.SetActive(true);
                costumeOption1Description.gameObject.SetActive(true);
            }
            else
            {
                costumeOption1Icon.gameObject.SetActive(false);
                costumeOption1Description.gameObject.SetActive(false);
            }
        }

        // private void OnClickUpgradeButton()
        // {
        //     _costumeItemData.CurrentLevel = _tempCostumeLevel;
        //     _costumeItemData.CurrentExp = _tempCostumeExp;
        //
        //     foreach (UI_Panel_CostumeUpgradeItem selectedItem in _ui_Panel_CostumeUpgradeItems.Where(item => item.IsSelected))
        //     {
        //         _currentCostumeItemData.Remove(selectedItem.CostumeItemData);
        //         ObjectPoolManager.Instance.ReturnObject(PoolKey, selectedItem);
        //     }
        //
        //     ClearUpgradeItems();
        //     Activate(_costumeItemData);
        // }
        
        private void OnClickUpgradeButton()
        {
            var playerCostumeModule = VolatileDataManager.Instance.Player.PlayerCostumeModule;

            // 장착된 장비의 강화 전 스탯 감소 처리
            if (_costumeItemData.IsEquipped)
            {
                playerCostumeModule.UpdateEquippedCostumeStats(_costumeItemData, true); // 스탯 감소
            }

            // 강화 데이터 갱신
            _costumeItemData.CurrentLevel = _tempCostumeLevel;
            _costumeItemData.CurrentExp = _tempCostumeExp;

            // 장착된 장비의 강화 후 스탯 증가 처리
            if (_costumeItemData.IsEquipped)
            {
                playerCostumeModule.UpdateEquippedCostumeStats(_costumeItemData, false); // 스탯 증가
            }

            // 강화 재료 제거
            foreach (var item in _ui_Panel_CostumeUpgradeItems.Where(item => item.IsSelected))
            {
                _currentCostumeItemData.Remove(item.CostumeItemData);
                ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
            }

            // UI 업데이트
            _updateCostumeInfoAction?.Invoke(_costumeItemData);
            _updateCostumeInventoryAction?.Invoke();

            ClearUpgradeItems();
            Activate(_costumeItemData);
        }


        private void OnDisable()
        {
            ClearUpgradeItems();
        }
    }
}