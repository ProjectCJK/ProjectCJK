using Modules.DesignPatterns.MVVMs;

namespace UI.InventoryPanels
{
    public class CurrentInventoryCountModel : BaseModel
    {
        private int _currentInventorySize;
        private int _maxInventorySize;
        
        public int CurrentInventorySize
        {
            get => _currentInventorySize;

            private set
            {
                if (SetField(ref _currentInventorySize, value)) OnPropertyChanged();
            }
        }
        
        public int MaxInventorySize
        {
            get => _maxInventorySize;

            private set
            {
                if (SetField(ref _maxInventorySize, value)) OnPropertyChanged();
            }
        }

        public void SetValues(int currentInventorySize, int maxInventorySize)
        {
            CurrentInventorySize = currentInventorySize;
            MaxInventorySize = maxInventorySize;
        }
    }
}