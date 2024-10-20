using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Buildings.UI
{
    public class BlenderProductViewModel : BaseViewModel
    {
        private readonly BlenderProductModel _blenderProductModel;

        public BlenderProductViewModel(BlenderProductModel blenderProductModel)
        {
            _blenderProductModel = blenderProductModel;
            _blenderProductModel.OnValuesChanged += OnModelValuesChanged;
        }

        public int RemainedMaterialCount => _blenderProductModel.RemainedMaterialCount;
        public float ElapsedTime => _blenderProductModel.ElapsedTime;
        public float ProductLeadTime => _blenderProductModel.ProductLeadTime;

        private void OnModelValuesChanged(int remainedMaterialCount, float elapsedTime, float productLeadTime)
        {
            OnPropertyChanged(nameof(RemainedMaterialCount));
            OnPropertyChanged(nameof(ElapsedTime));
            OnPropertyChanged(nameof(ProductLeadTime));
        }

        public void UpdateValues(int remainedMaterialCount, float elapsedTime, float productLeadTime)
        {
            _blenderProductModel.SetValues(remainedMaterialCount, elapsedTime, productLeadTime);
        }
    }
}