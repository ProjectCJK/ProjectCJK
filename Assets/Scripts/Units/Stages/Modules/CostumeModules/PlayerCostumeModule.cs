using System;
using System.Collections.Generic;
using Managers;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.BattleModules;
using Units.Stages.Modules.SpriteModules;
using UnityEngine;

namespace Units.Stages.Modules.CostumeModules
{
    public class PlayerCostumeModule
    {
        private readonly CreatureSpriteModule _creatureSpriteModule;
        private readonly SpriteRenderer _weaponSprite;

        public PlayerCostumeModule(CreatureSpriteModule creatureSpriteModule, Weapon weapon)
        {
            _creatureSpriteModule = creatureSpriteModule;
            _weaponSprite = weapon.WeaponSprite;
        }

        public void EquipCostume(ECostumeType costumeType, CostumeItemData costumeItemData)
        {
            if (costumeItemData.IsEquipped) return;
            
            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(costumeType))
            {
                UnEquipPreviousCostume(costumeType);
            }

            EquipCurrentCostume(costumeType, costumeItemData);
        }

        private void UnEquipPreviousCostume(ECostumeType costumeType)
        {
            VolatileDataManager.Instance.EquippedCostumes[costumeType].IsEquipped = false;
            DecreaseOptionValues(costumeType);
        }
        
        private void EquipCurrentCostume(ECostumeType costumeType, CostumeItemData costumeItemData)
        {
            costumeItemData.IsEquipped = true;
            IncreaseOptionValues(costumeType, costumeItemData);
            ChangeCostume(costumeType, costumeItemData);
        }

        private void IncreaseOptionValues(ECostumeType costumeType, CostumeItemData costumeItemData)
        {
            List<float> currentOptionValues = costumeItemData.GetCurrentOptionValue();
                
            for (var i = 0; i < currentOptionValues.Count; i++)
            {
                if (VolatileDataManager.Instance.CostumeEquipmentOption.ContainsKey(costumeItemData.CostumeItemOptionDatas[i].CostumeOptionType))
                {
                    VolatileDataManager.Instance.CostumeEquipmentOption[costumeItemData.CostumeItemOptionDatas[i].CostumeOptionType] += currentOptionValues[i];   
                }
                else
                {
                    VolatileDataManager.Instance.CostumeEquipmentOption.TryAdd(costumeItemData.CostumeItemOptionDatas[i].CostumeOptionType, currentOptionValues[i]);
                }
            }
                
            VolatileDataManager.Instance.EquippedCostumes[costumeType] = costumeItemData;
        }

        private void DecreaseOptionValues(ECostumeType costumeType)
        {
            List<float> previousOptionValues = VolatileDataManager.Instance.EquippedCostumes[costumeType].GetCurrentOptionValue();

            for (var i = 0; i < previousOptionValues.Count; i++)
            {
                VolatileDataManager.Instance.CostumeEquipmentOption[VolatileDataManager.Instance.EquippedCostumes[costumeType].CostumeItemOptionDatas[i].CostumeOptionType] -= previousOptionValues[i];
            }
        }

        private void ChangeCostume(ECostumeType costumeType, CostumeItemData costumeItemData)
        {
            var currentSprite = _creatureSpriteModule.CreatureSprite;

            switch (costumeType)
            {
                case ECostumeType.Weapon:
                    _weaponSprite.sprite = costumeItemData.CostumeSprites[1];
                    break;
                case ECostumeType.Clothes:
                    currentSprite.Body.Clear();
                    currentSprite.Leg_Left.Clear();
                    currentSprite.Leg_Right.Clear();
                    currentSprite.Body.Add(costumeItemData.CostumeSprites[1]);
                    currentSprite.Leg_Left.Add(costumeItemData.CostumeSprites[2]);
                    currentSprite.Leg_Right.Add(costumeItemData.CostumeSprites[2]);
                    break;
                case ECostumeType.Bag:
                    currentSprite.Bag.Clear();
                    currentSprite.Bag.Add(costumeItemData.CostumeSprites[1]);
                    break;
                case ECostumeType.Hat:
                    currentSprite.Hat.Clear();
                    currentSprite.Hat.Add(costumeItemData.CostumeSprites[1]);
                    break;
            }

            _creatureSpriteModule.SetSprites(currentSprite);
        }
    }
}