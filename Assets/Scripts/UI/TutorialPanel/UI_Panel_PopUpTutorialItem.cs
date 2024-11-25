using UnityEngine;
using UnityEngine.UI;

namespace UI.TutorialPanel
{
    public class UI_Panel_PopUpTutorialItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        public void RegisterReference()
        {
            _button.onClick.AddListener(OnClickPanel);
        }
        
        private void OnClickPanel()
        {
            gameObject.SetActive(false);
        }
    }
}