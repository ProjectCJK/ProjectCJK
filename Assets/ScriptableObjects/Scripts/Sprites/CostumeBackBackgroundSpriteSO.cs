using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace ScriptableObjects.Scripts.Sprites
{
    [CreateAssetMenu(fileName = "So_CostumeBackBackgroundSprite", menuName = "Datas/Sprites/Costume/CostumeBackBackgroundSprite")]
    public class CostumeBackBackgroundSpriteSO : ScriptableObject
    {
        public List<CostumeBackBackgroundSprite> CostumeBackBackgroundSprites;
    }
    
    [Serializable]
    public struct CostumeBackBackgroundSprite
    {
        public ECostumeGrade Grade;
        public Sprite Sprite;
    }
}