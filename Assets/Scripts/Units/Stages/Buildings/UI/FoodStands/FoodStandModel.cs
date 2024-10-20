using System;
using Modules.DesignPatterns.MVVMs;
using Units.Modules.InventoryModules.Units;
using Units.Stages.Items.Enums;

namespace Units.Stages.Buildings.UI.FoodStands
{
    public class FoodStandModel : BaseModel
    {
        private int _remainedProductCount;

        // 모델이 직접 값을 관리
        public int RemainedProductCount
        {
            get => _remainedProductCount;
            private set
            {
                if (SetField(ref _remainedProductCount, value)) // 값이 변경되면 PropertyChanged 호출
                {
                    OnPropertyChanged(nameof(RemainedProductCount));
                }
            }
        }
        
        public void SetValues(int remainedProductCount)
        {
            RemainedProductCount = remainedProductCount;
        }
    }
}