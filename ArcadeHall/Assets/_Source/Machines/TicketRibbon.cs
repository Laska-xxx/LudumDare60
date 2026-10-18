using Interactable;
using Player;
using UnityEngine;

namespace Machines
{
    public class TicketRibbon : MonoBehaviour, IInteractable
    {
        public string HintText => _dispenser.RibbonHint;

        private TicketsDispenser _dispenser;

        public void Init(TicketsDispenser dispenser)
        {
            _dispenser = dispenser;
        }

        public void Interact(PlayerInteractor player)
        {
            _dispenser.Collect();
        }
    }
}