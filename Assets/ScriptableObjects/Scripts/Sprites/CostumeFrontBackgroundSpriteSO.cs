using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ScriptableObjects.Scripts.Sprites
{
    [CreateAssetMenu(fileName = "So_CostumeFrontBackgroundSprite", menuName = "Datas/Sprites/Costume/CostumeFrontBackgroundSprite")]
    public class CostumeFrontBackgroundSpriteSO : ScriptableObject
    {
        public List<CostumeFrontBackgroundSprite> CostumeFrontBackgroundSprites;
    }
    
    [Serializable]
    public struct CostumeFrontBackgroundSprite
    {
        public ECostumeType Type;
        public ECostumeGrade Grade;
        public Sprite Sprite;
    }
}