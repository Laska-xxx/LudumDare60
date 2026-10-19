using DG.Tweening;
using UnityEngine;

namespace Machines.Minigames.PumpItUp
{
    public class LaneGlow : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer glow;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private float fadeDuration = 0.2f;

        private void Awake()
        {
            SetAlpha(0f);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Flash()
        {
            DOTween.Kill(this);

            SetAlpha(maxAlpha);
            glow.DOFade(0f, fadeDuration).SetId(this);
        }

        private void SetAlpha(float alpha)
        {
            Color color = glow.color;
            color.a = alpha;
            glow.color = color;
        }
    }
}
