using System;
using System.Collections.Generic;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts.Items
{
    [Serializable]
    public struct ItemSprite
    {
        [Header("상품 타입")] 
        public EItemType ItemType;
        [Header("재료 타입")] 
        public EStageMaterialType StageMaterialType;
        [Header("아이템 이미지")] 
        public Sprite Sprite;
    }
    
    [Serializable]
    public struct CurrencySprite
    {
        [Header("상품 타입")] 
        public ECurrencyType CurrencyType;
        [Header("아이템 이미지")] 
        public Sprite Sprite;
    }

    [CreateAssetMenu(fileName = "New Item Data SO", menuName = "Datas/Item Data")]
    public class ItemDataSO : ScriptableObject
    {
        [Header("### Item 기본 세팅 ###")]
        [Header("prefab")] public GameObject prefab;
        [Header("아이템 Sprite")] public List<ItemSprite> ItemSprites;
        [Header("재화 Sprite")] public List<CurrencySprite> CurrencySprites;
        [Header("기본 이동 속도")] public float BaseMovementSpeed;
    }
}