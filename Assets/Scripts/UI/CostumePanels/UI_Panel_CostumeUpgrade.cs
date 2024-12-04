using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Modules.DesignPatterns.ObjectPools;
using TMPro;
using Units.Stages.Units.Items.Enums;
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
        [SerializeField] private Image costumeOption1IconLock;
        [SerializeField] private TextMeshProUGUI costumeOption1Description;

        [Space(20), SerializeField] private Button upgradeButton_EnoughRedGem;
        [SerializeField] private TextMeshProUGUI upgradeButton_EnoughRedGemText;
        [SerializeField] private Button upgradeButton_NotEnoughRedGem;
        [SerializeField] private TextMeshProUGUI upgradeButton_NotEnoughRedGemText;

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
        private int _requiredGems;

        private const float duration = 0.5f;

        private Action<CostumeItemData> _updateCostumeInfoAction;
        private Action _updateCostumeInventoryAction;
        private Action _updateUICurrentEquippedCostumeInfo;
        
        private int _totalMaterialValue; // 선택한 아이템들의 MaterialValues 합산

        public void RegisterReference(
            Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> frontGroundImageCachePara,
            List<CostumeItemData> currentCostumeItemData)
        {
            _frontGroundImageCache = frontGroundImageCachePara;
            _currentCostumeItemData = currentCostumeItemData;

            upgradeButton_EnoughRedGem.onClick.AddListener(() =>
            {
                CurrencyManager.Instance.RemoveCurrency(ECurrencyType.RedGem, _requiredGems);
                
                if (!GameManager.Instance.ES3Saver.first_costume_upgrade)
                {
                    GameManager.Instance.ES3Saver.first_costume_upgrade = true;
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("first_costume_upgrade");
                }
                
                QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Upgrade, "Quest", 1);
                OnClickUpgradeButton();
            });

            ObjectPoolManager.Instance.CreatePool(PoolKey, 50, 99999, true, () => InstantiateCostumeUpgradeItem(_costumeUpgradeItemPrefab), _itemPrefabInstancePosition);
        }

        public void RegisterUpdateActions(Action<CostumeItemData> updateCostumeInfo, Action updateCostumeInventory,
            Action updateUICurrentEquippedCostumeInfo)
        {
            _updateCostumeInfoAction = updateCostumeInfo;
            _updateCostumeInventoryAction = updateCostumeInventory;
            _updateUICurrentEquippedCostumeInfo = updateUICurrentEquippedCostumeInfo;
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
            var materialValue = materialItem.MaterialValues[materialItem.CurrentLevel - 1];
            _finalExp += isAdding ? materialValue : -materialValue;

            // 선택한 아이템들의 합산 값 업데이트
            _totalMaterialValue += isAdding ? materialValue : -materialValue;
            _requiredGems = Mathf.CeilToInt(_totalMaterialValue * 0.5f);
            
            UpdateUpgradeButtonState();

            if (_sliderAnimationCoroutine != null)
            {
                StopCoroutine(_sliderAnimationCoroutine);
                _cachedSliderValue = costumeCurrentExp.value;
            }
            else
            {
                _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;
            }

            _sliderAnimationCoroutine = StartCoroutine(AnimateSliderAndLevelChange());
        }

        private void UpdateUpgradeButtonState()
        {
            if (_totalMaterialValue == 0)
            {
                // 선택된 아이템이 없는 경우
                upgradeButton_EnoughRedGem.gameObject.SetActive(false);
                upgradeButton_NotEnoughRedGem.gameObject.SetActive(false);
                upgradeButton_EnoughRedGemText.gameObject.SetActive(false);
                upgradeButton_NotEnoughRedGemText.gameObject.SetActive(false);
            }
            else
            {
                float requiredGems = Mathf.CeilToInt(_totalMaterialValue * 0.5f);
                var costText = $"<sprite=4> {requiredGems}";

                if (CurrencyManager.Instance.RedGem >= requiredGems)
                {
                    upgradeButton_EnoughRedGemText.text = costText;
                    upgradeButton_EnoughRedGem.gameObject.SetActive(true);
                    upgradeButton_NotEnoughRedGem.gameObject.SetActive(false);
                }
                else
                {
                    upgradeButton_NotEnoughRedGemText.text = costText;
                    upgradeButton_EnoughRedGem.gameObject.SetActive(false);
                    upgradeButton_NotEnoughRedGem.gameObject.SetActive(true);
                }

                upgradeButton_EnoughRedGemText.gameObject.SetActive(true);
                upgradeButton_NotEnoughRedGemText.gameObject.SetActive(true);
            }
        }

        private IEnumerator AnimateSliderAndLevelChange()
        {
            float totalDuration = duration;
            while (true)
            {
                if (_finalExp < 0 && _tempCostumeLevel > 1)
                {
                    int previousMaxExp = _costumeItemData.MaxExps[_tempCostumeLevel - 2];
                    float firstPhaseDuration = totalDuration * (float)(_tempCostumeExp) / (_tempCostumeMaxExp + _tempCostumeExp);
                    float secondPhaseDuration = totalDuration - firstPhaseDuration;

                    yield return AnimateSlider(_cachedSliderValue, 0f, firstPhaseDuration);
                    _tempCostumeLevel--;
                    _tempCostumeMaxExp = previousMaxExp;

                    UpdateLevelText();
                    UpdateCostumeDescriptions();
                    UpdateUpgradeButtonState();

                    _tempCostumeExp = _finalExp + _tempCostumeMaxExp;
                    _finalExp = _tempCostumeExp;
                    _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;

                    yield return AnimateSlider(1f, _cachedSliderValue, secondPhaseDuration);
                }
                else if (_finalExp >= _tempCostumeMaxExp && _tempCostumeLevel < _costumeItemData.MaxLevel)
                {
                    int overflowExp = _finalExp - _tempCostumeMaxExp;
                    float firstPhaseDuration = totalDuration * (float)(_tempCostumeMaxExp - _tempCostumeExp) / (_tempCostumeMaxExp + overflowExp);
                    float secondPhaseDuration = totalDuration - firstPhaseDuration;

                    yield return AnimateSlider(_cachedSliderValue, 1f, firstPhaseDuration);
                    _tempCostumeLevel++;
                    _tempCostumeMaxExp = _costumeItemData.MaxExps[_tempCostumeLevel - 1];

                    UpdateLevelText();
                    UpdateCostumeDescriptions();
                    UpdateUpgradeButtonState();

                    _tempCostumeExp = overflowExp;
                    _finalExp = _tempCostumeExp;
                    _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;

                    yield return AnimateSlider(0f, _cachedSliderValue, secondPhaseDuration);
                }
                else
                {
                    yield return AnimateSlider(_cachedSliderValue, (float)_finalExp / _tempCostumeMaxExp, totalDuration);
                    _tempCostumeExp = _finalExp;
                    _cachedSliderValue = (float)_tempCostumeExp / _tempCostumeMaxExp;
                    break;
                }
            }

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

            costumeCurrentExp.value = endValue;
        }

        private void InitializeTemporaryValues()
        {
            _tempCostumeLevel = _costumeItemData.CurrentLevel;
            _tempCostumeExp = _costumeItemData.CurrentExp;
            _tempCostumeMaxExp = _costumeItemData.MaxExps[_tempCostumeLevel - 1];
            _finalExp = _tempCostumeExp;

            costumeCurrentExp.value = (float)_tempCostumeExp / _tempCostumeMaxExp;
            _totalMaterialValue = 0;
            UpdateUpgradeButtonState(); // 초기 상태 업데이트
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
            UpdateUpgradeButtonState();

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
                costumeOption1IconLock.gameObject.SetActive(false);
                costumeOption1Description.gameObject.SetActive(true);
            }
            else
            {
                costumeOption1Description.text = $"No Option"; 
                costumeOption1Icon.gameObject.SetActive(false);
                costumeOption1IconLock.gameObject.SetActive(true);
            }
        }

        private void OnClickUpgradeButton()
        {
            var playerCostumeModule = VolatileDataManager.Instance.Player.PlayerCostumeModule;

            if (_costumeItemData.IsEquipped)
            {
                playerCostumeModule.UpdateEquippedCostumeStats(_costumeItemData, true);
            }

            _costumeItemData.CurrentLevel = _tempCostumeLevel;
            _costumeItemData.CurrentExp = _tempCostumeExp;

            if (_costumeItemData.IsEquipped)
            {
                playerCostumeModule.UpdateEquippedCostumeStats(_costumeItemData, false);
            }

            foreach (var item in _ui_Panel_CostumeUpgradeItems.Where(item => item.IsSelected))
            {
                _currentCostumeItemData.Remove(item.CostumeItemData);
                ObjectPoolManager.Instance.ReturnObject(PoolKey, item);
            }

            _updateCostumeInfoAction?.Invoke(_costumeItemData);
            _updateCostumeInventoryAction?.Invoke();
            _updateUICurrentEquippedCostumeInfo?.Invoke();

            CostumeManager.Instance.SaveCostumeData();
            ClearUpgradeItems();
            Activate(_costumeItemData);
        }

        private void OnDisable()
        {
            ClearUpgradeItems();
        }
    }
}