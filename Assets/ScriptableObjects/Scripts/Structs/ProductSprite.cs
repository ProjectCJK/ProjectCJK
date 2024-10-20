using System;
using Units.Stages.Items.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Scripts.Structs
{
    [Serializable]
    public struct ProductSprite
    {
        [Header("--- 재료 타입 ---")]
        public EMaterialType MaterialType;
        [Header("--- 상품 타입 ---")]
        public EItemType itemType;
        [Header("--- 이미지 타입 ---")]
        public Sprite Sprite;
    }
}