using Modules.DesignPatterns.Singletons;
using ScriptableObjects.Scripts.Buildings;
using ScriptableObjects.Scripts.Buildings.Units;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using ScriptableObjects.Scripts.HuntingZones;
using ScriptableObjects.Scripts.Items;

namespace Managers
{
    public class DataManager : SingletonMono<DataManager>
    {
        public ItemDataSO ItemData;
        public PlayerDataSO PlayerData;
        public MonsterDataSO MonsterData;
        public GuestDataSO GuestData;
        public KitchenDataSO KitchenData;
        public StandDataSO StandData;
        public ManagementDeskDataSO ManagementDeskData;
        public HuntingZoneDataSO HuntingZoneData;
    }
}