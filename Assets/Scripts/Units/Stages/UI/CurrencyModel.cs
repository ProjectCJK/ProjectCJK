using Modules.DesignPatterns.MVVMs;

namespace Units.Stages.UI
{
    public class CurrencyModel : BaseModel
    {
        private int _gold;
        public int _dia;

        public int Gold
        {
            get => _gold;
            
            private set
            {
                if (SetField(ref _gold, value))
                {
                    OnPropertyChanged();
                }
            }
        }
        
        public int Dia
        {
            get => _dia;
            
            private set
            {
                if (SetField(ref _dia, value))
                {
                    OnPropertyChanged();
                }
            }
        }

        public void SetValues(int gold, int dia)
        {
            _gold = gold;
            _dia = dia;
        }
    }
}