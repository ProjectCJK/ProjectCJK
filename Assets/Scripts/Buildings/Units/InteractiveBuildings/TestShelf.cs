using Buildings.Abstract;
using Buildings.Interfaces;
using Buildings.Units.Abstract;

namespace Buildings.Units.InteractiveBuildings
{
    public class TestShelf : InteractiveBuilding, ISendObject, IReceiveObject
    {
        public override void Initialize()
        {
            interactionTrade.OnSendObject += SendObject;
            interactionTrade.OnReceiveObject += ReceiveObject;
        }
        
        public void SendObject()
        {
            
        }

        public void ReceiveObject()
        {
            
        }
    }
}