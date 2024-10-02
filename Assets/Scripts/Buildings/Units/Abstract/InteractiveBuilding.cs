using Buildings.Abstract;
using Buildings.Modules;
using UnityEngine;

namespace Buildings.Units.Abstract
{
    public abstract class InteractiveBuilding : BaseBuilding
    {
        [SerializeField] protected InteractionTrade interactionTrade;
    }
}