using System;
using Interfaces;
using Managers;
using ScriptableObjects.Scripts.Buildings.Units;
using Units.Modules;
using Units.Modules.FactoryModules.Units;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Buildings.Abstract;
using Units.Stages.Units.Buildings.Modules;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.Units
{
    public interface IManagementDesk : IBuilding, IRegisterReference<IItemFactory>
    {
        
    }

    [Serializable]
    public struct ManagementDeskDefaultSetting
    {
        [Space(10), Header("아이템 보관 장소")]
        public Transform managementDeskInventory;
        
        [Space(10), Header("TradeZone_Player")]
        public Transform InteractionTradePlayer;
        
        [Space(10), Header("TradeZone_NPC")]
        public Transform InteractionTradeNPC;
    }
    
    [Serializable]
    public struct ManagementDeskCustomSetting
    {
        [Header("재화 타입")]
        public ECurrencyType CurrencyType;
    }
    
    public class ManagementDesk : Building, IManagementDesk
    {
        [SerializeField] private ManagementDeskDefaultSetting _managementDeskDefaultSetting;
        [SerializeField] private ManagementDeskCustomSetting _managementDeskCustomSetting;
        
        public override string BuildingKey { get; protected set; }
        public override string InputItemKey { get; protected set; }
        public override string OutputItemKey { get; protected set; }
        public override Transform TradeZoneNpcTransform => _managementDeskDefaultSetting.InteractionTradeNPC;

        private IManagementDeskStatsModule _managementDeskStatsModule;
        private IManagementDeskInventoryModule _managementDeskInventoryModule;
        private IItemFactory _itemFactory;
        private IInteractionTrade _interactionTradePlayer;
        private IInteractionTrade _interactionTradeNPC;
        
        private ManagementDeskDataSO _managementDeskDataSo;

        public void RegisterReference(IItemFactory itemController)
        {
            _managementDeskDataSo = DataManager.Instance.ManagementDeskData;
            
            _itemFactory = itemController;
            BuildingKey = EnumParserModule.ParseEnumToString(_managementDeskDataSo.BuildingType);
            InputItemKey = EnumParserModule.ParseEnumToString(_managementDeskCustomSetting.CurrencyType);
            OutputItemKey = EnumParserModule.ParseEnumToString(_managementDeskCustomSetting.CurrencyType);

            _managementDeskStatsModule = new ManagementDeskStatsModule(_managementDeskDataSo);
            _managementDeskInventoryModule = new ManagementDeskInventoryModule(
                _managementDeskDefaultSetting.managementDeskInventory,
                _managementDeskDefaultSetting.managementDeskInventory,
                _managementDeskStatsModule,
                _itemFactory,
                InputItemKey, OutputItemKey);
            
            _interactionTradePlayer = _managementDeskDefaultSetting.InteractionTradePlayer.GetComponent<IInteractionTrade>();
            _interactionTradePlayer.RegisterReference(_managementDeskInventoryModule.ReceiverTransform, _managementDeskInventoryModule, _managementDeskInventoryModule, BuildingKey, InputItemKey);
            
            _interactionTradeNPC = _managementDeskDefaultSetting.InteractionTradeNPC.GetComponent<IInteractionTrade>();
            _interactionTradeNPC.RegisterReference(_managementDeskInventoryModule.ReceiverTransform, _managementDeskInventoryModule, _managementDeskInventoryModule, BuildingKey, InputItemKey);
        }

        public override void Initialize() { }
        
        private void Update()
        {
            _managementDeskInventoryModule.Update();
        }
    }
}