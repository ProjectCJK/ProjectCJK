using Modules.DesignPatterns.MVVMs;
using TMPro;

namespace Units.Stages.UI
{
    public class CurrencyView : BaseView<CurrencyViewModel>
    {
        public TextMeshProUGUI GoldText;
        public TextMeshProUGUI DiaText;
    }
}