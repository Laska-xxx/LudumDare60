using Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Machines.Minigames.CrazyMonkey
{
    public class CrazyMonkeyMachine : ArcadeMachine
    {
        [Header("Strength")]
        [SerializeField] private CrazyMonkeyConfigSO config;
        [SerializeField] private Slider slider;
        [SerializeField] private GameObject playerHand;
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private string goText = "GO!";

        [Header("Hands")]
        [SerializeField] private Transform hands;
        [SerializeField] private Vector3 handsAxis;
        [SerializeField] private float gorillaWinAngle = 45f;
        [SerializeField] private float playerWinAngle = -45f;

        [Header("Sounds")]
        [SerializeField] private AudioClip startSound;
        [SerializeField] private AudioClip gorillaSound;
        [SerializeField] private AudioClip victorySound;
        [SerializeField] private AudioClip chestBeatSound;
        [SerializeField] private AudioClip gorillaWinSound;
        [SerializeField] private AudioClip[] playerScreams;
        [SerializeField] private float screamChance = 0.2f;
        [SerializeField] private float screamCooldown = 1f;

        private Quaternion _handsBaseRotation;
        private float _fill;
        private float _elapsed;
        private bool _isWrestling;
        private float _nextScreamTime;
        private string _currentScreenText;

        private void Start()
        {
            _handsBaseRotation = hands.localRotation;

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.interactable = false;

            SetFill(config.StartFill);
            SetVisuals(showHand: false, showText: false, showSlider: false);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);

            if (InputReader?.GameInput != null)
                InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;
        }

        protected override void StartGame()
        {
            _elapsed = 0f;
            SetFill(config.StartFill);
            SetVisuals(showHand: true, showText: true, showSlider: false);
            PlaySound(startSound);

            PlayCountdown();
        }

        private void PlayCountdown()
        {
            Sequence countdown = DOTween.Sequence().SetId(this);

            for (int i = config.CountdownFrom; i > 0; i--)
            {
                string number = i.ToString();
                countdown.AppendCallback(() => countdownText.text = number);
                countdown.AppendInterval(config.CountdownStep);
            }

            countdown.AppendCallback(() => countdownText.text = goText);
            countdown.AppendInterval(config.GoDuration);
            countdown.AppendCallback(BeginWrestling);
        }

        private void BeginWrestling()
        {
            SetVisuals(showHand: true, showText: false, showSlider: true);
            PlaySound(gorillaSound);
            PlaySound(chestBeatSound);

            _nextScreamTime = 0f;
            _isWrestling = true;
            InputReader.GameInput.Minigame.Action.performed += OnActionPressed;
        }

        private void Update()
        {
            if (!_isWrestling)
                return;

            _elapsed += Time.deltaTime;
            SetFill(_fill - config.DrainSpeed * Time.deltaTime);

            if (_elapsed >= config.GameDuration)
                EndWrestling();
        }

        private void OnActionPressed(InputAction.CallbackContext ctx)
        {
            if (!_isWrestling)
                return;

            SetFill(_fill + config.ClickRestore);
            TryScream();

            if (_fill >= 1f)
                EndWrestling();
        }

        private void EndWrestling()
        {
            _isWrestling = false;
            InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;

            PlaySound(_fill >= 1f ? victorySound : gorillaWinSound);

            int tickets = Mathf.RoundToInt(Mathf.Lerp(config.MinTickets, config.MaxTickets, _fill));

            SetVisuals(showHand: false, showText: true, showSlider: false);
            countdownText.text = $"+{tickets}";

            DOVirtual.DelayedCall(config.ResultDelay, () =>
            {
                SetVisuals(showHand: false, showText: false, showSlider: false);
                SetFill(config.StartFill);
                FinishGame(tickets);
            }, false)
                .SetId(this);
        }

        private void TryScream()
        {
            if (playerScreams == null || playerScreams.Length == 0)
                return;

            if (_elapsed < _nextScreamTime || Random.value > screamChance)
                return;

            _nextScreamTime = _elapsed + screamCooldown;
            PlaySound(playerScreams[Random.Range(0, playerScreams.Length)]);
        }

        private void SetFill(float value)
        {
            _fill = Mathf.Clamp01(value);
            slider.value = _fill;

            float angle = Mathf.Lerp(gorillaWinAngle, playerWinAngle, _fill);
            hands.localRotation = _handsBaseRotation * Quaternion.AngleAxis(angle, handsAxis);
        }

        private void SetVisuals(bool showHand, bool showText, bool showSlider)
        {
            playerHand.SetActive(showHand);
            countdownText.gameObject.SetActive(showText);
            slider.gameObject.SetActive(showSlider);
        }
    }
}