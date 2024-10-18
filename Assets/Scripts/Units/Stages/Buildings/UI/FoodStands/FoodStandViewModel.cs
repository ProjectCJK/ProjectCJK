using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Buildings.UI.FoodStands
{
    public class FoodStandViewModel : BaseViewModel
    {
        private readonly FoodStandModel _foodStandModel;

        public FoodStandViewModel(FoodStandModel foodStandModel)
        {
            _foodStandModel = foodStandModel;
            _foodStandModel.PropertyChanged += OnModelValuesChanged;
        }

        public int RemainedProductCount => _foodStandModel.RemainedProductCount;

        private void OnModelValuesChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        
        public void UpdateValues(int remainedMaterialCount)
        {
            _foodStandModel.SetValues(remainedMaterialCount);
        }
    }
}