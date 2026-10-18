using DG.Tweening;
using UnityEngine;

namespace Machines.Minigames.KillTheRobot
{
    public class Robot : MonoBehaviour
    {
        [SerializeField] private Vector3 hiddenOffset = new(0f, -0.3f, 0f);
        [SerializeField] private float popDuration = 0.15f;

        [Header("Hit")]
        [SerializeField] private Vector3 hitSquashScale = new(1.2f, 0.6f, 1.2f);
        [SerializeField] private float hitSquashDuration = 0.08f;

        private Vector3 _upPosition;
        private Vector3 _downPosition;
        private Vector3 _baseScale;

        public bool IsUp { get; private set; }
        public bool IsBusy { get; private set; }

        private void Awake()
        {
            _upPosition = transform.localPosition;
            _downPosition = _upPosition + hiddenOffset;
            _baseScale = transform.localScale;

            transform.localPosition = _downPosition;
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Show(float upTime)
        {
            DOTween.Kill(this);

            IsUp = true;
            IsBusy = true;

            DOTween.Sequence()
                .Append(transform.DOLocalMove(_upPosition, popDuration).SetEase(Ease.OutQuad))
                .AppendInterval(upTime)
                .AppendCallback(() => IsUp = false)
                .Append(transform.DOLocalMove(_downPosition, popDuration).SetEase(Ease.InQuad))
                .OnComplete(() => IsBusy = false)
                .SetId(this);
        }

        public bool TryHit()
        {
            if (!IsUp)
                return false;

            DOTween.Kill(this);
            IsUp = false;

            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.Scale(_baseScale, hitSquashScale), hitSquashDuration))
                .Append(transform.DOLocalMove(_downPosition, popDuration))
                .Join(transform.DOScale(_baseScale, popDuration))
                .OnComplete(() => IsBusy = false)
                .SetId(this);

            return true;
        }

        public void HideInstant()
        {
            DOTween.Kill(this);

            IsUp = false;
            IsBusy = false;
            transform.localPosition = _downPosition;
            transform.localScale = _baseScale;
        }
    }
}