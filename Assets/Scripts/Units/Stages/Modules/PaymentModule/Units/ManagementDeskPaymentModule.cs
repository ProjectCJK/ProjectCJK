using System;
using System.Collections.Generic;
using Managers;
using Units.Stages.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Stages.Modules.PaymentModule.Abstract;
using Units.Stages.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Units;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Modules.PaymentModule.Units
{
    public interface IManagementDeskPaymentModule
    {
        void Update();
        bool RegisterPaymentTarget(ICreature creature, bool register);
    }

    public class CashierPayment
    {
        public float Delay;
        public Guest Guest;
    }

    public class ManagementDeskPaymentModule : BuildingPaymentModule, IManagementDeskPaymentModule
    {
        private readonly Queue<Guest> _customerQueue = new(); // NPC 손님 대기열
        private readonly List<CashierPayment> _cashierPayments = new(); // 계산원 처리 리스트

        private readonly string _inputKey; // 관리 데스크 키
        private readonly IManagementDeskInventoryModule _inventoryModule; // 인벤토리 모듈
        private readonly float _npcPaymentDelay; // NPC 결제 딜레이

        private Player _player; // 플레이어 참조
        private float _playerPaymentElapsedTime; // 플레이어 결제 대기 시간
        private float _playerPaymentDelay; // 플레이어 결제 딜레이

        public int CurrentSpawnedCashierCount { get; set; }

        public ManagementDeskPaymentModule(ManagementDeskStatsModule statsModule, 
                                           IManagementDeskInventoryModule inventoryModule, 
                                           string inputKey)
        {
            _npcPaymentDelay = statsModule.CurrentBuildingOption1Value;
            _inventoryModule = inventoryModule;
            _inputKey = inputKey;
        }

        public void Update()
        {
            ProcessPlayerPayment();
            ProcessCashierPayments();
        }

        public bool RegisterPaymentTarget(ICreature creature, bool register)
        {
            // 플레이어 또는 NPC 등록 처리
            switch (creature.CreatureType)
            {
                case ECreatureType.Player:
                    return HandlePlayerRegistration(creature as Player, register);
                case ECreatureType.NPC:
                    return HandleNpcRegistration(creature as Guest, register);
                default:
                    return false; // 다른 타입은 무시
            }
        }

        private bool HandlePlayerRegistration(Player player, bool register)
        {
            if (register)
            {
                _player = player;
                _playerPaymentDelay = player?.PaymentDelay ?? 0f;
            }
            else
            {
                _player = null;
                _playerPaymentElapsedTime = 0f;
            }
            return true;
        }

        private bool HandleNpcRegistration(Guest guest, bool register)
        {
            switch (register)
            {
                case true when guest != null && !_customerQueue.Contains(guest):
                    _customerQueue.Enqueue(guest);
                    break;
                case false:
                    RemoveFromQueue(guest);
                    break;
            }

            return true;
        }

        private void ProcessPlayerPayment()
        {
            if (_player == null || _customerQueue.Count == 0) return;

            _playerPaymentElapsedTime += Time.deltaTime;

            if (_playerPaymentElapsedTime >= _playerPaymentDelay)
            {
                Guest guest = _customerQueue.Dequeue();
                ProcessPaymentForGuest(guest);

                _playerPaymentElapsedTime = 0f; // 결제 딜레이 초기화
            }
        }

        private void ProcessCashierPayments()
        {
            foreach (CashierPayment cashier in _cashierPayments)
            {
                if (cashier.Guest != null)
                {
                    cashier.Delay += Time.deltaTime;

                    if (cashier.Delay >= _npcPaymentDelay)
                    {
                        ProcessPaymentForGuest(cashier.Guest);
                        cashier.Guest = null;
                        cashier.Delay = 0f; // 계산원 결제 딜레이 초기화
                    }
                }
                else if (_customerQueue.TryDequeue(out var guest))
                {
                    cashier.Guest = guest;
                    cashier.Delay = 0f; // 새로운 손님 처리 시작
                }
            }
        }

        private void ProcessPaymentForGuest(Guest guest)
        {
            if (guest == null) return;

            Tuple<string, int> purchasedItem = guest.GetItem();
            if (purchasedItem == null) return;

            (EItemType? itemType, EMaterialType? materialType) = ParserModule.ParseStringToEnum<EItemType, EMaterialType>(purchasedItem.Item1);
            var totalItemPrice = VolatileDataManager.Instance.GetItemPrice(itemType, materialType) * purchasedItem.Item2;

            // 퀘스트 진행 상황 업데이트
            QuestManager.Instance.OnUpdateCurrentQuestProgress?.Invoke(EQuestType1.Selling, purchasedItem.Item1, purchasedItem.Item2);

            // 아이템 가격 처리
            while (totalItemPrice > 0)
            {
                var goldAmount = Mathf.Min(totalItemPrice, DataManager.GoldSendingMaximum);
                _inventoryModule.ReceiveItemNoThroughTransfer(_inputKey, goldAmount);
                totalItemPrice -= goldAmount;
            }

            guest.CheckNextDestination(); // 손님 다음 행동 처리
        }

        private void RemoveFromQueue(Guest guest)
        {
            if (guest == null) return;

            var tempQueue = new Queue<Guest>();

            while (_customerQueue.Count > 0)
            {
                Guest dequeuedGuest = _customerQueue.Dequeue();
                
                if (dequeuedGuest == guest) continue; // 제거할 손님은 대기열에서 제외
                tempQueue.Enqueue(dequeuedGuest);
            }

            while (tempQueue.Count > 0)
            {
                _customerQueue.Enqueue(tempQueue.Dequeue());
            }
        }

        public void AddCashierPaymentSlot(int slotCount)
        {
            for (var i = 0; i < slotCount; i++)
            {
                _cashierPayments.Add(new CashierPayment());
            }
        }

        public void RemoveCashierPaymentSlot(int slotCount)
        {
            for (var i = 0; i < slotCount && _cashierPayments.Count > 0; i++)
            {
                _cashierPayments.RemoveAt(_cashierPayments.Count - 1);
            }
        }
    }
}