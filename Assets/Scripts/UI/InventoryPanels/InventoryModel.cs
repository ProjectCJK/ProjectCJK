using Modules.DesignPatterns.MVVMs;

namespace UI.InventoryPanels
{
    public class InventoryModel : BaseModel
    {
        private int _tomatoCount;
        private int _cucumberCount;
        private int _beanCount;
        private int _tomatoCanCount;
        private int _cucumberCanCount;
        private int _beanCanCount;
        private int _tomatoStewCanCount;

        public int TomatoCount
        {
            get => _tomatoCount;
            private set
            {
                if (SetField(ref _tomatoCount, value)) OnPropertyChanged();
            }
        }

        public int CucumberCount
        {
            get => _cucumberCount;
            private set
            {
                if (SetField(ref _cucumberCount, value)) OnPropertyChanged();
            }
        }

        public int BeanCount
        {
            get => _beanCount;
            private set
            {
                if (SetField(ref _beanCount, value)) OnPropertyChanged();
            }
        }

        public int TomatoCanCount
        {
            get => _tomatoCanCount;
            private set
            {
                if (SetField(ref _tomatoCanCount, value)) OnPropertyChanged();
            }
        }

        public int CucumberCanCount
        {
            get => _cucumberCanCount;
            private set
            {
                if (SetField(ref _cucumberCanCount, value)) OnPropertyChanged();
            }
        }

        public int BeanCanCount
        {
            get => _beanCanCount;
            private set
            {
                if (SetField(ref _beanCanCount, value)) OnPropertyChanged();
            }
        }

        public int TomatoStewCanCount
        {
            get => _tomatoStewCanCount;
            private set
            {
                if (SetField(ref _tomatoStewCanCount, value)) OnPropertyChanged();
            }
        }

        public void SetValues(
            int tomatoCount,
            int cucumberCount,
            int beanCount,
            int tomatoCanCount,
            int cucumberCanCount,
            int beanCanCount,
            int tomatoStewCanCount)
        {
            TomatoCount = tomatoCount;
            CucumberCount = cucumberCount;
            BeanCount = beanCount;
            TomatoCanCount = tomatoCanCount;
            CucumberCanCount = cucumberCanCount;
            BeanCanCount = beanCanCount;
            TomatoStewCanCount = tomatoStewCanCount;
        }
    }
}