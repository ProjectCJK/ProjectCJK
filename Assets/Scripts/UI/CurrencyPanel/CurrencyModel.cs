using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.UI
{
    public class CurrencyModel : BaseModel
    {
        private int _diamondCount;
        private int _redGemCount;
        private int _goldCount;
        
        public int DiamondCount
        {
            get => _diamondCount;

            private set
            {
                if (SetField(ref _diamondCount, value)) OnPropertyChanged();
            }
        }
        
        public int RedGemCount
        {
            get => _redGemCount;

            private set
            {
                if (SetField(ref _redGemCount, value)) OnPropertyChanged();
            }
        }

        public int GoldCount
        {
            get => _goldCount;

            private set
            {
                if (SetField(ref _goldCount, value)) OnPropertyChanged();
            }
        }

        public void SetValues(int diamondCount, int redGemCount, int goldCount)
        {
            DiamondCount = diamondCount;
            RedGemCount = redGemCount;
            GoldCount = goldCount;
        }
    }
}