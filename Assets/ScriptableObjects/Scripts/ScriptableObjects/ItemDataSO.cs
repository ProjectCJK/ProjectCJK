using System.Collections.Generic;
using ScriptableObjects.Scripts.Structs;
using Units.Stages.Items.Units;
using UnityEngine;

namespace ScriptableObjects.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Item Data SO", menuName = "Datas/Item Data")]
    public class ItemDataSO : ScriptableObject
    {
        [Header("상품 타입 별 이미지")] public List<ProductSprite> itemSprites;
        
        [Space(10)]
        [Header("상품 타입 별 이미지")] public Item itemPrefab;
        
        [Space(10)]
        [Header("기본 이동 속도")] public float BaseMovementSpeed;
    }
}