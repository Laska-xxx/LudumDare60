using Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private string menuSceneName = "MainMenu";
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private UIPanel optionsPanel;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private GameObject continueButtonObj;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button exitButton;

        private InputReader _inputReader;
        private CursorController _cursorController;
        private ActionMapType _previousMap;
        private bool _wasCursorLocked;
        private bool _isPaused;

        [Inject]
        private void Init(InputReader inputReader, CursorController cursorController)
        {
            _inputReader = inputReader;
            _cursorController = cursorController;

            _inputReader.GameInput.Player.Pause.performed += OnPausePressed;
            _inputReader.GameInput.Minigame.Pause.performed += OnPausePressed;
            _inputReader.GameInput.UI.Back.performed += OnBackPressed;
        }

        private void OnDestroy()
        {
            if (_inputReader?.GameInput != null)
            {
                _inputReader.GameInput.Player.Pause.performed -= OnPausePressed;
                _inputReader.GameInput.Minigame.Pause.performed -= OnPausePressed;
                _inputReader.GameInput.UI.Back.performed -= OnBackPressed;
            }
        }

        private void Awake()
        {
            pausePanel.SetActive(false);

            continueButton.onClick.AddListener(Resume);
            optionsButton.onClick.AddListener(OpenOptiens);
            exitButton.onClick.AddListener(ExitToMenu);
        }

        public void Pause()
        {
            if (_isPaused)
                return;

            _isPaused = true;
            _previousMap = _inputReader.CurrentActionMapType;
            _wasCursorLocked = Cursor.lockState == CursorLockMode.Locked;

            Time.timeScale = 0f;
            AudioListener.pause = true;
            _inputReader.SwitchActionMapType(ActionMapType.UI);
            _cursorController.UnlockCursor();

            bestScoreText.text = BestScore.Value.ToString("D4");
            pausePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(continueButtonObj);
        }

        public void Resume()
        {
            if (!_isPaused)
                return;

            _isPaused = false;
            optionsPanel.Close();
            pausePanel.SetActive(false);

            Time.timeScale = 1f;
            AudioListener.pause = false;
            _inputReader.SwitchActionMapType(_previousMap);

            if (_wasCursorLocked)
                _cursorController.LockCursor();
        }

        public void OpenOptiens()
        {
            optionsPanel.Open();
        }

        public void ExitToMenu()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            _inputReader.SwitchActionMapType(ActionMapType.Player);
            SceneManager.LoadScene(menuSceneName);
        }

        private void OnPausePressed(InputAction.CallbackContext ctx)
        {
            Pause();
        }

        private void OnBackPressed(InputAction.CallbackContext ctx)
        {
            if (!_isPaused)
                return;

            if (optionsPanel.IsOpen)
                optionsPanel.Back();
            else
                Resume();
        }
    }
}