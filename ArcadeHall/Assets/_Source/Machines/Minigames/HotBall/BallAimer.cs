using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Machines.Minigames.HotBall
{
    public class BallAimer : MonoBehaviour
    {
        [SerializeField] private Transform arrow;

        [Header("Power Scale")]
        [SerializeField] private Vector3 minPowerScale = new(1f, 1f, 0.3f);
        [SerializeField] private Vector3 maxPowerScale = Vector3.one;

        public event Action<float, float> Aimed;

        private enum Stage { None, Angle, Power }

        private HotBallConfigSO _config;
        private Stage _stage;
        private Quaternion _arrowBaseRotation;
        private Vector3 _arrowBaseScale;
        private float _angle;
        private float _power;
        private Tween _swing;

        private void Awake()
        {
            _arrowBaseRotation = arrow.localRotation;
            _arrowBaseScale = arrow.localScale;

            arrow.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Begin(HotBallConfigSO config)
        {
            _config = config;
            _stage = Stage.Angle;

            arrow.gameObject.SetActive(true);
            SetPower(1f);
            SetAngle(-config.MaxAngle);

            _swing = DOVirtual.Float(-config.MaxAngle, config.MaxAngle, config.AngleSwingDuration, SetAngle)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo)
                .SetId(this);
        }

        public void Press()
        {
            switch (_stage)
            {
                case Stage.Angle:
                    _swing.Kill();
                    _stage = Stage.Power;

                    SetPower(0f);

                    _swing = DOVirtual.Float(0f, 1f, _config.PowerSwingDuration, SetPower)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetId(this);
                    break;

                case Stage.Power:
                    _swing.Kill();
                    _stage = Stage.None;

                    arrow.gameObject.SetActive(false);
                    Aimed?.Invoke(_angle, _power);
                    break;
            }
        }

        private void SetAngle(float angle)
        {
            _angle = angle;
            arrow.localRotation = _arrowBaseRotation * Quaternion.Euler(0f, angle, 0f);
        }

        private void SetPower(float power)
        {
            _power = power;

            Vector3 multiplier = Vector3.Lerp(minPowerScale, maxPowerScale, power);
            arrow.localScale = Vector3.Scale(_arrowBaseScale, multiplier);
        }
    }
}
