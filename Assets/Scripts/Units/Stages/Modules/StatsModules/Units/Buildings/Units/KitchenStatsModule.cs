using System;
 using System.Linq;
 using Managers;
 using ScriptableObjects.Scripts.Buildings.Units;
 using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
 using Units.Stages.Units.Items.Enums;
 using Units.Stages.Units.Zones.Units.BuildingZones.Enums;
 using Units.Stages.Units.Zones.Units.BuildingZones.Units;
 using UnityEngine;
 
 namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
 {
     public interface IKitchenStatsModule : IBuildingStatsModule
     {
     }
     
     public class KitchenStatsModule : BuildingStatsModule, IKitchenStatsModule
     {
         public string KitchenName => $"Kitchen {MaterialType}";
         public string KitchenProductName;
         public Sprite KitchenProductSprite;
         public int CurrentKitchenOption1Value;
         public float CurrentKitchenOption2Value;
         public int NextKitchenOption1Value;
         public float NextKitchenOption2Value;
         public int MaxKitchenOption1Level;
         public int MaxKitchenOption2Level;
         public int RequiredGoldToUpgradeKitchenOption1;
         public int RequiredGoldToUpgradeKitchenOption2;
         public int RequiredKitchenLevelToUpgradeOption2;
         
         // TODO : 데이터 저장해야함!
         public int CurrentKitchenLevel;
         public int CurrentKitchenOption1Level;
         public int CurrentKitchenOption2Level;

         public readonly EItemType ItemType;
         public readonly EStageMaterialType StageMaterialType;
         public readonly EItemType InputItemType;
         public readonly EItemType OutputItemType;
         public readonly EMaterialType MaterialType;
         public readonly EBuildingType BuildingType;
         public readonly string BuildingKey;
         public readonly string InputItemKey;
         public readonly string OutputItemKey;

         public int MaxMaterialInventorySize { get; }
         
         public KitchenStatsModule(KitchenDataSO kitchenDataSo, KitchenCustomSetting kitchenCustomSetting) : base(kitchenDataSo)
         {
             MaterialType = kitchenCustomSetting.MaterialType;
             BuildingType = kitchenDataSo.BuildingType;
             InputItemType = kitchenCustomSetting.InputItemType;
             OutputItemType = kitchenCustomSetting.OutputItemType;
             
             BuildingKey = ParserModule.ParseEnumToString(BuildingType, MaterialType);
             InputItemKey = ParserModule.ParseEnumToString(InputItemType, MaterialType);
             OutputItemKey = ParserModule.ParseEnumToString(OutputItemType, MaterialType);

             ItemType = OutputItemType;
             StageMaterialType = VolatileDataManager.Instance.MaterialMappings[MaterialType];
             
             CurrentKitchenLevel = 1;
             CurrentKitchenOption1Level = 1;
             CurrentKitchenOption2Level = 1;

             MaxMaterialInventorySize = kitchenDataSo.BaseMaterialInventorySize;
             
             UpdateKitchenStatsModule();
         }

         private void UpdateKitchenStatsModule()
         {
             KitchenProductName = $"{VolatileDataManager.Instance.MaterialMappings[MaterialType]}";
             KitchenProductSprite = DataManager.Instance.ItemDataSo.ItemSprites.FirstOrDefault(item => item.ItemType == ItemType && item.StageMaterialType == StageMaterialType).Sprite;
             
             var KitchenOption1ValueData = DataManager.Instance.KitchenOption1ValueData.GetData();
             var KitchenOption2ValueData = DataManager.Instance.KitchenOption2ValueData.GetData();
             var KitchenOption1CostData = DataManager.Instance.KitchenOption1CostData.GetData();
             var KitchenOption2CostData = DataManager.Instance.KitchenOption2CostData.GetData();
             var KitchenData = DataManager.Instance.KitchenData.GetData();
             
             CurrentKitchenOption1Value = Enumerable.Range(0, KitchenOption1ValueData.GetLength(0))
                 .Where(i =>
                     KitchenOption1ValueData[i, 1] == $"{ItemType}_{MaterialType}" &&
                     KitchenOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     KitchenOption1ValueData[i, 3] == CurrentKitchenOption1Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenOption1ValueData[i, 4], CurrentKitchenOption1Value))
                 .FirstOrDefault();
             
             CurrentKitchenOption2Value = Enumerable.Range(0, KitchenOption2ValueData.GetLength(0))
                 .Where(i =>
                     KitchenOption2ValueData[i, 1] == $"{ItemType}_{MaterialType}" &&
                     KitchenOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     KitchenOption2ValueData[i, 3] == CurrentKitchenOption2Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenOption2ValueData[i, 4], CurrentKitchenOption2Value))
                 .FirstOrDefault();
             
             NextKitchenOption1Value = Enumerable.Range(0, KitchenOption1ValueData.GetLength(0))
                 .Where(i =>
                     KitchenOption1ValueData[i, 1] == $"{ItemType}_{MaterialType}" &&
                     KitchenOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     KitchenOption1ValueData[i, 3] == (CurrentKitchenOption1Level + 1).ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenOption1ValueData[i, 4], NextKitchenOption1Value))
                 .FirstOrDefault();
             
             NextKitchenOption2Value = Enumerable.Range(0, KitchenOption2ValueData.GetLength(0))
                 .Where(i =>
                     KitchenOption2ValueData[i, 1] == $"{ItemType}_{MaterialType}" &&
                     KitchenOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     KitchenOption2ValueData[i, 3] == (CurrentKitchenOption2Level + 1).ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenOption2ValueData[i, 4], NextKitchenOption2Value))
                 .FirstOrDefault();
             
             RequiredGoldToUpgradeKitchenOption1 = Enumerable.Range(0, KitchenOption1CostData.GetLength(0))
                 .Where(i =>
                     KitchenOption1CostData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                     KitchenOption1CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     KitchenOption1CostData[i, 3] == CurrentKitchenOption1Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenOption1CostData[i, 4], RequiredGoldToUpgradeKitchenOption1))
                 .FirstOrDefault();
             
             RequiredGoldToUpgradeKitchenOption2 = Enumerable.Range(0, KitchenOption2CostData.GetLength(0))
                 .Where(i =>
                     KitchenOption2CostData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                     KitchenOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     KitchenOption2CostData[i, 3] == CurrentKitchenOption2Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenOption2CostData[i, 4], RequiredGoldToUpgradeKitchenOption2))
                 .FirstOrDefault();
             
             RequiredKitchenLevelToUpgradeOption2 = Enumerable.Range(0, KitchenOption2CostData.GetLength(0))
                 .Where(i =>
                     KitchenOption2CostData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                     KitchenOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     KitchenOption2CostData[i, 3] == CurrentKitchenOption2Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenOption2CostData[i, 5], RequiredKitchenLevelToUpgradeOption2))
                 .FirstOrDefault();
             
             MaxKitchenOption1Level = Enumerable.Range(0, KitchenData.GetLength(0))
                 .Where(i =>
                     KitchenData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                     KitchenData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenData[i, 3], MaxKitchenOption1Level))
                 .FirstOrDefault();
             
             MaxKitchenOption2Level = Enumerable.Range(0, KitchenData.GetLength(0))
                 .Where(i =>
                     KitchenData[i, 1] == $"{BuildingType}_{MaterialType}" &&
                     KitchenData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                 .Select(i => ParserModule.ParseOrDefault(KitchenData[i, 4], MaxKitchenOption1Level))
                 .FirstOrDefault();
         }

         public void OnClickUpgradeButtonForKitchenOption1()
         {
             if (RequiredGoldToUpgradeKitchenOption1 <= CurrencyManager.Instance.Gold)
             {
                 CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeKitchenOption1);
                 IncreaseCurrentKitchenOption1Level();
             }
         }

         public void OnClickUpgradeButtonForKitchenOption2()
         {
             if (RequiredGoldToUpgradeKitchenOption2 <= CurrencyManager.Instance.Gold)
             {
                 CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeKitchenOption2);
                 IncreaseCurrentKitchenOption2Level();
             }
         }
         
         public void IncreaseCurrentKitchenLevel()
         {
             CurrentKitchenLevel++;
             
             UpdateKitchenStatsModule();
             GetUIKitchenEnhancement();
         }
         
         public void IncreaseCurrentKitchenOption1Level()
         {
             CurrentKitchenOption1Level++;

             UpdateKitchenStatsModule();
             GetUIKitchenEnhancement();
         }

         public void IncreaseCurrentKitchenOption2Level()
         {
             CurrentKitchenOption2Level++;

             UpdateKitchenStatsModule();
             GetUIKitchenEnhancement();
         }
         
         public void GetUIKitchenEnhancement()
         {
             UIManager.Instance.GetUIKitchenEnhancement(
                 KitchenName,
                 KitchenProductSprite,
                 KitchenProductName,
                 CurrentKitchenOption1Value,
                 CurrentKitchenOption2Value,
                 NextKitchenOption1Value,
                 NextKitchenOption2Value,
                 CurrentKitchenLevel,
                 CurrentKitchenOption1Level,
                 CurrentKitchenOption2Level,
                 MaxKitchenOption1Level,
                 MaxKitchenOption2Level,
                 RequiredGoldToUpgradeKitchenOption1,
                 RequiredGoldToUpgradeKitchenOption2,
                 RequiredKitchenLevelToUpgradeOption2,
                 OnClickUpgradeButtonForKitchenOption1,
                 OnClickUpgradeButtonForKitchenOption2
             );
         }

         public void ReturnUIKitchenEnhancement()
         {
             UIManager.Instance.ReturnUIKitchenEnhancement();
         }
     }
 }