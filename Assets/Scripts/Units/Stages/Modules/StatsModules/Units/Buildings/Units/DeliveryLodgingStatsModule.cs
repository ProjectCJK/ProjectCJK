using System.Collections.Generic;
using System.Linq;
using Managers;
using ScriptableObjects.Scripts.Buildings.Abstract;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Stages.Modules.StatsModules.Units.Buildings.Abstract;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Zones.Units.BuildingZones.Enums;
using UnityEngine;

namespace Units.Stages.Modules.StatsModules.Units.Buildings.Units
{
    public interface IDeliveryLodgingStatsModule : IBuildingStatsModule
    {
        
    }
    
    public class DeliveryLodgingStatsModule : BuildingStatsModule, IDeliveryLodgingStatsModule
    { public string DeliveryLodgingName => $"DeliveryLodging";
        public string DeliveryLodgingProductName;
        public Sprite DeliveryLodgingProductSprite;
        public float CurrentDeliveryLodgingOption1Value;
        public float CurrentDeliveryLodgingOption2Value;
        public float NextDeliveryLodgingOption1Value;
        public float NextDeliveryLodgingOption2Value;
        public int MaxDeliveryLodgingOption1Level;
        public int MaxDeliveryLodgingOption2Level;
        public int RequiredGoldToUpgradeDeliveryLodgingOption1;
        public int RequiredGoldToUpgradeDeliveryLodgingOption2;
        public int RequiredOption1LevelToUpgradeDeliveryLodgingDeskLevel;
        public int RequiredDeliveryLodgingLevelToUpgradeOption2;
         
        // TODO : 데이터 저장해야함!
        public int CurrentDeliveryLodgingLevel;
        public int CurrentDeliveryLodgingOption1Level;
        public int CurrentDeliveryLodgingOption2Level;
        
        public readonly EBuildingType BuildingType;
        private readonly HashSet<IDeliveryMan> _currentSpawnedDeliveryMans;
        public readonly string BuildingKey;
        
        public DeliveryLodgingStatsModule(
            DeliveryLodgingDataSO deliveryLodgingDataSo,
            HashSet<IDeliveryMan> currentSpawnedDeliveryMans)
            : base(deliveryLodgingDataSo)
        {
            _currentSpawnedDeliveryMans = currentSpawnedDeliveryMans;
            BuildingType = deliveryLodgingDataSo.BuildingType;
            
            BuildingKey = ParserModule.ParseEnumToString(BuildingType);
            
            CurrentDeliveryLodgingLevel = 1;
            CurrentDeliveryLodgingOption1Level = 1;
            CurrentDeliveryLodgingOption2Level = 1;
            
            UpdateDeliveryLodgingStatsModule();
        }

         private void UpdateDeliveryLodgingStatsModule()
         {
             var DeliveryLodgingData = DataManager.Instance.DeliveryLodgingData.GetData();
             var DeliveryLodgingOption1ValueData = DataManager.Instance.DeliveryLodgingOption1ValueData.GetData();
             var DeliveryLodgingOption2ValueData = DataManager.Instance.DeliveryLodgingOption2ValueData.GetData();
             var DeliveryLodgingOption1CostData = DataManager.Instance.DeliveryLodgingOption1CostData.GetData();
             var DeliveryLodgingOption2CostData = DataManager.Instance.DeliveryLodgingOption2CostData.GetData();
             
             CurrentDeliveryLodgingOption1Value = Enumerable.Range(0, DeliveryLodgingOption1ValueData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingOption1ValueData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingOption1ValueData[i, 3] == CurrentDeliveryLodgingOption1Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingOption1ValueData[i, 4], CurrentDeliveryLodgingOption1Value))
                 .FirstOrDefault();
             
             CurrentDeliveryLodgingOption2Value = Enumerable.Range(0, DeliveryLodgingOption2ValueData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingOption2ValueData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingOption2ValueData[i, 3] == CurrentDeliveryLodgingOption2Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingOption2ValueData[i, 4], CurrentDeliveryLodgingOption2Value))
                 .FirstOrDefault();
             
