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
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "DevScene";
        [SerializeField] private UIPanel optionsPanel;
        [SerializeField] private UIPanel creditsPanel;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        private InputReader _inputReader;
        private CursorController _cursorController;

        [Inject]
        private void Init(InputReader inputReader, CursorController cursorController)
        {
            _inputReader = inputReader;
            _cursorController = cursorController;

            _inputReader.GameInput.UI.Back.performed += OnBackPressed;
        }

        private void OnDestroy()
        {
            if (_inputReader?.GameInput != null)
                _inputReader.GameInput.UI.Back.performed -= OnBackPressed;
        }

        private void Awake()
        {
            playButton.onClick.AddListener(Play);
            optionsButton.onClick.AddListener(OpenOptions);
            creditsButton.onClick.AddListener(OpenCredits);
            exitButton.onClick.AddListener(Exit);

#if UNITY_WEBGL
            exitButton.gameObject.SetActive(false);
#endif
        }

        private void Start()
        {
            _inputReader.SwitchActionMapType(ActionMapType.UI);
            _cursorController.UnlockCursor();

            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }

        private void Play()
        {
            _inputReader.SwitchActionMapType(ActionMapType.Player);
            SceneManager.LoadScene(gameSceneName);
        }

        private void OpenOptions()
        {
            optionsPanel.Open();
        }

        private void OpenCredits()
        {
            creditsPanel.Open();
        }

        private void Exit()
        {
            Application.Quit();
        }

        private void OnBackPressed(InputAction.CallbackContext ctx)
        {
            if (optionsPanel.IsOpen)
                optionsPanel.Back();

            else if (creditsPanel.IsOpen)
                creditsPanel.Back();
        }
    }
}