using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Machines.Minigames.KillTheRobot
{
    public class KillTheRobotMachine : ArcadeMachine
    {
        [Header("Kill The Robot")]
        [SerializeField] private KillTheRobotConfigSO config;
        [SerializeField] private Hammer hammer;
        [SerializeField] private RobotSpawner robotSpawner;

        [Header("Screen")]
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private string scoreFormat = "00";

        private int _spawned;
        private int _hits;

        private void Start()
        {
            SetScreenVisible(false);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
            hammer.RobotHit -= OnRobotHit;

            if (InputReader?.GameInput != null)
                InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;
        }

        protected override void StartGame()
        {
            _spawned = 0;
            _hits = 0;

            waveText.text = string.Empty;
            UpdateScore();
            SetScreenVisible(true);

            hammer.Begin();
            hammer.RobotHit += OnRobotHit;
            InputReader.GameInput.Minigame.Action.performed += OnActionPressed;

            StartCoroutine(GameRoutine());
        }

        private IEnumerator GameRoutine()
        {
            yield return new WaitForSeconds(config.StartDelay);

            foreach (RobotWave wave in config.Waves)
            {
                waveText.text = wave.Name;

                float waveDuration = config.GetWaveDuration(wave);
                float waveTime = 0f;

                while (waveTime < waveDuration)
                {
                    _spawned += robotSpawner.Spawn(wave);
                    yield return new WaitForSeconds(wave.SpawnInterval);
                    waveTime += wave.SpawnInterval;
                }
            }

            yield return new WaitUntil(() => robotSpawner.AllHidden);

            EndGame();
        }

        private void OnActionPressed(InputAction.CallbackContext ctx)
        {
            hammer.Swing();
        }

        private void OnRobotHit(Robot robot)
        {
            _hits++;
            UpdateScore();
        }

        private void EndGame()
        {
            hammer.RobotHit -= OnRobotHit;
            InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;
            hammer.End();

            float accuracy = _spawned == 0 ? 0f : (float)_hits / _spawned;
            int tickets = config.GetTickets(accuracy);

            waveText.text = $"+{tickets}";

            DOVirtual.DelayedCall(config.ResultDelay, () =>
            {
                SetScreenVisible(false);
                FinishGame(tickets);
            })
                .SetId(this);
        }

        private void UpdateScore()
        {
            scoreText.text = _hits.ToString(scoreFormat);
        }

        private void SetScreenVisible(bool isVisible)
        {
            waveText.gameObject.SetActive(isVisible);
            scoreText.gameObject.SetActive(isVisible);
        }
    }
}