using System;
using Interfaces;
using Modules.DesignPatterns.ObjectPools;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules;
using Units.Stages.Modules.FSMModules.Units.Creature;
using Units.Stages.Modules.HealthModules.Units;
using Units.Stages.Modules.MovementModules.Units;
using Units.Stages.Modules.SpriteModules;
using Units.Stages.Modules.StatsModules.Units.Creatures.Units;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;
using Units.Stages.Units.Creatures.Interfaces;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public interface IMonster : ICreature, IPoolable, IRegisterReference<MonsterDataSO>,
        IInitializable<Vector3, MonsterSprite, Action>, ITakeDamage
    {
    }

    public class Monster : Creature, IMonster
    {
        private CreatureStateMachine _creatureStateMachine;
        private IDamageFlashModule _damageFlashModule;
        private EMaterialType _materialType;
        private IMonsterHealthModule _monsterHealthModule;
        private IMonsterMovementModule _monsterMovementModule;

        private MonsterSpriteModule _monsterSpriteModule;

        private IMonsterStatsModule _monsterStatsModule;
        private SpriteRenderer _spriteRenderer;
        public override Animator Animator { get; protected set; }

        private void Update()
        {
            _monsterMovementModule.Update();
        }

        private void FixedUpdate()
        {
            _monsterMovementModule.FixedUpdate();
        }

        public override ECreatureType CreatureType => _monsterStatsModule.CreatureType;
        public override Transform Transform => transform;

        public void RegisterReference(MonsterDataSO monsterDataSo)
        {
            Animator = spriteTransform.GetComponent<Animator>();
            _spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
            _damageFlashModule = spriteTransform.GetComponent<DamageFlashModule>();
            _monsterSpriteModule = spriteTransform.GetComponent<MonsterSpriteModule>();

            _creatureStateMachine = new CreatureStateMachine(this);
            _monsterStatsModule = new MonsterStatsModule(monsterDataSo);
            _monsterHealthModule = new MonsterHealthModule(_monsterStatsModule);
            _monsterMovementModule =
                new MonsterMovementModule(this, _monsterStatsModule, _creatureStateMachine, spriteTransform);

            _damageFlashModule.RegisterReference();
        }

        public void Initialize(Vector3 randomSpawnPoint, MonsterSprite monsterSprites, Action action)
        {
            // TODO : 이후 애니메이션 클립이 완성되면 Sprite만 교체할 것
            // _spriteRenderer.sprite = monsterSprite;
            _monsterSpriteModule.SetSprites(monsterSprites);

            transform.position = randomSpawnPoint;
            OnReturnMonster = action;

            _creatureStateMachine.ChangeState(_creatureStateMachine.CreatureIdleState);

            _monsterMovementModule.Initialize();
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

        public bool TakeDamage(int damage)
        {
            if (_monsterHealthModule.TakeDamage(damage))
            {
                _monsterMovementModule.hitTrigger = true;
                _damageFlashModule.ActivateEffects();
                return true;
            }

            OnReturnMonster?.Invoke();
            return false;
        }

        private event Action OnGetMonster;
        private event Action OnReturnMonster;

        private void SetActive(bool value)
        {
            if (gameObject.activeInHierarchy != value) gameObject.SetActive(value);
        }

        private void SetSprite(Sprite sprite)
        {
            // _spriteRenderer.sprite = sprite;
        }
    }
}