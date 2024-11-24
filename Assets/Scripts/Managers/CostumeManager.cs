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

        public List<float> GetCurrentOptionValue()
        {
            return CostumeItemOptionDatas.Select(costumeItemOptionData =>
                costumeItemOptionData.BaseOptionValue +
                (CurrentLevel - 1) * costumeItemOptionData.UpgradeOptionValue).ToList();
        }

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
    public class CostumeDataSerializable
    {
        public string CostumeName;
        public int CurrentLevel;
        public int CurrentExp;
        public bool IsEquipped;
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

        private List<CostumeItemData> _currentCostumeItemData;

        public void RegisterReference()
        {
            _costumeData = DataManager.Instance.CostumeData.GetData();
            _costumeBoxData = DataManager.Instance.CostumeBoxData.GetData();
            _costumeParamData = DataManager.Instance.CostumeParamData.GetData();
            _costumeUpgradeData = DataManager.Instance.CostumeUpgradeData.GetData();

            _cachedCommonCostumes = new List<CostumeItemData>();
            _cachedRareCostumes = new List<CostumeItemData>();
            _currentCostumeItemData = new List<CostumeItemData>();

            CacheCostumes();
            CacheGachaBackgroundSprites();
            LoadCostumeData();

            UIManager.Instance.UI_Panel_Main.UI_Panel_Costume_Gacha.RegisterReference(_backgroundImageCache, _frontGroundImageCache);
            UIManager.Instance.UI_Panel_Main.UI_Panel_Costume.RegisterReference(_frontGroundImageCache, _currentCostumeItemData);

            UIManager.Instance.UI_Panel_Main.UI_Button_CostumeGachaPanel.onClick.AddListener(ActivateCostumeGacha);
            UIManager.Instance.UI_Panel_Main.UI_Button_CostumePanel.onClick.AddListener(ActivateCostumePanel);
        }

        private void CacheGachaBackgroundSprites()
        {
            foreach (var sprite in DataManager.Instance.CostumeBackBackgroundSpriteSO.CostumeBackBackgroundSprites)
                _backgroundImageCache.TryAdd(sprite.Grade, sprite.Sprite);

            foreach (var sprite in DataManager.Instance.CostumeFrontBackgroundSpriteSo.CostumeFrontBackgroundSprites)
                _frontGroundImageCache.TryAdd(new Tuple<ECostumeType, ECostumeGrade>(sprite.Type, sprite.Grade), sprite.Sprite);
        }

        private void CacheCostumes()
        {
            for (var i = 2; i < _costumeData.GetLength(0); i++)
            {
                var costumeItem = new CostumeItemData
                {
                    CostumeName = _costumeData[i, 1],
                    CostumeType = Enum.Parse<ECostumeType>(_costumeData[i, 3]),
                    CostumeGrade = Enum.Parse<ECostumeGrade>(_costumeData[i, 5]),
                    SpriteIndex = int.Parse(_costumeData[i, 4]),
                    CurrentLevel = 1,
                    MaxLevel = CalculateMaxLevel(_costumeData[i, 5]),
                    MaxExps = CalculateMaxExps(_costumeData[i, 5]),
                    MaterialValues = CalculateMaterialValues(_costumeData[i, 5]),
                    CostumeItemOptionDatas = ParseCostumeOptions(i)
                };
                costumeItem.CostumeSprites = RetrieveCostumeSprites(costumeItem);

                if (costumeItem.CostumeGrade == ECostumeGrade.Common)
                    _cachedCommonCostumes.Add(costumeItem);
                else
                    _cachedRareCostumes.Add(costumeItem);
            }
        }

        private List<Sprite> RetrieveCostumeSprites(CostumeItemData item)
        {
            return item.CostumeType switch
            {
                ECostumeType.Weapon => DataManager.Instance.CostumeSpriteSo.WeaponCostumeSprites[item.SpriteIndex].Sprites,
                ECostumeType.Clothes => DataManager.Instance.CostumeSpriteSo.BodyCostumeSprites[item.SpriteIndex].Sprites,
                ECostumeType.Bag => DataManager.Instance.CostumeSpriteSo.BagCostumeSprites[item.SpriteIndex].Sprites,
                ECostumeType.Hat => DataManager.Instance.CostumeSpriteSo.HatCostumeSprites[item.SpriteIndex].Sprites,
                _ => new List<Sprite>()
            };
        }

        private int CalculateMaxLevel(string grade)
        {
            return Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                .Where(index => _costumeUpgradeData[index, 1] == grade)
                .Select(index => int.Parse(_costumeUpgradeData[index, 2]))
                .DefaultIfEmpty(0).Max() + 1;
        }

        private List<int> CalculateMaxExps(string grade)
        {
            return Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                .Where(index => _costumeUpgradeData[index, 1] == grade)
                .Select(index => int.Parse(_costumeUpgradeData[index, 3])).ToList();
        }

        private List<int> CalculateMaterialValues(string grade)
        {
            return Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                .Where(index => _costumeUpgradeData[index, 1] == grade)
                .Select(index => int.Parse(_costumeUpgradeData[index, 4])).ToList();
        }

        private List<CostumeItemOptionData> ParseCostumeOptions(int index)
        {
            var options = new List<CostumeItemOptionData>();
            if (Enum.TryParse(_costumeData[index, 7], out ECostumeOptionType option1))
                options.Add(CreateOptionData(option1));

            if (Enum.TryParse(_costumeData[index, 8], out ECostumeOptionType option2))
                options.Add(CreateOptionData(option2));

            return options;
        }

        private CostumeItemOptionData CreateOptionData(ECostumeOptionType optionType)
        {
            var paramIndex = Enumerable.Range(0, _costumeParamData.GetLength(0))
                .FirstOrDefault(index => _costumeParamData[index, 4] == $"{optionType}");

            if (paramIndex < 0) return null;

            return new CostumeItemOptionData
            {
                CostumeOptionType = optionType,
                CostumeOptionCalculateType = Enum.Parse<ECostumeOptionCalculateType>(_costumeParamData[paramIndex, 5]),
                OptionDescription = _costumeParamData[paramIndex, 6],
                BaseOptionValue = float.Parse(_costumeParamData[paramIndex, 7]),
                UpgradeOptionValue = float.Parse(_costumeParamData[paramIndex, 8])
            };
        }

        private void ActivateCostumeGacha()
        {
            var gachaItems = GetRandomItem(int.Parse(_costumeBoxData[2, 9]), int.Parse(_costumeBoxData[2, 10]));

            SortCostumeItems();
            UIManager.Instance.UI_Panel_Main.UI_Panel_Costume_Gacha.Activate(gachaItems);
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
                var rareOrNormalItem = randomIndex <= int.Parse(_costumeBoxData[2, 8]) ? GetRareItem() : GetNormalItem();
                _currentCostumeItemData.Add(rareOrNormalItem);
                randomItems.Add(rareOrNormalItem);
            }

            SaveCostumeData();

            return randomItems;
        }

        private CostumeItemData GetNormalItem()
        {
            if (_cachedCommonCostumes.Count == 0) return null;
            var randomIndex = Random.Range(0, _cachedCommonCostumes.Count);
            return _cachedCommonCostumes[randomIndex].Clone();
        }

        private CostumeItemData GetRareItem()
        {
            if (_cachedRareCostumes.Count == 0) return null;
            var randomIndex = Random.Range(0, _cachedRareCostumes.Count);
            return _cachedRareCostumes[randomIndex].Clone();
        }

        private void ActivateCostumePanel()
        {
            UIManager.Instance.UI_Panel_Main.UI_Panel_Costume.Activate();
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
                return typeComparison != 0
                    ? typeComparison
                    : string.Compare(a.CostumeName, b.CostumeName, StringComparison.InvariantCulture);
            });
        }
        
        public void SaveCostumeData()
        {
            GameManager.Instance.ES3Saver.CurrentCostumes = _currentCostumeItemData
                .Select(costume => new CostumeDataSerializable
                {
                    CostumeName = costume.CostumeName,
                    CurrentLevel = costume.CurrentLevel,
                    CurrentExp = costume.CurrentExp,
                    IsEquipped = costume.IsEquipped
                }).ToList();
            Debug.Log("Costume data saved.");
        }

        private void LoadCostumeData()
        {
            var savedData = GameManager.Instance.ES3Saver.CurrentCostumes;
            if (savedData == null || savedData.Count == 0) return;

            foreach (var data in savedData)
            {
                var baseItem = _cachedCommonCostumes.Concat(_cachedRareCostumes)
                    .FirstOrDefault(item => item.CostumeName == data.CostumeName);

                if (baseItem != null)
                {
                    var newItem = baseItem.Clone();
                    newItem.CurrentLevel = data.CurrentLevel;
                    newItem.CurrentExp = data.CurrentExp;
                    newItem.IsEquipped = data.IsEquipped;
                    _currentCostumeItemData.Add(newItem);
                    
                    if (data.IsEquipped)
                    {
                        VolatileDataManager.Instance.EquippedCostumes[newItem.CostumeType] = newItem;
                        VolatileDataManager.Instance.Player.PlayerCostumeModule.UpdateEquippedCostumeStats(newItem, false); // 스탯 증가
                    }
                }
            }
        }
    }
}