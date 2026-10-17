using Player;

namespace Interactable
{
    public interface IInteractable
    {
        public string HintText { get; }

        public void Interact(PlayerInteractor player);
    }
}