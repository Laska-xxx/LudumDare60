using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Machines.Minigames.HotBall
{
    public class HotBallMachine : ArcadeMachine
    {
        [Header("Hot Ball")]
        [SerializeField] private HotBallConfigSO config;
        [SerializeField] private Ball ball;
        [SerializeField] private BallAimer aimer;
        [SerializeField] private Transform launchPoint;

        [Header("Screen")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI ballsText;

        [Header("Sounds")]
        [SerializeField] private AudioClip holeSound;

        private int _score;
        private int _ballsLeft;

        private void Start()
        {
            SetScreenVisible(false);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
            ball.Landed -= OnBallLanded;
            aimer.Aimed -= OnAimed;

            if (InputReader?.GameInput != null)
                InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;
        }

        protected override void StartGame()
        {
            _score = 0;
            _ballsLeft = config.BallsCount;

            UpdateScreen();
            SetScreenVisible(true);

            ball.Landed += OnBallLanded;
            aimer.Aimed += OnAimed;
            InputReader.GameInput.Minigame.Action.performed += OnActionPressed;

            PrepareBall();
        }

        private void PrepareBall()
        {
            if (_ballsLeft == 0)
            {
                EndGame();
                return;
            }

            ball.PlaceAt(launchPoint);
            aimer.Begin(config);
        }

        private void OnActionPressed(InputAction.CallbackContext ctx)
        {
            aimer.Press();
        }

        private void OnAimed(float angle, float power)
        {
            _ballsLeft--;
            UpdateScreen();

            Vector3 direction = Quaternion.AngleAxis(angle, launchPoint.up) * launchPoint.forward;
            ball.Launch(direction * config.GetLaunchSpeed(power), config.MaxRollTime);
        }

        private void OnBallLanded(int points)
        {
            if (points > 0)
                PlaySound(holeSound);

            _score += points;
            UpdateScreen();

            DOVirtual.DelayedCall(config.NextBallDelay, PrepareBall, false).SetId(this);
        }

        private void EndGame()
        {
            ball.Landed -= OnBallLanded;
            aimer.Aimed -= OnAimed;
            InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;

            ball.ReturnToRest();

            int tickets = config.GetTickets(_score);
            ballsText.text = $"+{tickets}";

            DOVirtual.DelayedCall(config.ResultDelay, () =>
            {
                SetScreenVisible(false);
                FinishGame(tickets);
            }, false)
                .SetId(this);
        }

        private void UpdateScreen()
        {
            scoreText.text = _score.ToString();
            ballsText.text = _ballsLeft.ToString();
        }

        private void SetScreenVisible(bool isVisible)
        {
            scoreText.gameObject.SetActive(isVisible);
            ballsText.gameObject.SetActive(isVisible);
        }
    }
}
