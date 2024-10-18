using System;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Buildings.UI.Kitchens
{
    public class KitchenModel : BaseModel
    {
        private int _remainedMaterialCount;
        private float _elapsedTime;
        private float _productLeadTime;

        // 재료 수량
        public int RemainedMaterialCount
        {
            get => _remainedMaterialCount;
            set
            {
                if (SetField(ref _remainedMaterialCount, value)) // 값이 변경되면 PropertyChanged 호출
                {
                    OnPropertyChanged(nameof(RemainedMaterialCount));
                }
            }
        }

        // 경과 시간
        public float ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                if (SetField(ref _elapsedTime, value))
                {
                    OnPropertyChanged(nameof(ElapsedTime));
                }
            }
        }

        // 생산 시간
        public float ProductLeadTime
        {
            get => _productLeadTime;
            set
            {
                if (SetField(ref _productLeadTime, value))
                {
                    OnPropertyChanged(nameof(ProductLeadTime));
                }
            }
        }

        // 주어진 값으로 모델 상태를 갱신
        public void SetValues(int remainedMaterialCount, float elapsedTime, float productLeadTime)
        {
            RemainedMaterialCount = remainedMaterialCount;
            ElapsedTime = elapsedTime;
            ProductLeadTime = productLeadTime;
        }
    }
}