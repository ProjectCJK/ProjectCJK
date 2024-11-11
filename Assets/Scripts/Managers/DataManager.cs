using System;
using GoogleSheets;
using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Buildings.Units;
using ScriptableObjects.Scripts.Creatures.Units;
using ScriptableObjects.Scripts.Items;
using ScriptableObjects.Scripts.Zones;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

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
        [Header("=== Building GameData ===")]
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
        [Header("--- WareHouse GameData ---")]
        public GameData WareHouseData;
        public GameData WareHouseOption1ValueData;
        public GameData WareHouseOption2ValueData;
        public GameData WareHouseOption1CostData;
        public GameData WareHouseOption2CostData;
        
        [Space(20)] [Header("### Building Data ###")]
        public GameData Building
    }
}