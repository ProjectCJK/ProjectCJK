using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.Modules.UnlockModules.UI
{
    public class UnlockZoneModel : BaseModel
    {
        private int _goldCount;
        private int _maxGoldCount;

        public int GoldCount
        {
            get => _goldCount;

            private set
            {
                if (SetField(ref _goldCount, value)) OnPropertyChanged();
            }
        }

        public int MaxGoldCount
        {
            get => _maxGoldCount;

            private set
            {
                if (SetField(ref _maxGoldCount, value)) OnPropertyChanged();
            }
        }

        public void SetValues(int gold, int maxGold)
        {
            GoldCount = gold;
            MaxGoldCount = maxGold;
        }
    }
}