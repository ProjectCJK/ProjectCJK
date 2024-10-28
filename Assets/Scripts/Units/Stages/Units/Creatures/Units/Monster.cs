using System;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Modules.FSMModules.Units;
using Units.Modules.HealthModules.Units;
using Units.Modules.MovementModules.Units;
using Units.Modules.StatsModules.Units;
using Units.Stages.Controllers;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Interfaces;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IMonster : IBaseCreature, IPoolable, IRegisterReference<MonsterDataSO>, IInitializable<Sprite, Action>, ITakeDamage
    {
        
    }

    public class Monster : Creature, IMonster
    {
        private event Action OnGetMonster;
        private event Action OnReturnMonster;
        
        public override ECreatureType CreatureType => _monsterStatsModule.Type;
        public override Animator Animator { get; protected set; }
        public override Transform Transform => transform;
        protected override CreatureStateMachine creatureStateMachine { get; set; }
        
        private IMonsterStatsModule _monsterStatsModule;
        private IMonsterHealthModule _monsterHealthModule;
        private IMonsterMovementModule _monsterMovementModule;
        
        private SpriteRenderer _spriteRenderer;
        private EMaterialType _materialType;

        public void RegisterReference(MonsterDataSO monsterDataSo)
        {
            _spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();

            creatureStateMachine = new CreatureStateMachine(this);
            _monsterStatsModule = new MonsterStatsModule(monsterDataSo);
            _monsterHealthModule = new MonsterHealthModule(_monsterStatsModule);
            _monsterMovementModule = new MonsterMovementModule(_monsterStatsModule);
        }
        
        public void Initialize(Sprite monsterSprite, Action action)
        {
            // TODO : 이후 애니메이션 클립이 완성되면 Sprite만 교체할 것
            // _spriteRenderer.sprite = monsterSprite;
            
            OnReturnMonster = action;
            _spriteRenderer.color = Color.red;
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
        }
    }
}