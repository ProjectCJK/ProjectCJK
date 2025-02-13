using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Units.Buildings.UI.Kitchens
{
    public class KitchenViewModel : BaseViewModel
    {
        private readonly KitchenModel _kitchenModel;

        // 생성자에서 모델을 받아와 구독
        public KitchenViewModel(KitchenModel kitchenModel)
        {
            _kitchenModel = kitchenModel;
            _kitchenModel.PropertyChanged += OnModelPropertyChanged;
        }

        public int RemainedMaterialCount => _kitchenModel.RemainedMaterialCount;
        public float ElapsedTime => _kitchenModel.ElapsedTime;
        public float ProductLeadTime => _kitchenModel.ProductLeadTime;

        // 모델의 값이 변경될 때 UI에 알림
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName); // 변경된 프로퍼티를 UI에 알림
        }

        // 모델에 새로운 값을 설정하는 메서드
        public void UpdateValues(int remainedMaterialCount, float elapsedTime, float productLeadTime)
        {
            _kitchenModel.SetValues(remainedMaterialCount, elapsedTime, productLeadTime);
        }
    }
}