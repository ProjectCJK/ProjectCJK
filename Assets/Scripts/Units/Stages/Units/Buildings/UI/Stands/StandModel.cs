using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Units.Buildings.UI.Stands
{
    public class StandModel : BaseModel
    {
        private int _remainedProductCount;

        // 모델이 직접 값을 관리
        public int RemainedProductCount
        {
            get => _remainedProductCount;
            private set
            {
                if (SetField(ref _remainedProductCount, value)) // 값이 변경되면 PropertyChanged 호출
                    OnPropertyChanged();
            }
        }

        public void SetValues(int remainedProductCount)
        {
            RemainedProductCount = remainedProductCount;
        }
    }
}