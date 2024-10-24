using System;
using System.Collections.Generic;
using Interfaces;
using Units.Modules.BattleModules.Abstract;
using Units.Stages.Units.Creatures.Interfaces;
using UnityEngine;

namespace Units.Modules.BattleModules
{

    public enum EBattleTag
    {
        Monster,
        Player,
        Structure
    }
    
    public interface IWeapon : IRegisterReference<IBattleProperty>
    {
        public void ActivateWeapon(bool value);
    }
    
    public class Weapon : MonoBehaviour, IWeapon
    {
        [SerializeField] private AttackTrigger _attackTrigger;
        [SerializeField] private LayerMask _targetLayerMask;
        [SerializeField] private List<EBattleTag> _targetTags;
        
        public void RegisterReference(IBattleProperty battleProperty)
        {
            _attackTrigger.RegisterReference(battleProperty, _targetLayerMask, _targetTags);
        }

        public void ActivateWeapon(bool value)
        {
            _attackTrigger.Initialize(value);
        }
    }
}