using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.UI
{
    public class CurrencyModel : BaseModel
    {
        private int _goldCount;

        public int GoldCount
        {
            get => _goldCount;

            private set
            {
                if (SetField(ref _goldCount, value)) OnPropertyChanged();
            }
        }

        public void SetValues(int gold)
        {
            GoldCount = gold;
        }
    }
}