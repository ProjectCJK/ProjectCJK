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
        
        private static readonly int Disable = Animator.StringToHash("Disable");
        private bool isWeaponActivated;
        
        public void RegisterReference(IBattleProperty battleProperty)
        {
            _attackTrigger.RegisterReference(battleProperty, _targetLayerMask, _targetTags, HandleOnInvokeAnimationEvent);
        }

        public void ActivateWeapon(bool value)
        {
            if (isWeaponActivated != value)
            {
                isWeaponActivated = value;

                if (value)
                {
                    transform.gameObject.SetActive(true);
                    _attackTrigger.Initialize(true);
                }
                else
                {
                    _attackTrigger.Animator.SetTrigger(Disable);
                }
            }
        }

        private void HandleOnInvokeAnimationEvent()
        {
            transform.gameObject.SetActive(false);
            _attackTrigger.Initialize(false);
        }
    }
}