using System;
using System.Collections.Generic;
using System.Linq;
using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Sprites;
using UI;
using UI.CostumeGachaPanels;
using UI.CostumePanels;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum ECostumeType
    {
        Clothes,
        Bag,
        Hat,
        Weapon
    }
    
    public enum ECostumeOptionType
    {
        Player_Power,
        Player_Speed,
        Player_Bag_Size,
        All_Product_Income,
        Tip_Income
    }
    
    public enum ECostumeOptionCalculateType
    {
        Sum,
        Percent
    }
    
    public enum ECostumeGrade
    {
        Common,
        Rare
    }

    [Serializable]
    public class CostumeItemData
    {
        public bool IsEquipped;
        public string CostumeName;
        public List<Sprite> CostumeSprites;
        public ECostumeType CostumeType;
        public ECostumeGrade CostumeGrade;
        public int SpriteIndex;
        public int CurrentLevel;
        public int MaxLevel;
        public int CurrentExp;
        public List<int> MaxExps;
        public List<int> MaterialValues;
        public List<CostumeItemOptionData> CostumeItemOptionDatas;

        /// <summary>
        /// 현재 레벨에 따른 옵션 값 계산
        /// </summary>
        public List<float> GetCurrentOptionValue()
        {
            var currentLevel = CurrentLevel;
            return CostumeItemOptionDatas.Select(costumeItemOptionData =>
                costumeItemOptionData.BaseOptionValue +
                (currentLevel - 1) * costumeItemOptionData.UpgradeOptionValue).ToList();
        }

        /// <summary>
        /// CostumeSprites를 스프라이트 인덱스에 따라 설정
        /// </summary>
        public void RestoreSprites()
        {
            CostumeSprites = CostumeType switch
            {
                ECostumeType.Weapon => DataManager.Instance.CostumeSpriteSo
                    .WeaponCostumeSprites[SpriteIndex].Sprites,
                ECostumeType.Clothes => DataManager.Instance.CostumeSpriteSo
                    .BodyCostumeSprites[SpriteIndex].Sprites,
                ECostumeType.Bag => DataManager.Instance.CostumeSpriteSo
                    .BagCostumeSprites[SpriteIndex].Sprites,
                ECostumeType.Hat => DataManager.Instance.CostumeSpriteSo
                    .HatCostumeSprites[SpriteIndex].Sprites,
                _ => CostumeSprites
            };
        }

        /// <summary>
        /// 현재 CostumeItemData 객체를 복제
        /// </summary>
        public CostumeItemData Clone()
        {
            return new CostumeItemData
            {
                IsEquipped = IsEquipped,
                CostumeName = CostumeName,
                CostumeSprites = new List<Sprite>(CostumeSprites),
                CostumeType = CostumeType,
                CostumeGrade = CostumeGrade,
                SpriteIndex = SpriteIndex,
                CurrentLevel = CurrentLevel,
                MaxLevel = MaxLevel,
                CurrentExp = CurrentExp,
                MaxExps = new List<int>(MaxExps),
                MaterialValues = new List<int>(MaterialValues),
                CostumeItemOptionDatas = CostumeItemOptionDatas.Select(option => new CostumeItemOptionData
                {
                    CostumeOptionType = option.CostumeOptionType,
                    CostumeOptionCalculateType = option.CostumeOptionCalculateType,
                    BaseOptionValue = option.BaseOptionValue,
                    UpgradeOptionValue = option.UpgradeOptionValue,
                    OptionDescription = option.OptionDescription
                }).ToList()
            };
        }
    }
        
    [Serializable]
    public class CostumeItemOptionData
    {
        public ECostumeOptionType CostumeOptionType;
        public ECostumeOptionCalculateType CostumeOptionCalculateType;
        public float BaseOptionValue;
        public float UpgradeOptionValue;
        
        public string OptionDescription;
    }
    
    [Serializable]
    public class CostumeDataSerializable
    {
        public string CostumeName;
        public ECostumeType CostumeType;
        public ECostumeGrade CostumeGrade;
        public int SpriteIndex;
        public int CurrentLevel;
        public int CurrentExp;
    }

    public class CostumeManager : Singleton<CostumeManager>
    {
        private string[,] _costumeData;
        private string[,] _costumeBoxData;
        private string[,] _costumeParamData;
        private string[,] _costumeUpgradeData;

        private List<CostumeItemData> _cachedCommonCostumes;
        private List<CostumeItemData> _cachedRareCostumes;

        private readonly Dictionary<ECostumeGrade, Sprite> _backgroundImageCache = new();
        private readonly Dictionary<Tuple<ECostumeType, ECostumeGrade>, Sprite> _frontGroundImageCache = new();

        private readonly UI_Panel_Costume_Gacha _uiPanelCostumeGacha = UIManager.Instance.UI_Panel_CostumeGacha;
        private readonly UI_Panel_Costume _uiPanelCostume = UIManager.Instance.UI_Panel_Costume;

        private List<CostumeItemData> _currentCostumeItemData = new();

        public void RegisterReference()
        {
            _costumeData = DataManager.Instance.CostumeData.GetData();
            _costumeBoxData = DataManager.Instance.CostumeBoxData.GetData();
            _costumeParamData = DataManager.Instance.CostumeParamData.GetData();
            _costumeUpgradeData = DataManager.Instance.CostumeUpgradeData.GetData();

            CacheCostumes();
            CacheGachaBackgroundSprites();

            LoadCostumeData(); // ES3Saver에서 데이터 로드

            _uiPanelCostumeGacha.RegisterReference(_backgroundImageCache, _frontGroundImageCache);
            _uiPanelCostume.RegisterReference(_frontGroundImageCache, _currentCostumeItemData);

            UIManager.Instance.UI_Button_CostumeGachaPanel.onClick.AddListener(ActivateCostumeGacha);
            UIManager.Instance.UI_Button_CostumePanel.onClick.AddListener(ActivateCostumePanel);
        }

        private void CacheCostumes()
        {
            _cachedCommonCostumes = new List<CostumeItemData>();
            _cachedRareCostumes = new List<CostumeItemData>();

            for (var i = 2; i < _costumeData.GetLength(0); i++)
            {
                var costumeItem = new CostumeItemData
                {
                    CostumeName = _costumeData[i, 1],
                    CostumeType = Enum.Parse<ECostumeType>(_costumeData[i, 3]),
                    CostumeGrade = Enum.Parse<ECostumeGrade>(_costumeData[i, 5]),
                    SpriteIndex = int.Parse(_costumeData[i, 4])
                };

                costumeItem.RestoreSprites();
                costumeItem.CurrentLevel = 1;
                costumeItem.MaxLevel = Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                    .Where(index => _costumeUpgradeData[index, 1] == $"{costumeItem.CostumeGrade}")
                    .Select(index => int.Parse(_costumeUpgradeData[index, 2]))
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                costumeItem.MaxExps = Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                    .Where(index => _costumeUpgradeData[index, 1] == $"{costumeItem.CostumeGrade}")
                    .Select(index => int.Parse(_costumeUpgradeData[index, 3]))
                    .ToList();

                costumeItem.MaterialValues = Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                    .Where(index => _costumeUpgradeData[index, 1] == $"{costumeItem.CostumeGrade}")
                    .Select(index => int.Parse(_costumeUpgradeData[index, 4]))
                    .ToList();

                costumeItem.CostumeItemOptionDatas = new List<CostumeItemOptionData>();
                List<ECostumeOptionType> options = GetCostumeOptions(i);
                foreach (ECostumeOptionType costumeOptionType in options)
                {
                    var costumeItemOptionData = new CostumeItemOptionData();

                    // CostumeOptionType 설정
                    var paramIndex = Enumerable.Range(0, _costumeParamData.GetLength(0))
                        .FirstOrDefault(index => _costumeParamData[index, 4] == $"{costumeOptionType}");

                    if (paramIndex >= 0)
                    {
                        costumeItemOptionData.CostumeOptionType =
                            Enum.Parse<ECostumeOptionType>(_costumeParamData[paramIndex, 4]);

                        // CostumeOptionCalculateType 설정
                        costumeItemOptionData.CostumeOptionCalculateType =
                            Enum.Parse<ECostumeOptionCalculateType>(_costumeParamData[paramIndex, 5]);

                        // OptionDescription 설정
                        costumeItemOptionData.OptionDescription = _costumeParamData[paramIndex, 6];

                        // BaseOptionValue와 UpgradeOptionValue 설정 (Common과 Rare에 따라 다름)
                        if (costumeItem.CostumeGrade == ECostumeGrade.Common)
                        {
                            costumeItemOptionData.BaseOptionValue = float.Parse(_costumeParamData[paramIndex, 7]);
                            costumeItemOptionData.UpgradeOptionValue = float.Parse(_costumeParamData[paramIndex, 8]);
                        }
                        else if (costumeItem.CostumeGrade == ECostumeGrade.Rare)
                        {
                            costumeItemOptionData.BaseOptionValue = float.Parse(_costumeParamData[paramIndex, 9]);
                            costumeItemOptionData.UpgradeOptionValue = float.Parse(_costumeParamData[paramIndex, 10]);
                        }
                    }

                    costumeItem.CostumeItemOptionDatas.Add(costumeItemOptionData);
                }

                switch (costumeItem.CostumeGrade)
                {
                    case ECostumeGrade.Common:
                        _cachedCommonCostumes.Add(costumeItem);
                        break;
                    case ECostumeGrade.Rare:
                        _cachedRareCostumes.Add(costumeItem);
                        break;
                }
            }
        }
        
        private void CacheGachaBackgroundSprites()
        {
            foreach (CostumeBackBackgroundSprite costumeBackBackgroundSprite in DataManager.Instance.CostumeBackBackgroundSpriteSO.CostumeBackBackgroundSprites)
            {
                _backgroundImageCache.TryAdd(costumeBackBackgroundSprite.Grade, costumeBackBackgroundSprite.Sprite);
            }

            foreach (CostumeFrontBackgroundSprite costumeFrontBackgroundSprite in DataManager.Instance.CostumeFrontBackgroundSpriteSo.CostumeFrontBackgroundSprites)
            {
                _frontGroundImageCache.TryAdd(
                    new Tuple<ECostumeType, ECostumeGrade>(costumeFrontBackgroundSprite.Type, costumeFrontBackgroundSprite.Grade),
                    costumeFrontBackgroundSprite.Sprite
                );
            }
        }

        private List<ECostumeOptionType> GetCostumeOptions(int index)
        {
            var optionTypes = new List<ECostumeOptionType>();

            if (Enum.TryParse(_costumeData[index, 7], out ECostumeOptionType option1))
            {
                optionTypes.Add(option1);
            }

            if (Enum.TryParse(_costumeData[index, 8], out ECostumeOptionType option2))
            {
                optionTypes.Add(option2);
            }

            return optionTypes;
        }

        public void LoadCostumeData()
        {
            if (GameManager.Instance.ES3Saver.CurrentCostumes != null && GameManager.Instance.ES3Saver.CurrentCostumes.Count > 0)
            {
                _currentCostumeItemData = GameManager.Instance.ES3Saver.CurrentCostumes
                    .Select(data => new CostumeItemData
                    {
                        CostumeName = data.CostumeName,
                        CostumeType = data.CostumeType,
                        CostumeGrade = data.CostumeGrade,
                        SpriteIndex = data.SpriteIndex,
                        CurrentLevel = data.CurrentLevel,
                        CurrentExp = data.CurrentExp
                    }).ToList();

                foreach (var costume in _currentCostumeItemData)
                {
                    costume.RestoreSprites();
                }

                Debug.Log("Costume data loaded from CurrentCostumes.");
            }
            else
            {
                Debug.LogWarning("No costume data found in CurrentCostumes.");
            }
        }

        public void SaveCostumeData()
        {
            GameManager.Instance.ES3Saver.CurrentCostumes = _currentCostumeItemData.Select(costume => new CostumeDataSerializable
            {
                CostumeName = costume.CostumeName,
                CostumeType = costume.CostumeType,
                CostumeGrade = costume.CostumeGrade,
                SpriteIndex = costume.SpriteIndex,
                CurrentLevel = costume.CurrentLevel,
                CurrentExp = costume.CurrentExp
            }).ToList();

            Debug.Log("Costume data saved to CurrentCostumes.");
        }

        private void ActivateCostumeGacha()
        {
            var gachaItems = GetRandomItem(int.Parse(_costumeBoxData[2, 9]), int.Parse(_costumeBoxData[2, 10]));
            SortCostumeItems();
            _uiPanelCostumeGacha.Activate(gachaItems);
        }

        private List<CostumeItemData> GetRandomItem(int maxCommonGet, int maxRareGet)
        {
            var randomItems = new List<CostumeItemData>();

            for (var i = 0; i < maxCommonGet; i++)
            {
                var normalItem = GetNormalItem();
                _currentCostumeItemData.Add(normalItem);
                randomItems.Add(normalItem);
            }

            for (var i = 0; i < maxRareGet; i++)
            {
                var randomIndex = Random.Range(1, 101);
                var randomItem = randomIndex <= int.Parse(_costumeBoxData[2, 8]) ? GetRareItem() : GetNormalItem();
                _currentCostumeItemData.Add(randomItem);
                randomItems.Add(randomItem);
            }

            SaveCostumeData();

            return randomItems;
        }

        private CostumeItemData GetNormalItem()
        {
            if (_cachedCommonCostumes.Count == 0)
            {
                Debug.LogWarning("No Common costumes found.");
                return default;
            }

            var randomIndex = Random.Range(0, _cachedCommonCostumes.Count);
            return _cachedCommonCostumes[randomIndex].Clone();
        }

        private CostumeItemData GetRareItem()
        {
            if (_cachedRareCostumes.Count == 0)
            {
                Debug.LogWarning("No Rare costumes found.");
                return default;
            }

            var randomIndex = Random.Range(0, _cachedRareCostumes.Count);
            return _cachedRareCostumes[randomIndex].Clone();
        }

        private void ActivateCostumePanel()
        {
            _uiPanelCostume.Activate();
        }

        public void SortCostumeItems()
        {
            var typeOrder = new List<ECostumeType> { ECostumeType.Weapon, ECostumeType.Hat, ECostumeType.Bag, ECostumeType.Clothes };

            _currentCostumeItemData.Sort((a, b) =>
            {
                if (a == null || b == null) return a == null ? 1 : -1;

                var equippedComparison = b.IsEquipped.CompareTo(a.IsEquipped);
                if (equippedComparison != 0) return equippedComparison;

                var gradeComparison = b.CostumeGrade.CompareTo(a.CostumeGrade);
                if (gradeComparison != 0) return gradeComparison;

                var typeComparison = typeOrder.IndexOf(a.CostumeType).CompareTo(typeOrder.IndexOf(b.CostumeType));
                if (typeComparison != 0) return typeComparison;

                return string.Compare(a.CostumeName, b.CostumeName, StringComparison.InvariantCulture);
            });
        }
    }
}