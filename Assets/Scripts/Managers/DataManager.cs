using System;
using System.Collections.Generic;
using GoogleSheets;
using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Buildings.Units;
using ScriptableObjects.Scripts.Creatures.Units;
using ScriptableObjects.Scripts.Items;
using ScriptableObjects.Scripts.Levels;
using ScriptableObjects.Scripts.Sprites;
using ScriptableObjects.Scripts.Zones;
using TMPro;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Object = UnityEngine.Object;

namespace Managers
{
    [Serializable]
    public struct ItemPrice
    {
        public EItemType ItemType;
        public EMaterialType MaterialType;
        public int Price;
    }

    public class DataManager : SingletonMono<DataManager>
    {
        [Header("### Tracker Prefab ###")]
        public GameObject ObjectTrackerPrefab;
        
        [Space(20)] [Header("### Item Data ###")]
        public const int GoldSendingMaximum = 1000;

        [Header("### Unit Data ###")]
        public PlayerDataSO PlayerDataSo;
        public MonsterDataSO MonsterDataSo;
        public GuestDataSO GuestDataSo;
        public DeliveryManDataSO DeliveryManDataSo;
        public HunterDataSO HunterDataSo;
        public ZombieDataSO ZombieDataSo;

        [Space(20)] [Header("### Building Data ###")]
        public LevelPrefabSO levelPrefabSo;
        public KitchenDataSO KitchenDataSo;
        public StandDataSO StandDataSo;
        public ManagementDeskDataSO ManagementDeskDataSo;
        public DeliveryLodgingDataSO DeliveryLodgingDataSo;
        public WareHouseDataSO WareHouseDataSo;

        [Space(20)] [Header("### Zone Data ###")]
        public HuntingZoneDataSO HuntingZoneDataSo;

        public GuestSpawnZoneDataSo GuestSpawnZoneDataSo;
        public ItemDataSO ItemDataSo;

        [Space(20), Header("### GameData ###")]
        [Header("=== Level GameData ===")]
        public GameData LevelData;
        
        [Header("=== Quest GameData ===")]
        public GameData QuestData;

        [Header("=== Stage GameData ===")]
        public GameData StageData;
        
        [Space(20), Header("=== Building GameData ===")]
        [Header("--- Kitchen GameData ---")]
        public GameData KitchenData;
        public GameData KitchenOption1ValueData;
        public GameData KitchenOption2ValueData;
        public GameData KitchenOption1CostData;
        public GameData KitchenOption2CostData;
        
        [Space(20), Header("--- ManagementDesk GameData ---")]
        public GameData ManagementDeskData;
        public GameData ManagementDeskOption1ValueData;
        public GameData ManagementDeskOption2ValueData;
        public GameData ManagementDeskOption1CostData;
        public GameData ManagementDeskOption2CostData;
        
        [Space(20), Header("--- DeliveryLodging GameData ---")]
        public GameData DeliveryLodgingData;
        public GameData DeliveryLodgingOption1ValueData;
        public GameData DeliveryLodgingOption2ValueData;
        public GameData DeliveryLodgingOption1CostData;
        public GameData DeliveryLodgingOption2CostData;
        
        [Space(20), Header("--- WareHouse GameData ---")]
        public GameData WareHouseData;
        public GameData WareHouseOption1ValueData;
        public GameData WareHouseOption2ValueData;
        public GameData WareHouseOption1CostData;
        public GameData WareHouseOption2CostData;
        
        [Space(20), Header("--- Costume GameData ---")]
        public GameData CostumeData;
        public GameData CostumeBoxData;
        public GameData CostumeParamData;
        public GameData CostumeUpgradeData;
        public CostumeSpriteSO CostumeSpriteSo;
        public CostumeBackBackgroundSpriteSO CostumeBackBackgroundSpriteSO;
        public CostumeFrontBackgroundSpriteSO CostumeFrontBackgroundSpriteSo;
        
        [Space(20), Header("### TMP_SpriteAssets ###")]
        public TMP_SpriteAsset TMP_SpriteDatas;
        
        [Space(20), Header("### SpriteAssets ###")]
        public Sprite[] SpriteDatas;
        
        [Space(20)] [Header("### UI ###")]
        [Header("--- BuildingUpgradePanel GameData ---")]
        public GameData BuildingUpgradePanel;

        [Space(20), Header("### ItemAssets ###")]
        public Dictionary<string, Sprite> ItemSprites;

        [Space(20), Header("### CurrencyAssets ###")]
        public Dictionary<string, Sprite> CurrencySprites;

        public Sprite GetCurrencyIcon(ECurrencyType currencyType)
        {
            var spriteIndex = 0;
            
            switch (currencyType)
            {
                case ECurrencyType.Money:
                    spriteIndex = 0;
                    break;
                case ECurrencyType.Diamond:
                    spriteIndex = 2;
                    break;
                case ECurrencyType.RedGem:
                    spriteIndex = 4;
                    break;
                case ECurrencyType.Star:
                    spriteIndex = 110;
                    break;
            }

            return SpriteDatas[spriteIndex];
        }
    }
}