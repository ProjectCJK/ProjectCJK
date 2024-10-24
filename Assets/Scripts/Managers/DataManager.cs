using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.HuntingZones;
using ScriptableObjects.Scripts.Items;

namespace Managers
{
    public class DataManager : SingletonMono<DataManager>
    {
        public ItemDataSO ItemData;
        public PlayerDataSO PlayerData;
        public MonsterDataSO MonsterData;
        public BuildingDataSO KitchenData;
        public BuildingDataSO StandData;
        public HuntingZoneDataSO HuntingZoneData;
    }
}