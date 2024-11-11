using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Modules.UnlockModules.UI
{
    public class UnlockZoneViewModel : BaseViewModel
    {
        private readonly UnlockZoneModel _unlockZoneModel;

        // 생성자에서 모델을 받아와 구독
        public UnlockZoneViewModel(UnlockZoneModel unlockZoneModel)
        {
            _unlockZoneModel = unlockZoneModel;
            _unlockZoneModel.PropertyChanged += OnModelPropertyChanged;
        }

        public int Gold => _unlockZoneModel.GoldCount;
        public int MaxGold => _unlockZoneModel.MaxGoldCount;

        // 모델의 값이 변경될 때 UI에 알림
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName); // 변경된 프로퍼티를 UI에 알림
        }

        // 모델에 새로운 값을 설정하는 메서드
        public void UpdateValues(int gold, int maxGold)
        {
            _unlockZoneModel.SetValues(gold, maxGold);
        }
    }
}