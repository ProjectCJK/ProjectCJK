using System.Collections.Generic;
using Interfaces;
using Units.Stages.Modules.BattleModules.Abstract;
using UnityEngine;

namespace Units.Stages.Modules.BattleModules
{

    public enum EBattleTag
    {
        Monster,
        Player,
        Structure
    }
    
    public interface IWeapon : IRegisterReference<IBattleProperty>
    {
        public AttackTrigger AttackTrigger { get; }
        public void ActivateWeapon(bool value);
    }
    
    public class Weapon : MonoBehaviour, IWeapon
    {
        public AttackTrigger AttackTrigger => _attackTrigger;
        
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