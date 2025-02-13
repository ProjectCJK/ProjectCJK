using Managers;
using ScriptableObjects.Scripts.Creatures.Units;
using Units.Stages.Modules.BattleModules.Abstract;
using Units.Stages.Modules.CollisionModules.Abstract;
using Units.Stages.Modules.CostumeModules;
using Units.Stages.Modules.HealthModules.Abstract;
using Units.Stages.Modules.InventoryModules.Abstract;
using Units.Stages.Modules.MovementModules.Abstract;
using Units.Stages.Modules.PaymentModule.Abstract;
using Units.Stages.Modules.StatsModules.Abstract;
using Units.Stages.Units.Creatures.Abstract;
using Units.Stages.Units.Creatures.Enums;

namespace Units.Stages.Modules.StatsModules.Units.Creatures.Units
{
    public interface IPlayerStatsModule : ICreatureTypeProperty, IMovementProperty, IInventoryProperty,
        IInteractionProperty, IBattleProperty, IHealthProperty, IPaymentProperty
    {
    }

    public class PlayerStatsModule : StatsModule, IPlayerStatsModule
    {
        private readonly PlayerDataSO _playerDataSo;

        public ECreatureType CreatureType => ECreatureType.Player;

        public float MovementSpeed
        {
            get
            {
                if (VolatileDataManager.Instance.CostumeEquipmentOption.ContainsKey(ECostumeOptionType.Player_Speed))
                {
                    return movementSpeed + VolatileDataManager.Instance.CostumeEquipmentOption[ECostumeOptionType.Player_Speed];
                }

                return movementSpeed;
            }
            set => movementSpeed += value;
        }

        public float WaitingTime => _playerDataSo.BaseInteractionStandbySecond;
        public float PaymentDelay => _playerDataSo.BasePaymentDelay;

        public int MaxProductInventorySize
        {
            get
            {
                if (VolatileDataManager.Instance.CostumeEquipmentOption.ContainsKey(ECostumeOptionType.Player_Bag_Size))
                {
                    return _playerDataSo.BaseInventorySize + (int) VolatileDataManager.Instance.CostumeEquipmentOption[ECostumeOptionType.Player_Bag_Size];
                }

                return _playerDataSo.BaseInventorySize;
            }
        }

        public int MaxHealth => _playerDataSo.BaseHealth;

        public int Damage
        {
            get
            {
                if (VolatileDataManager.Instance.CostumeEquipmentOption.ContainsKey(ECostumeOptionType.Player_Power))
                {
                    return _playerDataSo.BaseDamage + (int) VolatileDataManager.Instance.CostumeEquipmentOption[ECostumeOptionType.Player_Power];
                }

                return _playerDataSo.BaseDamage;
            }
        }
        
        public float AttackDelay => _playerDataSo.BaseAttackDelay;

        public float RevenueGrowth
        {
            get
            {
                if (VolatileDataManager.Instance.CostumeEquipmentOption.ContainsKey(ECostumeOptionType.All_Product_Income))
                {
                    return _playerDataSo.BaseRevenueGrowth + (int) VolatileDataManager.Instance.CostumeEquipmentOption[ECostumeOptionType.All_Product_Income];
                }

                return _playerDataSo.BaseRevenueGrowth;
            }
        }

        public PlayerStatsModule(PlayerDataSO playerDataSo)
        {
            _playerDataSo = playerDataSo;
            MovementSpeed = _playerDataSo.BaseMovementSpeed;

            // TODO : 스탯을 런타임 중에 바꾸는 기능이 추가되면 (카드 시스템, 광고 보상으로 인한 일시적 스탯 강화 등) 값을 복사하고 이 값을 수정하는 식으로 해야 함.
            // MovementSpeed = playerStatSo.BaseMovementSpeed;
            // InventorySize = playerStatSo.BaseInventorySize;
        }
    }
}