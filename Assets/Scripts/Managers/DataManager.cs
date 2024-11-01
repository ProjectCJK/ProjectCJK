using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Units;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using ScriptableObjects.Scripts.Items;
using ScriptableObjects.Scripts.Zones;
using UnityEngine;

namespace Managers
{
    public class DataManager : SingletonMono<DataManager>
    {
        [Header("### Unit Data ###")]
        public PlayerDataSO PlayerData;
        public MonsterDataSO MonsterData;
        public GuestDataSO GuestData;
        
        [Space(20), Header("### Building Data ###")]
        public KitchenDataSO KitchenData;
        public StandDataSO StandData;
        public ManagementDeskDataSO ManagementDeskData;
        
        [Space(20), Header("### Zone Data ###")]
        public HuntingZoneDataSO HuntingZoneData;
        public GuestSpawnZoneDataSo GuestSpawnZoneDataSo;
        
        [Space(20), Header("### Item Data ###")]
        public ItemDataSO ItemData;
    }
}