             NextDeliveryLodgingOption1Value = Enumerable.Range(0, DeliveryLodgingOption1ValueData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingOption1ValueData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingOption1ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingOption1ValueData[i, 3] == (CurrentDeliveryLodgingOption1Level + 1).ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingOption1ValueData[i, 4], NextDeliveryLodgingOption1Value))
                 .FirstOrDefault();
             
             NextDeliveryLodgingOption2Value = Enumerable.Range(0, DeliveryLodgingOption2ValueData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingOption2ValueData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingOption2ValueData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingOption2ValueData[i, 3] == (CurrentDeliveryLodgingOption2Level + 1).ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingOption2ValueData[i, 4], NextDeliveryLodgingOption2Value))
                 .FirstOrDefault();
             
             RequiredGoldToUpgradeDeliveryLodgingOption1 = Enumerable.Range(0, DeliveryLodgingOption1CostData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingOption1CostData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingOption1CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingOption1CostData[i, 3] == CurrentDeliveryLodgingOption1Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingOption1CostData[i, 4], RequiredGoldToUpgradeDeliveryLodgingOption1))
                 .FirstOrDefault();
             
             RequiredGoldToUpgradeDeliveryLodgingOption2 = Enumerable.Range(0, DeliveryLodgingOption2CostData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingOption2CostData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingOption2CostData[i, 3] == CurrentDeliveryLodgingOption2Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingOption2CostData[i, 4], RequiredGoldToUpgradeDeliveryLodgingOption2))
                 .FirstOrDefault();
             
             RequiredOption1LevelToUpgradeDeliveryLodgingDeskLevel = Enumerable.Range(0, DeliveryLodgingData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingData[i, 5] == CurrentDeliveryLodgingLevel.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingData[i, 6], RequiredOption1LevelToUpgradeDeliveryLodgingDeskLevel))
                 .FirstOrDefault();
             
             RequiredDeliveryLodgingLevelToUpgradeOption2 = Enumerable.Range(0, DeliveryLodgingOption2CostData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingOption2CostData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingOption2CostData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString() &&
                     DeliveryLodgingOption2CostData[i, 3] == CurrentDeliveryLodgingOption2Level.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingOption2CostData[i, 5], RequiredDeliveryLodgingLevelToUpgradeOption2))
                 .FirstOrDefault();
             
             MaxDeliveryLodgingOption1Level = Enumerable.Range(0, DeliveryLodgingData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingData[i, 3], MaxDeliveryLodgingOption1Level))
                 .FirstOrDefault();
             
             MaxDeliveryLodgingOption2Level = Enumerable.Range(0, DeliveryLodgingData.GetLength(0))
                 .Where(i =>
                     DeliveryLodgingData[i, 1] == $"{BuildingKey}" &&
                     DeliveryLodgingData[i, 2] == VolatileDataManager.Instance.CurrentStageLevel.ToString())
                 .Select(i => ParserModule.ParseOrDefault(DeliveryLodgingData[i, 4], MaxDeliveryLodgingOption1Level))
                 .FirstOrDefault();
             
             UpdateDeliveryManMoveSpeed();
         }

         private void UpdateDeliveryManMoveSpeed()
         {
             foreach (var deliveryMan in _currentSpawnedDeliveryMans)
             {
                 deliveryMan.SetMovementSpeed(CurrentDeliveryLodgingOption1Value);
             }
         }

         private void OnClickUpgradeButtonForDeliveryLodgingOption1()
         {
             if (RequiredGoldToUpgradeDeliveryLodgingOption1 <= CurrencyManager.Instance.Gold)
             {
                 CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeDeliveryLodgingOption1);
                 IncreaseCurrentDeliveryLodgingOption1Level();
             }
         }

         private void OnClickUpgradeButtonForDeliveryLodgingOption2()
         {
             if (RequiredGoldToUpgradeDeliveryLodgingOption2 <= CurrencyManager.Instance.Gold)
             {
                 CurrencyManager.Instance.RemoveGold(RequiredGoldToUpgradeDeliveryLodgingOption2);
                 IncreaseCurrentDeliveryLodgingOption2Level();
             }
         }
         
         private void IncreaseCurrentDeliveryLodgingOption1Level()
         {
             CurrentDeliveryLodgingOption1Level++;

             if (CurrentDeliveryLodgingOption1Level >= RequiredOption1LevelToUpgradeDeliveryLodgingDeskLevel)
             {
                 CurrentDeliveryLodgingLevel++;
             }

             UpdateDeliveryLodgingStatsModule();
             GetUIDeliveryLodgingEnhancement();
         }

         private void IncreaseCurrentDeliveryLodgingOption2Level()
         {
             CurrentDeliveryLodgingOption2Level++;

             UpdateDeliveryLodgingStatsModule();
             GetUIDeliveryLodgingEnhancement();
         }
         
         public void GetUIDeliveryLodgingEnhancement()
         {
             UIManager.Instance.GetPanelBuildingEnhancement(
                 DeliveryLodgingName,
                 DeliveryLodgingProductSprite,
                 DeliveryLodgingProductName,
                 CurrentDeliveryLodgingOption1Value,
                 CurrentDeliveryLodgingOption2Value,
                 NextDeliveryLodgingOption1Value,
                 NextDeliveryLodgingOption2Value,
                 CurrentDeliveryLodgingLevel,
                 CurrentDeliveryLodgingOption1Level,
                 CurrentDeliveryLodgingOption2Level,
                 MaxDeliveryLodgingOption1Level,
                 MaxDeliveryLodgingOption2Level,
                 RequiredGoldToUpgradeDeliveryLodgingOption1,
                 RequiredGoldToUpgradeDeliveryLodgingOption2,
                 RequiredDeliveryLodgingLevelToUpgradeOption2,
                 OnClickUpgradeButtonForDeliveryLodgingOption1,
                 OnClickUpgradeButtonForDeliveryLodgingOption2
             );
         }

         public void ReturnUIDeliveryLodgingEnhancement()
         {
             UIManager.Instance.ReturnPanelBuildingEnhancement();
         }
    }
}