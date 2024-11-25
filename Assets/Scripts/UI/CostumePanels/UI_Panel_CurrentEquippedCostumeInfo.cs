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
        
        [Space(20), SerializeField] private Image playerWeaponImage;
        [SerializeField] private Image playerClothesImage;
        [SerializeField] private List<Image> playerShoesImage;
        [SerializeField] private Image playerHatImage;
        [SerializeField] private Image playerBagImage;
        
        [Space(20), SerializeField] private TextMeshProUGUI revenueGrowthText;
        [SerializeField] private TextMeshProUGUI damageGrowthText;
        [SerializeField] private TextMeshProUGUI inventorySizeGrowthText;
        [SerializeField] private TextMeshProUGUI healthGrowthText;

        public void Activate()
        {
            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Weapon))
            {
                equippedWeaponImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Weapon].CostumeSprites[0];
                playerWeaponImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Weapon].CostumeSprites[1];
                
                if (equippedWeaponImage != null) equippedWeaponImage.gameObject.SetActive(true);
            }
            else
            {
                if (equippedWeaponImage != null) equippedWeaponImage.gameObject.SetActive(false);
            }
            
            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Clothes))
            {
                equippedClothesImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Clothes].CostumeSprites[0];
                playerClothesImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Clothes].CostumeSprites[1];
                playerShoesImage[0].sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Clothes].CostumeSprites[2];
                playerShoesImage[1].sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Clothes].CostumeSprites[2];
                if (equippedClothesImage != null) equippedClothesImage.gameObject.SetActive(true);
            }
            else
            {
                if (equippedClothesImage != null) equippedClothesImage.gameObject.SetActive(false);
            }
            
            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Hat))
            {
                equippedHatImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Hat].CostumeSprites[0];
                playerHatImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Hat].CostumeSprites[1];
                if (equippedHatImage != null) equippedHatImage.gameObject.SetActive(true);
            }
            else
            {
                if (equippedHatImage != null) equippedHatImage.gameObject.SetActive(false);
            }

            if (VolatileDataManager.Instance.EquippedCostumes.ContainsKey(ECostumeType.Bag))
            {
                equippedBagImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Bag].CostumeSprites[0];
                playerBagImage.sprite = VolatileDataManager.Instance.EquippedCostumes[ECostumeType.Bag].CostumeSprites[1];
                if (equippedBagImage != null) equippedBagImage.gameObject.SetActive(true);
            }
            else
            {
                if (equippedBagImage != null) equippedBagImage.gameObject.SetActive(false);
            }

            damageGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.Damage}";
            inventorySizeGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.MaxProductInventorySize}";
            healthGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.MovementSpeed}";
            revenueGrowthText.text = $"{VolatileDataManager.Instance.Player.PlayerStatsModule.RevenueGrowth}";

            gameObject.SetActive(true);
        }
    }
}