using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace Machines.Minigames.PumpItUp
{
    public class JudgementView : MonoBehaviour
    {
        [SerializeField] private TMP_Text judgementText;
        [SerializeField] private TMP_Text accuracyText;
        [SerializeField]
        private JudgementStyle[] styles =
        {
            new() { Text = "PERFECT", Color = Color.cyan },
            new() { Text = "GREAT", Color = Color.green },
            new() { Text = "GOOD", Color = Color.yellow },
            new() { Text = "MISS", Color = Color.red }
        };

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Clear()
        {
            DOTween.Kill(this);
            judgementText.text = string.Empty;
            accuracyText.text = string.Empty;
        }

        public void Show(Judgement judgement, float accuracy, float showTime)
        {
            DOTween.Kill(this);

            JudgementStyle style = styles[(int)judgement];
            judgementText.text = style.Text;
            judgementText.color = style.Color;
            judgementText.transform.localScale = Vector3.one;
            judgementText.transform.DOPunchScale(Vector3.one * 0.2f, 0.15f).SetId(this);

            accuracyText.text = $"{accuracy * 100f:0.00}%";

            DOVirtual.DelayedCall(showTime, () => judgementText.text = string.Empty).SetId(this);
        }

        [Serializable]
        private class JudgementStyle
        {
            public string Text;
            public Color Color = Color.white;
        }
    }
}
