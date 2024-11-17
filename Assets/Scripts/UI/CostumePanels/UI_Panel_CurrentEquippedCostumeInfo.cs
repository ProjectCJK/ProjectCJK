using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CostumePanels
{
    public class UI_Panel_CurrentEquippedCostumeInfo : MonoBehaviour
    {
        [SerializeField] private Image equippedWeaponImage;
        [SerializeField] private Image equippedClothesImage;
        [SerializeField] private Image equippedHatImage;
        [SerializeField] private Image equippedBagImage;
        
        [Space(20), SerializeField] private TextMeshProUGUI revenueGrowthText;
        [SerializeField] private TextMeshProUGUI damageGrowthText;
        [SerializeField] private TextMeshProUGUI inventorySizeGrowthText;
        [SerializeField] private TextMeshProUGUI healthGrowthText;

        public void Activate()
        {
            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Weapon))
            {
                equippedWeaponImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Weapon].CostumeSprites[0];
                equippedWeaponImage.gameObject.SetActive(true);
            }
            else
            {
                equippedWeaponImage.gameObject.SetActive(false);
            }
            
            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Clothes))
            {
                equippedClothesImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Clothes].CostumeSprites[0];
                equippedClothesImage.gameObject.SetActive(true);
            }
            else
            {
                equippedClothesImage.gameObject.SetActive(false);
            }
            
            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Hat))
            {
                equippedHatImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Hat].CostumeSprites[0];
                equippedHatImage.gameObject.SetActive(true);
            }
            else
            {
                equippedHatImage.gameObject.SetActive(false);
            }

            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Bag))
            {
                equippedBagImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Bag].CostumeSprites[0];
                equippedBagImage.gameObject.SetActive(true);
            }
            else
            {
                equippedBagImage.gameObject.SetActive(false);
            }

            damageGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.Damage}";
            inventorySizeGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.MaxProductInventorySize}";
            healthGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.MovementSpeed}";
            revenueGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.RevenueGrowth}";

            gameObject.SetActive(true);
        }
    }
}