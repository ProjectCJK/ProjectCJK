using System;
using System.Collections.Generic;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace ScriptableObjects.Scripts.Items
{
    [Serializable]
    public struct ItemSprite
    {
        [Header("재료 타입")] 
        public EMaterialType MaterialType;
        [Header("상품 타입")] 
        public EItemType ItemType;
        [Header("아이템 이미지")] 
        public Sprite Sprite;
    }

    [CreateAssetMenu(fileName = "New Item Data SO", menuName = "Datas/Item Data")]
    public class ItemDataSO : ScriptableObject
    {
        [Header("### Item 기본 세팅 ###")]
        [Header("prefab")] public GameObject prefab;
        [Header("타입별 Sprite")] public List<ItemSprite> ItemSprites;
        [Header("기본 이동 속도")] public float BaseMovementSpeed;
    }
}