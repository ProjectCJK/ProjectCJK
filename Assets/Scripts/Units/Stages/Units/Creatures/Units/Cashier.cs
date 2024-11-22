using Units.Stages.Units.Buildings.UI.ManagementDesks;
using UnityEngine;

namespace Units.Stages.Units.Creatures.Units
{
    public class Cashier : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PaymentView paymentView;
        
        public Animator Animator => animator;
    }
}