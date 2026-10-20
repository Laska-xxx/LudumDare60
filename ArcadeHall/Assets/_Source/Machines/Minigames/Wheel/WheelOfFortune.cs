using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Machines.Minigames.Wheel
{
    public class WheelOfFortune : ArcadeMachine
    {
        [Header("Wheel")]
        [SerializeField] private WheelConfigSO config;
        [SerializeField] private Transform wheel;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        [Header("Canvas")]
        [SerializeField] private TMP_Text screenText;
        [SerializeField] private string idleText = "PRESS SPACE";

        [Header("Sounds")]
        [SerializeField] private AudioClip handleSound;
        [SerializeField] private AudioSource spinSource;
        [SerializeField] private AudioClip victorySound;
        [SerializeField] private AudioClip lossSound;
        [SerializeField] private int victoryMinTickets = 10;

        private Quaternion _baseRotation;
        private float _currentAngle;
        private bool _isSpinning;

        private void Start()
        {
            _baseRotation = wheel.localRotation;
            SetScreenText(idleText);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);

            if (InputReader?.GameInput != null)
                InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;
        }

        protected override void StartGame()
        {
            _isSpinning = false;
            SetScreenText(idleText);
            InputReader.GameInput.Minigame.Action.performed += OnActionPressed;
        }

        private void OnActionPressed(InputAction.CallbackContext ctx)
        {
            if (_isSpinning)
                return;

            InputReader.GameInput.Minigame.Action.performed -= OnActionPressed;
            Spin();
        }

        private void Spin()
        {
            _isSpinning = true;

            PlaySound(handleSound);

            if (spinSource != null)
                spinSource.Play();

            int sectorIndex = PickSector();
            float sectorAngle = 360f / config.Sectors.Length;
            float offset = Random.Range(-config.SectorOffsetRange, config.SectorOffsetRange) * sectorAngle;

            float currentTurnStart = _currentAngle - (_currentAngle % 360f);
            float targetAngle = currentTurnStart
                                + config.ExtraRotations * 360f
                                + sectorIndex * sectorAngle
                                + offset;

            DOTween.To(() => _currentAngle, SetAngle, targetAngle, config.SpinDuration)
                .SetEase(Ease.OutQuart)
                .SetId(this)
                .OnComplete(() => OnSpinFinished(sectorIndex));
        }

        private void OnSpinFinished(int sectorIndex)
        {
            int tickets = config.Sectors[sectorIndex].Tickets;
            SetScreenText($"+{tickets}");

            if (spinSource != null)
                spinSource.Stop();

            PlaySound(tickets >= victoryMinTickets ? victorySound : lossSound);

            DOVirtual.DelayedCall(config.ResultDelay, () =>
            {
                _isSpinning = false;
                SetScreenText(idleText);
                FinishGame(tickets);
            }, false)
                .SetId(this);
        }

        private int PickSector()
        {
            float totalWeight = 0f;
            foreach (WheelSector sector in config.Sectors)
                totalWeight += sector.Weight;

            float roll = Random.Range(0f, totalWeight);

            for (int i = 0; i < config.Sectors.Length; i++)
            {
                roll -= config.Sectors[i].Weight;
                if (roll <= 0f)
                    return i;
            }

            return config.Sectors.Length - 1;
        }

        private void SetAngle(float angle)
        {
            _currentAngle = angle;
            wheel.localRotation = _baseRotation * Quaternion.AngleAxis(angle, rotationAxis);
        }

        private void SetScreenText(string text)
        {
            if (screenText != null)
                screenText.text = text;
        }
    }
}