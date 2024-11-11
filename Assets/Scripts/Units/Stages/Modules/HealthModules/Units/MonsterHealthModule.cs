using Interfaces;
using Units.Stages.Modules.HealthModules.Abstract;
using UnityEngine;

namespace Units.Stages.Modules.HealthModules.Units
{
    public interface IMonsterHealthModule : IInitializable
    {
        public bool TakeDamage(int damage);
    }

    public class MonsterHealthModule : HealthModule, IMonsterHealthModule
    {
        private readonly IHealthProperty _monsterStatModule;
        private int _currentHealth;

        public MonsterHealthModule(IHealthProperty monsterStatModule)
        {
            _monsterStatModule = monsterStatModule;
        }

        private int _maxHealth => _monsterStatModule.MaxHealth;

        public void Initialize()
        {
            _currentHealth = _maxHealth;
        }

        public bool TakeDamage(int damage)
        {
            var tempHealth = _currentHealth - damage;

            Debug.Log($"아야! {damage}, {tempHealth}");

            if (tempHealth < 0)
            {
                _currentHealth = 0;
                return false;
            }

            _currentHealth = tempHealth;
            return true;
        }
    }
}