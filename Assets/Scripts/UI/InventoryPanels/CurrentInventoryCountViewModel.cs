using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace UI.InventoryPanels
{
    public class CurrentInventoryCountViewModel : BaseViewModel
    {
        public int CurrentInventorySize => _currentInventoryCountModel.CurrentInventorySize;
        public int MaxInventorySize => _currentInventoryCountModel.MaxInventorySize;

        private readonly CurrentInventoryCountModel _currentInventoryCountModel;
        
        public CurrentInventoryCountViewModel(CurrentInventoryCountModel currentInventoryCountModel)
        {
            _currentInventoryCountModel = currentInventoryCountModel;
            _currentInventoryCountModel.PropertyChanged += OnModelPropertyChanged;
        }
        
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        
        public void UpdateValues(int currentInventorySize, int maxInventorySize)
        {
            _currentInventoryCountModel.SetValues(currentInventorySize, maxInventorySize);
        }
    }
}