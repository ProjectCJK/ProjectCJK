using Interfaces;
using Units.Modules.HealthModules.Abstract;
using UnityEngine;

namespace Units.Modules.HealthModules.Units
{
    public interface IMonsterHealthModule : IInitializable
    {
        public bool TakeDamage(int damage);
    }
    
    public class MonsterHealthModule : HealthModule, IMonsterHealthModule
    {
        private int _currentHealth;
        private int _maxHealth => _monsterStatModule.MaxHealth;
        
        private readonly IHealthProperty _monsterStatModule;
        
        public MonsterHealthModule(IHealthProperty monsterStatModule)
        {
            _monsterStatModule = monsterStatModule;
        }
        
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
                return true;
            }

            _currentHealth = tempHealth;
            return false;
        }
    }
}