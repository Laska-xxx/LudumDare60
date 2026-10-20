using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ConsoleButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private string text;
        [SerializeField] private string selectedPrefix = "->";
        [SerializeField] private string normalPrefix = "-";

        private void Awake()
        {
            SetSelected(false);

            if (TryGetComponent(out Button button))
                button.onClick.AddListener(UISounds.PlayClick);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetSelected(true);
            UISounds.PlaySelect();
        }

        public void OnDeselect(BaseEventData eventData) => SetSelected(false);

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        private void SetSelected(bool isSelected)
        {
            label.text = (isSelected ? selectedPrefix : normalPrefix) + text;
        }
    }
}