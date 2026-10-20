using Core;
using DG.Tweening;
using Session;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class ResultsUI : MonoBehaviour
    {
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private TextMeshProUGUI ticketsText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private GameObject firstSelected;
        [SerializeField] private float showDelay = 1f;
        [SerializeField] private string menuSceneName = "MainMenu";

        [Header("Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        private SignalBus _signalBus;
        private InputReader _inputReader;
        private CursorController _cursorController;

        [Inject]
        private void Init(SignalBus signalBus, InputReader inputReader, CursorController cursorController)
        {
            _signalBus = signalBus;
            _inputReader = inputReader;
            _cursorController = cursorController;
        }

        private void Awake()
        {
            resultsPanel.SetActive(false);

            restartButton.onClick.AddListener(Restart);
            exitButton.onClick.AddListener(ExitToMenu);
        }

        private void OnEnable()
        {
            _signalBus?.Subscribe<SessionEndedSignal>(OnSessionEnded);
        }

        private void OnDisable()
        {
            DOTween.Kill(this);
            _signalBus?.TryUnsubscribe<SessionEndedSignal>(OnSessionEnded);
        }

        public void Restart()
        {
            _inputReader.SwitchActionMapType(ActionMapType.Player);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void ExitToMenu()
        {
            _inputReader.SwitchActionMapType(ActionMapType.Player);
            SceneManager.LoadScene(menuSceneName);
        }

        private void OnSessionEnded(SessionEndedSignal signal)
        {
            _inputReader.SwitchActionMapType(ActionMapType.UI);

            DOVirtual.DelayedCall(showDelay, () => Show(signal)).SetId(this);
        }

        private void Show(SessionEndedSignal signal)
        {
            ticketsText.text = signal.Tickets.ToString("D4");
            bestScoreText.text = BestScore.Value.ToString("D4");

            resultsPanel.SetActive(true);
            _cursorController.UnlockCursor();
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
}