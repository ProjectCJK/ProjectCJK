using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Units.Buildings.UI.Kitchens
{
    public class KitchenModel : BaseModel
    {
        private float _elapsedTime;
        private float _productLeadTime;
        private int _remainedMaterialCount;

        // 재료 수량
        public int RemainedMaterialCount
        {
            get => _remainedMaterialCount;

            private set
            {
                if (SetField(ref _remainedMaterialCount, value)) // 값이 변경되면 PropertyChanged 호출
                    OnPropertyChanged();
            }
        }

        // 경과 시간
        public float ElapsedTime
        {
            get => _elapsedTime;

            private set
            {
                if (SetField(ref _elapsedTime, value)) OnPropertyChanged();
            }
        }

        // 생산 시간
        public float ProductLeadTime
        {
            get => _productLeadTime;

            private set
            {
                if (SetField(ref _productLeadTime, value)) OnPropertyChanged();
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