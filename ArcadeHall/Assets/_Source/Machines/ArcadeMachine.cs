using Core;
using Interactable;
using Player;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Machines
{
    public abstract class ArcadeMachine : MonoBehaviour, IInteractable
    {
        [SerializeField] private string playHint = "PLAY";
        [SerializeField] private string noTokensHint = "NO TOKENS";
        [SerializeField] private CinemachineCamera machineCamera;
        [SerializeField] private bool unlockCursor;

        public string HintText => _sessionManager.Tokens > 0 ? playHint : noTokensHint;

        protected bool IsPlaying { get; private set; }
        protected InputReader InputReader;

        private SessionManager _sessionManager;
        private CursorController _cursorController;


        [Inject]
        private void Init(InputReader inputReader, SessionManager sessionManager, CursorController cursorController)
        {
            InputReader = inputReader;
            _sessionManager = sessionManager;
            _cursorController = cursorController;

            machineCamera.gameObject.SetActive(false);
        }

        public void Interact(PlayerInteractor player)
        {
            if (IsPlaying)
                return;

            if (!_sessionManager.TrySpendToken())
                return;

            IsPlaying = true;
            EnterMachine();
            StartGame();
        }

        protected abstract void StartGame();

        protected void FinishGame(int tickets)
        {
            IsPlaying = false;
            _sessionManager.AddTickets(tickets);
            ExitMachine();
        }

        private void EnterMachine()
        {
            machineCamera.gameObject.SetActive(true);
            InputReader.SwitchActionMapType(ActionMapType.Minigame);

            if (unlockCursor)
                _cursorController.UnlockCursor();
        }

        private void ExitMachine()
        {
            machineCamera.gameObject.SetActive(false);
            InputReader.SwitchActionMapType(ActionMapType.Player);
            _cursorController.LockCursor();
        }
    }
}