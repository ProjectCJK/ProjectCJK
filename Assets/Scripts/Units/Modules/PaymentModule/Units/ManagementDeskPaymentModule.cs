using System;
using System.Collections.Generic;
using Modules.DataStructures;
using Units.Modules.InventoryModules.Interfaces;
using Units.Modules.InventoryModules.Units.BuildingInventoryModules.Units;
using Units.Modules.PaymentModule.Abstract;
using Units.Modules.StatsModules.Units.Buildings.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Units;
using UnityEngine;

namespace Units.Modules.PaymentModule.Units
{
    public interface IManagementDeskPaymentModule
    {
        public void Update();
        public bool RegisterPaymentTarget(ICreature creature, bool register);
    }
    
    public class ManagementDeskPaymentModule : BuildingPaymentModule, IManagementDeskPaymentModule
    {
        // private readonly List<> // TODO : 이후 계산원 유닛 추가되면 이 부분에서 처리할 것
        private readonly IManagementDeskInventoryModule _managementDeskInventoryModule;
        private readonly Queue<Guest> _customerQueue = new();
        private readonly string _inputKey;
        
        private Player _player;
        private float _playerPaymentDelay;
        private float _npcPaymentDelay;
        
        private float _playerPaymentElapsedTime;
        private float _npcPaymentElapsedTime;

        public ManagementDeskPaymentModule(IManagementDeskStatsModule managementDeskStatsModule, IManagementDeskInventoryModule managementDeskInventoryModule, string inputKey)
        {
            _npcPaymentDelay = managementDeskStatsModule.BaseProductLeadTime;
            _managementDeskInventoryModule = managementDeskInventoryModule;
            _inputKey = inputKey;
        }
        
        public void Update()
        {
            ProcessPlayerPayment();
        }

        private void ProcessPlayerPayment()
        {
            if (_player != null && _customerQueue.Count > 0)
            {
                _playerPaymentElapsedTime += Time.deltaTime;

                if (_playerPaymentElapsedTime >= _playerPaymentDelay)
                {
                    Guest guest = _customerQueue.Dequeue();
                    
                    _managementDeskInventoryModule.ReceiveItemNoThroughTransfer(_inputKey);
                    guest.CheckNextDestination();

                    _playerPaymentElapsedTime = 0;
                }
            }
        }

        public bool RegisterPaymentTarget(ICreature creature, bool register)
        {
            if (register)
            {
                switch (creature.CreatureType)
                {
                    case ECreatureType.Player:
                        _player = creature as Player;
                        if (_player != null) _playerPaymentDelay = _player.PaymentDelay;
                        return true;
                    case ECreatureType.NPC when creature as Creature:
                        _customerQueue.Enqueue(creature as Guest);
                        return true;
                    case ECreatureType.Monster:
                    default:
                        return false;
                }
            }
            else
            {
                switch (creature.CreatureType)
                {
                    case ECreatureType.Player:
                        _player = null;
                        return true;
                    case ECreatureType.NPC when creature is Creature:
                        FindAndDequeue(creature as Guest);
                        return true;
                    case ECreatureType.Monster:
                    default:
                        return false;
                }   
            }
        }

        private void FindAndDequeue(Guest creature)
        {
            var tempQueue = new Queue<Guest>();

            while (_customerQueue.Count > 0)
            {
                Guest currentGuest = _customerQueue.Dequeue();

                if (currentGuest == creature)
                {
                    break;
                }

                tempQueue.Enqueue(currentGuest);
            }
            
            foreach (Guest guest in tempQueue)
            {
                _customerQueue.Enqueue(guest);
            }
        }
    }
}