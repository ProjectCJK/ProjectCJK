using System;
using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Buildings.UI
{
    public class BlenderProductModel : BaseModel
    {
        private int _remainedMaterialCount;
        private float _elapsedTime;
        private float _productLeadTime;

        public event Action<int, float, float> OnValuesChanged;

        public int RemainedMaterialCount
        {
            get => _remainedMaterialCount;
            set
            {
                if (SetField(ref _remainedMaterialCount, value))
                {
                    TriggerValuesChanged();
                }
            }
        }

        public float ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                if (SetField(ref _elapsedTime, value))
                {
                    TriggerValuesChanged();
                }
            }
        }

        public float ProductLeadTime
        {
            get => _productLeadTime;
            set
            {
                if (SetField(ref _productLeadTime, value))
                {
                    TriggerValuesChanged();
                }
            }
        }

        private void TriggerValuesChanged()
        {
            OnValuesChanged?.Invoke(_remainedMaterialCount, _elapsedTime, _productLeadTime);
        }

        public void SetValues(int remainedMaterialCount, float elapsedTime, float productLeadTime)
        {
            RemainedMaterialCount = remainedMaterialCount;
            ElapsedTime = elapsedTime;
            ProductLeadTime = productLeadTime;
        }
    }
}