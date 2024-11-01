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
        public PlayerDataSO PlayerDataSo;
        public MonsterDataSO MonsterDataSo;
        public GuestDataSO GuestDataSo;
        
        [Space(20), Header("### Building Data ###")]
        public KitchenDataSO KitchenDataSo;
        public StandDataSO StandDataSo;
        public ManagementDeskDataSO ManagementDeskDataSo;
        
        [Space(20), Header("### Zone Data ###")]
        public HuntingZoneDataSO HuntingZoneDataSo;
        public GuestSpawnZoneDataSo GuestSpawnZoneDataSo;
        
        [Space(20), Header("### Item Data ###")]
        public ItemDataSO ItemDataSo;
    }
}