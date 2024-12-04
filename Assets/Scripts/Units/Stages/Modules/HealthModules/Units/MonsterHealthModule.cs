using Interfaces;
using Managers;
using Units.Stages.Modules.HealthModules.Abstract;
using Units.Stages.Units.Items.Enums;
using UnityEngine;

namespace Units.Stages.Modules.HealthModules.Units
{
    public interface IMonsterHealthModule : IInitializable<EMaterialType>
    {
        public bool TakeDamage(int damage);
    }

    public class MonsterHealthModule : HealthModule, IMonsterHealthModule
    {
        private readonly IHealthProperty _monsterStatModule;
        
        private int _currentHealth;
        private int _maxHealth;

        public MonsterHealthModule(IHealthProperty monsterStatModule)
        {
            _monsterStatModule = monsterStatModule;
        }

        public void Initialize(EMaterialType materialType)
        {
            _maxHealth = _monsterStatModule.MaxHealth;
            switch (GameManager.Instance.ES3Saver.CurrentStageLevel)
            {
                case 1:
                    switch (materialType)
                    {
                        case EMaterialType.B:
                            _maxHealth = 190;
                            break;
                    }

                    break;
                case 2:
                    switch (materialType)
                    {
                        case EMaterialType.B:
                            _maxHealth = 190;
                            break;
                        case EMaterialType.C:
                            _maxHealth = 280;
                            break;
                    }

                    break;
            }

            _currentHealth = _maxHealth;
        }

        public bool TakeDamage(int damage)
        {
            var tempHealth = _currentHealth - damage;

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