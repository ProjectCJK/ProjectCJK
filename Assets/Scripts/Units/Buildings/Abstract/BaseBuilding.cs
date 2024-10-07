using System;
using System.Collections.Generic;
using Enums;
using Units.Creatures.Abstract;
using Units.Creatures.Units.Players;
using UnityEngine;

namespace Units.Buildings.Abstract
{
    public abstract class BaseBuilding : MonoBehaviour
    {
        [SerializeField] private EMaterialType _materialType;
        [SerializeField] private EItemType _itemType;

        // 빌딩이 다루는 아이템의 키
        public Tuple<EMaterialType, EItemType> InventoryKey => Tuple.Create(_materialType, _itemType);

        // 빌딩 내 아이템 저장소
        private readonly Dictionary<Tuple<EMaterialType, EItemType>, int> _itemStorage = new();

        // 아이템을 빌딩에 추가
        public void ReceiveItem()
        {
            if (!_itemStorage.TryAdd(InventoryKey, 1))
            {
                _itemStorage[InventoryKey]++;  // 이미 존재하면 수량 추가
            }
        }

        // 빌딩에서 Creature에게 아이템을 전송
        public void TransferItemToCreature(BaseCreature creature)
        {
            if (_itemStorage.ContainsKey(InventoryKey) && _itemStorage[InventoryKey] > 0)
            {
                var player = creature as Player;
                if (player != null)
                {
                    player.ReceiveItem(InventoryKey);  // 플레이어의 ReceiveItem 호출
                    _itemStorage[InventoryKey]--;  // 아이템 수량 감소

                    if (_itemStorage[InventoryKey] == 0)
                    {
                        _itemStorage.Remove(InventoryKey);  // 0개가 되면 삭제
                    }
                }
            }
        }
    }
}