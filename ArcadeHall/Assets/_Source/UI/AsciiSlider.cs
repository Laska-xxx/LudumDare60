using Audio;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AsciiSlider : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI barText;
        [SerializeField] private string filledCell = "[#]";
        [SerializeField] private string emptyCell = "[ ]";

        private Slider _slider;

        public Slider Slider
        {
            get
            {
                if (_slider == null)
                    _slider = GetComponent<Slider>();

                return _slider;
            }
        }

        private void Awake()
        {
            Slider.onValueChanged.AddListener(OnValueChanged);
            Redraw();
        }

        public void Redraw()
        {
            int filled = Mathf.RoundToInt(Slider.value);
            int total = Mathf.RoundToInt(Slider.maxValue);

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < total; i++)
                builder.Append(i < filled ? filledCell : emptyCell);

            barText.text = builder.ToString();
        }

        private void OnValueChanged(float value)
        {
            Redraw();
            UISounds.PlaySlider(Slider.normalizedValue);
        }
    }
}