using System;
using System.Collections.Generic;
using DefaultNamespace;
using Interfaces;
using Units.Creatures.Enums;
using Units.Creatures.Interfaces;
using Unity.VisualScripting;
using EMaterial = Enums.EMaterial;
using IInitializable = Interfaces.IInitializable;

namespace Units.Creatures.Units.Players
{
    public class PlayerInventorySystem : IReferenceRegisterable<IInventoryProperty>, IInitializable
    {
        public int MaxInventorySize => _inventoryProperty.InventorySize;
        public int CurrentInventorySize => materialInventory.Count + productInventory.Count; 
        
        private readonly Dictionary<EMaterial, int> materialInventory = new();
        private readonly Dictionary<EProduct, int> productInventory = new();
        
        private IInventoryProperty _inventoryProperty;

        public void RegisterReference(IInventoryProperty inventoryProperty)
        {
            _inventoryProperty = inventoryProperty;
        }
        
        public void Initialize()
        {
            materialInventory.Clear();
            productInventory.Clear();
        }

        public bool ReceiveMaterial(EMaterial material)
        {
            if (CanReceive())
            {
                Receive(materialInventory, material);
                return true;
            }

            return false;
        }

        public bool ReceiveProduct(EProduct product)
        {
            if (CanReceive())
            {
                Receive(productInventory, product);
                return true;
            }

            return false;
        }

        public void SendMaterial(EMaterial material)
        {
            Send(materialInventory, material);
        }

        public void SendProduct(EProduct product)
        {
            Send(productInventory, product);
        }

        private bool CanReceive() => CurrentInventorySize + 1 <= MaxInventorySize;
        
        private void Receive<T>(Dictionary<T, int> inventory, T item)
        {
            if (inventory.ContainsKey(item))
            {
                if (inventory[item] < MaxInventorySize)
                {
                    inventory[item]++;
                }
            }
            else
            {
                inventory.TryAdd(item, 1);
            }
        }
        
        private void Send<T>(Dictionary<T, int> inventory, T item)
        {
            if (inventory.ContainsKey(item) && inventory[item] > 0)
            {
                inventory[item]--;
            }
        }
    }
}