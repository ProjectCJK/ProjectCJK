using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Units.Buildings.UI.Stands
{
    public class StandViewModel : BaseViewModel
    {
        private readonly StandModel _standModel;

        public StandViewModel(StandModel standModel)
        {
            _standModel = standModel;
            _standModel.PropertyChanged += OnModelValuesChanged;
        }

        public int RemainedProductCount => _standModel.RemainedProductCount;

        private void OnModelValuesChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public void UpdateValues(int remainedMaterialCount)
        {
            _standModel.SetValues(remainedMaterialCount);
        }
    }
}