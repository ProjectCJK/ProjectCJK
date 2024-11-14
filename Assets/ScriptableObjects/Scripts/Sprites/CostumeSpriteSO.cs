using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ScriptableObjects.Scripts.Sprites
{
    [CreateAssetMenu(fileName = "So_CostumeSprite", menuName = "Datas/Sprites/Costume/CostumeSprite")]
    public class CostumeSpriteSO : ScriptableObject
    {
        public List<CostumeSprite> WeaponCostumeSprites;
        public List<CostumeSprite> BodyCostumeSprites;
        public List<CostumeSprite> HatCostumeSprites;
        public List<CostumeSprite> BagCostumeSprites;
    }
    
    [Serializable]
    public struct CostumeSprite
    {
        public ECostumeGrade Grade;
        public List<Sprite> Sprites;
    }
}