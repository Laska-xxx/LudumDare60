using Interactable;
using Player;
using UnityEngine;
using Zenject;

namespace Machines
{
    public abstract class ArcadeMachine : MonoBehaviour, IInteractable
    {
        [SerializeField] private string playHint = "PLAY";
        [SerializeField] private string noTokensHint = "NO TOKENS";

        public string HintText => _sessionManager.Tokens > 0 ? playHint : noTokensHint;

        protected bool IsPlaying { get; private set; }

        private SessionManager _sessionManager;

        [Inject]
        private void Init(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void Interact(PlayerInteractor player)
        {
            if (IsPlaying)
                return;

            if (!_sessionManager.TrySpendToken())
                return;

            IsPlaying = true;
            StartGame();
        }

        protected abstract void StartGame();

        protected void FinishGame(int tickets)
        {
            IsPlaying = false;
            _sessionManager.AddTickets(tickets);
        }
    }
}