using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Modules.DesignPatterns.MVVMs;
using ScriptableObjects.Scripts.Items;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Buildings.UI.WareHouses
{
    public class WareHouseViewModel : BaseViewModel
    {
        private readonly WareHouseInventoryModule _wareHouseInventoryModule;

        public WareHouseViewModel(WareHouseInventoryModule wareHouseInventoryModule,
            Action<EMaterialType> onClickButton)
        {
            _wareHouseInventoryModule = wareHouseInventoryModule;

            OnClickButton = onClickButton;

            InitializeButton();
        }

        public List<ButtonData> Buttons { get; private set; } = new();
        private event Action<EMaterialType> OnClickButton;

        private void InitializeButton()
        {
            Buttons.Clear();

            foreach ((EMaterialType materialType, EStageMaterialType stageMaterialType) in VolatileDataManager.Instance
                         .MaterialMappings)
            {
                Sprite sprite = null;

                foreach (ItemSprite itemSprite in DataManager.Instance.ItemDataSo.ItemSprites.Where(itemSprite =>
                             itemSprite.ItemType == EItemType.Material &&
                             itemSprite.StageMaterialType == stageMaterialType)) sprite = itemSprite.Sprite;

                var buttonData = new ButtonData(sprite, materialType, OnClickButton);

                Buttons.Add(buttonData);
            }
        }

        public void UpdateButtonData()
        {
            foreach (ButtonData button in Buttons)
            {
                button.ItemCount =
                    _wareHouseInventoryModule.GetItemCount($"{EItemType.Material}_{button.MaterialType}");
                button.IsInteractable = button.ItemCount > 0;
            }

            OnPropertyChanged(nameof(Buttons));
        }
    }

    public class ButtonData
    {
        public ButtonData(Sprite sprite, EMaterialType materialType, Action<EMaterialType> onClickButton)
        {
            Sprite = sprite;
            MaterialType = materialType;
            OnClickButton = onClickButton;
        }

        public Sprite Sprite { get; set; }
        public EMaterialType MaterialType { get; set; }
        public int ItemCount { get; set; }
        public bool IsInteractable { get; set; }
        public Action<EMaterialType> OnClickButton { get; set; }
    }
}