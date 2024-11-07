using System;
using System.Collections.Generic;
using System.Linq;
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
        [Header("### Unit Data ###")]
        public PlayerDataSO PlayerDataSo;
        public MonsterDataSO MonsterDataSo;
        public GuestDataSO GuestDataSo;
        public DeliveryManDataSO DeliveryManDataSo;
        
        [Space(20), Header("### Building Data ###")]
        public KitchenDataSO KitchenDataSo;
        public StandDataSO StandDataSo;
        public ManagementDeskDataSO ManagementDeskDataSo;
        public DeliveryLodgingDataSO DeliveryLodgingDataSo;
        public WareHouseDataSO WareHouseDataSo;
        
        [Space(20), Header("### Zone Data ###")]
        public HuntingZoneDataSO HuntingZoneDataSo;
        public GuestSpawnZoneDataSo GuestSpawnZoneDataSo;
        
        [Space(20), Header("### Item Data ###")]
        public const int GoldSendingMaximum = 1000;
        public ItemDataSO ItemDataSo;
        
        public GameData KitchenData;
        public GameData KitchenOption1ValueData;
        public GameData KitchenOption2ValueData;
        public GameData KitchenOption1CostData;
        public GameData KitchenOption2CostData;
        public GameData ManagementDeskData;
        public GameData ManagementDeskOption1ValueData;
        public GameData ManagementDeskOption2ValueData;
        public GameData ManagementDeskOption1CostData;
        public GameData ManagementDeskOption2CostData;
    }
}