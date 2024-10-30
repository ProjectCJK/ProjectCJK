using System;
using Interfaces;
using Modules;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.FSMModules.Units;
using Units.Modules.HealthModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Interfaces;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IMonster : IBaseCreature, IPoolable, IRegisterReference<MonsterDataSO>, IInitializable<Vector3, Sprite, Action>, ITakeDamage
    {
        
    }

    public class Monster : Creature, IMonster
    {
        private event Action OnGetMonster;
        private event Action OnReturnMonster;
        
        public override ECreatureType CreatureType => _monsterStatsModule.Type;
        public override Animator Animator => _animator;
        public override Transform Transform => transform;

        protected override CreatureStateMachine creatureStateMachine { get; set; }
        
        private IMonsterStatsModule _monsterStatsModule;
        private IMonsterHealthModule _monsterHealthModule;
        private IMonsterMovementModule _monsterMovementModule;
        private IDamageFlashModule _damageFlashModule;
        
        private SpriteRenderer _spriteRenderer;
        private EMaterialType _materialType;
        private Animator _animator;

        public void RegisterReference(MonsterDataSO monsterDataSo)
        {
            _spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            _damageFlashModule = spriteTransform.GetComponent<DamageFlashModule>();
            
            creatureStateMachine = new CreatureStateMachine(this);
            _monsterStatsModule = new MonsterStatsModule(monsterDataSo);
            _monsterHealthModule = new MonsterHealthModule(_monsterStatsModule);
            _monsterMovementModule = new MonsterMovementModule(_monsterStatsModule);
            
            _damageFlashModule.RegisterReference();
        }
        
        public void Initialize(Vector3 randomSpawnPoint, Sprite monsterSprite, Action action)
        {
            // TODO : 이후 애니메이션 클립이 완성되면 Sprite만 교체할 것
            // _spriteRenderer.sprite = monsterSprite;
            
            transform.position = randomSpawnPoint;
            OnReturnMonster = action;
        }

        public void Create()
        {
            SetActive(false);
        }

        public void GetFromPool()
        {
            _monsterHealthModule.Initialize();
            
            SetActive(true);
        }

        public void ReturnToPool()
        {
            SetActive(false);
            SetSprite(null);
        }
        
        private void SetActive(bool value)
        {
            if (gameObject.activeInHierarchy != value) gameObject.SetActive(value);
        }

        private void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;   
        }

        public void TakeDamage(int damage)
        {
            if (_monsterHealthModule.TakeDamage(damage))
            {
                OnReturnMonster?.Invoke();
            }
            else
            {
                _damageFlashModule.ActivateEffects();
            }
        }
    }
}