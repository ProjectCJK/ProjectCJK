using System;
using System.Collections.Generic;
using System.Linq;
using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Sprites;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum ECostumeType
    {
        Weapon,
        Clothes,
        Bag,
        Hat
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
    public struct CostumeItemData
    {
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
    }
    
    [Serializable]
    public struct CostumeItemOptionData
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
        private readonly UI_Panel_Costume_Gacha _uiPanelCostumeGacha = UIManager.Instance.UI_Panel_CostumeGacha;

        private readonly List<CostumeItemData> _currentCostumeItemData = new();
        
        public void RegisterReference()
        {
            _costumeData = DataManager.Instance.CostumeData.GetData();
            _costumeBoxData = DataManager.Instance.CostumeBoxData.GetData();
            _costumeParamData = DataManager.Instance.CostumeParamData.GetData();
            _costumeUpgradeData = DataManager.Instance.CostumeUpgradeData.GetData();
            
            CacheCostumes();
            CacheGachaBackgroundSprites();
            
            _uiPanelCostumeGacha.RegisterReference(_backgroundImageCache, _frontGroundImageCache);
            UIManager.Instance.Button_Costume.onClick.AddListener(ActivateCostumeGacha);
        }

        private void CacheGachaBackgroundSprites()
        {
            foreach (CostumeBackBackgroundSprite costumeBackBackgroundSprite in DataManager.Instance.CostumeBackBackgroundSpriteSO.CostumeBackBackgroundSprites)
            {
                _backgroundImageCache.TryAdd(costumeBackBackgroundSprite.Grade, costumeBackBackgroundSprite.Sprite);
            }
            
            foreach (CostumeFrontBackgroundSprite costumeFrontBackgroundSprite in DataManager.Instance.CostumeFrontBackgroundSpriteSo.CostumeFrontBackgroundSprites)
            {
                _frontGroundImageCache.TryAdd(new Tuple<ECostumeType, ECostumeGrade>(costumeFrontBackgroundSprite.Type, costumeFrontBackgroundSprite.Grade), costumeFrontBackgroundSprite.Sprite);
            }
        }

        private void CacheCostumes()
        {
            _cachedCommonCostumes = new List<CostumeItemData>();
            _cachedRareCostumes = new List<CostumeItemData>();

            for (var i = 2; i < _costumeData.GetLength(0); i++)
            {
                var costumeItem = new CostumeItemData();

                costumeItem.CostumeType = Enum.Parse<ECostumeType>(_costumeData[i, 3]);
                costumeItem.CostumeGrade = Enum.Parse<ECostumeGrade>(_costumeData[i, 5]);
                costumeItem.SpriteIndex = int.Parse(_costumeData[i, 4]);
                
                costumeItem.CostumeSprites = costumeItem.CostumeType switch
                {
                    ECostumeType.Weapon => DataManager.Instance.CostumeSpriteSo
                        .WeaponCostumeSprites[costumeItem.SpriteIndex].Sprites,
                    ECostumeType.Clothes => DataManager.Instance.CostumeSpriteSo
                        .BodyCostumeSprites[costumeItem.SpriteIndex].Sprites,
                    ECostumeType.Bag => DataManager.Instance.CostumeSpriteSo.BagCostumeSprites[costumeItem.SpriteIndex]
                        .Sprites,
                    ECostumeType.Hat => DataManager.Instance.CostumeSpriteSo.HatCostumeSprites[costumeItem.SpriteIndex]
                        .Sprites,
                    _ => costumeItem.CostumeSprites
                };

                costumeItem.CurrentLevel = 1;
                costumeItem.MaxLevel = Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                    .Where(i => _costumeUpgradeData[i, 1] == $"{costumeItem.CostumeGrade}")
                    .Select(i => int.Parse(_costumeUpgradeData[i, 2]))  // 레벨을 int로 파싱
                    .DefaultIfEmpty(0)  // 데이터가 없는 경우 기본값을 설정
                    .Max() + 1;
                
                costumeItem.CurrentExp = 0;
                costumeItem.MaxExps = Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                    .Where(i => _costumeUpgradeData[i, 1] == $"{costumeItem.CostumeGrade}")  // CostumeGrade가 일치하는 행 필터링
                    .Select(i => int.Parse(_costumeUpgradeData[i, 3]))  // Upgrade 값 파싱
                    .ToList();
                
                costumeItem.MaterialValues = Enumerable.Range(0, _costumeUpgradeData.GetLength(0))
                    .Where(i => _costumeUpgradeData[i, 1] == $"{costumeItem.CostumeGrade}")  // CostumeGrade가 일치하는 행 필터링
                    .Select(i => int.Parse(_costumeUpgradeData[i, 4]))  // Material 값 파싱
                    .ToList();
                
                costumeItem.CostumeItemOptionDatas = new List<CostumeItemOptionData>();
                List<ECostumeOptionType> options = GetCostumeOptions(i);
                foreach (ECostumeOptionType costumeOptionType in options)
                {
                    var costumeItemOptionData = new CostumeItemOptionData();
    
                    // CostumeOptionType 설정
                    var paramIndex = Enumerable.Range(0, _costumeParamData.GetLength(0))
                        .FirstOrDefault(i => _costumeParamData[i, 4] == $"{costumeOptionType}");

                    if (paramIndex >= 0)
                    {
                        costumeItemOptionData.CostumeOptionType = Enum.Parse<ECostumeOptionType>(_costumeParamData[paramIndex, 4]);
        
                        // CostumeOptionCalculateType 설정
                        costumeItemOptionData.CostumeOptionCalculateType = Enum.Parse<ECostumeOptionCalculateType>(_costumeParamData[paramIndex, 5]);
        
                        // OptionDescription 설정
                        costumeItemOptionData.OptionDescription = _costumeParamData[paramIndex, 6];
        
                        // BaseOptionValue와 UpgradeOptionValue 설정 (Common과 Rare에 따라 다름)
                        if (costumeItem.CostumeGrade == ECostumeGrade.Common)
                        {
                            costumeItemOptionData.BaseOptionValue = float.Parse(_costumeParamData[paramIndex, 7]);  // CommonValue 열
                            costumeItemOptionData.UpgradeOptionValue = float.Parse(_costumeParamData[paramIndex, 8]);  // CommonUpgrade 열
                        }
                        else if (costumeItem.CostumeGrade == ECostumeGrade.Rare)
                        {
                            costumeItemOptionData.BaseOptionValue = float.Parse(_costumeParamData[paramIndex, 9]);  // RareValue 열
                            costumeItemOptionData.UpgradeOptionValue = float.Parse(_costumeParamData[paramIndex, 10]);  // RareUpgrade 열
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

        private void ActivateCostumeGacha()
        {
            List<CostumeItemData> costumeItems = GetRandomItem(int.Parse(_costumeBoxData[2, 9]), int.Parse(_costumeBoxData[2, 10]));
            
            _uiPanelCostumeGacha.Activate(costumeItems);
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
                randomItems.Add(randomItem);
            }

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
            return _cachedCommonCostumes[randomIndex];
        }

        private CostumeItemData GetRareItem()
        {
            if (_cachedRareCostumes.Count == 0)
            {
                Debug.LogWarning("No Rare costumes found.");
                return default;
            }

            var randomIndex = UnityEngine.Random.Range(0, _cachedRareCostumes.Count);
            return _cachedRareCostumes[randomIndex];
        }
    }
}