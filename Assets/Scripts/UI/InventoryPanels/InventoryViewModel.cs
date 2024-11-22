using System.ComponentModel;
using Modules.DesignPatterns.MVVMs;

namespace UI.InventoryPanels
{
    public class InventoryViewModel : BaseViewModel
    {
        public int TomatoCount => _inventoryModel.TomatoCount;
        public int CucumberCount => _inventoryModel.CucumberCount;
        public int BeanCount => _inventoryModel.BeanCount;
        public int TomatoCanCount => _inventoryModel.TomatoCanCount;
        public int CucumberCanCount => _inventoryModel.CucumberCanCount;
        public int BeanCanCount => _inventoryModel.BeanCanCount;
        public int TomatoStewCanCount => _inventoryModel.TomatoStewCanCount;

        private readonly InventoryModel _inventoryModel;

        public InventoryViewModel(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
            _inventoryModel.PropertyChanged += OnModelPropertyChanged;
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public void UpdateValues(
            int tomatoCount,
            int cucumberCount,
            int beanCount,
            int tomatoCanCount,
            int cucumberCanCount,
            int beanCanCount,
            int tomatoStewCanCount)
        {
            _inventoryModel.SetValues(
                tomatoCount,
                cucumberCount,
                beanCount,
                tomatoCanCount,
                cucumberCanCount,
                beanCanCount,
                tomatoStewCanCount);
        }
    }
}