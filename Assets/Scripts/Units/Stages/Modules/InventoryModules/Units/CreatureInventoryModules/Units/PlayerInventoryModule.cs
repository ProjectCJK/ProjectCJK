using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UI.InventoryPanels;
using Units.Stages.Modules.FactoryModules.Units;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Abstract;
using Units.Stages.Units.Buildings.Modules.TradeZones.Units;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Items.Enums;
using Units.Stages.Units.Items.Units;
using UnityEngine;

namespace Units.Stages.Modules.InventoryModules.Units.CreatureInventoryModules.Units
{
    public interface IPlayerInventoryModule : ICreatureInventoryModule
    {
    }

    public class PlayerInventoryModule : CreatureInventoryModule, IPlayerInventoryModule
    {
        public override Dictionary<string, int> Inventory
        {
            get
            {
                if (!GameManager.Instance.ES3Saver.CreatureItems.ContainsKey($"{CreatureType}"))
                {
                    GameManager.Instance.ES3Saver.CreatureItems.TryAdd($"{CreatureType}", new Dictionary<string, int>());
                }
                
                return GameManager.Instance.ES3Saver.CreatureItems[$"{CreatureType}"];
            }
            set
            {
                if (!GameManager.Instance.ES3Saver.CreatureItems.ContainsKey($"{CreatureType}"))
                {
                    GameManager.Instance.ES3Saver.CreatureItems.TryAdd($"{CreatureType}", new Dictionary<string, int>());
                }

                if (!GameManager.Instance.ES3Saver.CreatureItems[$"{CreatureType}"].ContainsKey(value.Keys.ToString()))
                {
                    GameManager.Instance.ES3Saver.CreatureItems[$"{CreatureType}"].TryAdd($"{CreatureType}", int.Parse(value.Values.ToString()));
                }
            }
        }
        
        public override ECreatureType CreatureType { get; }
        public override IItemFactory ItemFactory { get; }
        public override Transform SenderTransform { get; }
        public override Transform ReceiverTransform { get; }

        private readonly InventoryViewModel _inventoryViewModel;
        private readonly CurrentInventoryCountViewModel _currentInventoryCountViewModel;
        
        private CurrentInventoryCountModel _currentInventoryCountView;

        public PlayerInventoryModule(
            Transform senderTransform,
            Transform receiverTransform,
            IInventoryProperty inventoryProperty,
            IItemFactory itemFactory,
            ECreatureType creatureType,
            CurrentInventoryCountView currentInventoryCountView) : base(inventoryProperty)
        {
            SenderTransform = senderTransform;
            ReceiverTransform = receiverTransform;
            ItemFactory = itemFactory;
            CreatureType = creatureType;
            
            InventoryView inventoryView = UIManager.Instance.UI_Panel_Main.InventoryView;

            var inventoryModel = new InventoryModel();
            _inventoryViewModel = new InventoryViewModel(inventoryModel);
            inventoryView.BindViewModel(_inventoryViewModel);
            
            var currentInventoryCountModel = new CurrentInventoryCountModel();
            _currentInventoryCountViewModel = new CurrentInventoryCountViewModel(currentInventoryCountModel);
            currentInventoryCountView.BindViewModel(_currentInventoryCountViewModel);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            UpdateInventoryViewModel();
            UpdateCurrentInventoryCountViewModel();
        }

        public override void RegisterItemReceiver(ITradeZone zone, bool isConnected)
        {
            if (isConnected)
                RegisterZone(zone as IPlayerTradeZone);
            else
                UnregisterZone(zone as IPlayerTradeZone);
        }

        protected override void OnItemReceived(string inputItemKey, IItem item)
        {
            if (Enum.TryParse(inputItemKey, out ECurrencyType currencyType))
            {
                switch (currencyType)
                {
                    case ECurrencyType.Money:
                        QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Get, inputItemKey, item.Count);
                        CurrencyManager.Instance.AddCurrency(ECurrencyType.Gold, item.Count);
                        break;
                }
            }
            else
            {
                QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Get, inputItemKey, 1);
                AddItem(inputItemKey, item.Count);
                UpdateInventoryViewModel();
                UpdateCurrentInventoryCountViewModel();
            }
            
            ItemFactory.ReturnItem(item);
        }

        private void RegisterZone(IPlayerTradeZone zone)
        {
            if (zone.CheckInputAccessorPlayer() && interactionTradeZones.Add(zone))
                Debug.Log($"{CreatureType} => {zone.BuildingKey} 도킹 완료 @~@");

            if (zone.CheckOutputAccessorPlayer() && zone.RegisterItemReceiver(this, true))
                Debug.Log($"{zone.BuildingKey} => {CreatureType} 도킹 완료 @~@");
        }

        private void UnregisterZone(IPlayerTradeZone zone)
        {
            if (zone.CheckInputAccessorPlayer() && interactionTradeZones.Remove(zone))
                Debug.Log($"{CreatureType} => {zone.BuildingKey} 도킹 해제 완료 @~@");

            if (zone.CheckOutputAccessorPlayer() && zone.RegisterItemReceiver(this, false))
                Debug.Log($"{zone.BuildingKey} => {CreatureType} 도킹 해제 완료 @~@");
        }
        
        private void UpdateInventoryViewModel()
        {
            if (GameManager.Instance.ES3Saver.CreatureItems.ContainsKey($"{ECreatureType.Player}"))
            {
                _inventoryViewModel.UpdateValues(
                    GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].GetValueOrDefault($"{EItemType.Material}_{EMaterialType.A}", 0),
                    GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].GetValueOrDefault($"{EItemType.Material}_{EMaterialType.B}", 0),
                    GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].GetValueOrDefault($"{EItemType.Material}_{EMaterialType.C}", 0),
                    GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].GetValueOrDefault($"{EItemType.ProductA}_{EMaterialType.A}", 0),
                    GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].GetValueOrDefault($"{EItemType.ProductA}_{EMaterialType.B}", 0),
                    GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].GetValueOrDefault($"{EItemType.ProductA}_{EMaterialType.C}", 0),
                    GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].GetValueOrDefault($"{EItemType.ProductB}_{EMaterialType.A}", 0));
            }
            else
            {
                _inventoryViewModel.UpdateValues(0, 0, 0, 0, 0, 0, 0);
            }
        }

        private void UpdateCurrentInventoryCountViewModel()
        {
            _currentInventoryCountViewModel.UpdateValues(
                GameManager.Instance.ES3Saver.CreatureItems.ContainsKey($"{ECreatureType.Player}")
                    ? GameManager.Instance.ES3Saver.CreatureItems[$"{ECreatureType.Player}"].Values.Sum()
                    : 0, VolatileDataManager.Instance.Player.PlayerStatsModule.MaxProductInventorySize);
        }

        public override void RemoveItem(string itemKey)
        {
            base.RemoveItem(itemKey);
            
            UpdateInventoryViewModel();
            UpdateCurrentInventoryCountViewModel();
        }
    }
}