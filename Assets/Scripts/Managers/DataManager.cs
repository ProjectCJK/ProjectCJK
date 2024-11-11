using System;
using System.Collections.Generic;
using GoogleSheets;
using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Buildings.Units;
using ScriptableObjects.Scripts.Creatures.Units;
using ScriptableObjects.Scripts.Items;
using ScriptableObjects.Scripts.Zones;
using TMPro;
using Units.Stages.Units.Items.Enums;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
        [Space(20)] [Header("### Item Data ###")]
        public const int GoldSendingMaximum = 1000;

        [Header("### Unit Data ###")] public PlayerDataSO PlayerDataSo;

        public MonsterDataSO MonsterDataSo;
        public GuestDataSO GuestDataSo;
        public DeliveryManDataSO DeliveryManDataSo;
        public HunterDataSO HunterDataSo;

        [Space(20)] [Header("### Building Data ###")]
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
        [Header("=== Quest GameData ===")]
        public GameData QuestData;
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

        [Space(20), Header("### TMP_SpriteAssets ###")]
        public readonly Dictionary<string, TMP_SpriteAsset> TMP_SpriteAssets = new();
        
        public TMP_SpriteAsset TMP_SpriteAssets_Costumes;
        public TMP_SpriteAsset TMP_SpriteAssets_Goods;
        public TMP_SpriteAsset TMP_SpriteAssets_Materials;
        public TMP_SpriteAsset TMP_SpriteAssets_Products;
        public TMP_SpriteAsset TMP_SpriteAssets_Monsters;
        public TMP_SpriteAsset TMP_SpriteAssets_Others;
        
        [Space(20), Header("### SpriteAssets ###")]
        public readonly Dictionary<string, Texture2D> SpriteAssets = new();
        
        public Texture2D SpriteAssets_Costumes;
        public Texture2D SpriteAssets_Goods;
        public Texture2D SpriteAssets_Materials;
        public Texture2D SpriteAssets_Products;
        public Texture2D SpriteAssets_Monsters;
        public Texture2D SpriteAssets_Others;
        
        [Space(20)] [Header("### UI ###")]
        [Space(20), Header("--- BuildingUpgradePanel GameData ---")]
        public GameData BuildingUpgradePanel;
        
        protected override void Awake()
        {
            CreateTMPSpriteAssetsDictionary();
            CreateSpriteAssetsDictionary();
        }
        
        private void CreateTMPSpriteAssetsDictionary()
        {
            TMP_SpriteAssets.TryAdd(nameof(TMP_SpriteAssets_Costumes), TMP_SpriteAssets_Costumes);
            TMP_SpriteAssets.TryAdd(nameof(TMP_SpriteAssets_Goods), TMP_SpriteAssets_Goods);
            TMP_SpriteAssets.TryAdd(nameof(TMP_SpriteAssets_Materials), TMP_SpriteAssets_Materials);
            TMP_SpriteAssets.TryAdd(nameof(TMP_SpriteAssets_Products), TMP_SpriteAssets_Products);
            TMP_SpriteAssets.TryAdd(nameof(TMP_SpriteAssets_Monsters), TMP_SpriteAssets_Monsters);
            TMP_SpriteAssets.TryAdd(nameof(TMP_SpriteAssets_Others), TMP_SpriteAssets_Others);
        }
        
        private void CreateSpriteAssetsDictionary()
        {
            SpriteAssets.TryAdd(nameof(SpriteAssets_Costumes), SpriteAssets_Costumes);
            SpriteAssets.TryAdd(nameof(SpriteAssets_Goods), SpriteAssets_Goods);
            SpriteAssets.TryAdd(nameof(SpriteAssets_Materials), SpriteAssets_Materials);
            SpriteAssets.TryAdd(nameof(SpriteAssets_Products), SpriteAssets_Products);
            SpriteAssets.TryAdd(nameof(SpriteAssets_Monsters), SpriteAssets_Monsters);
            SpriteAssets.TryAdd(nameof(SpriteAssets_Others), SpriteAssets_Others);
        }
    }
}