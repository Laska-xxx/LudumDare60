using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
        }

        public void OnSelect(BaseEventData eventData) => SetSelected(true);

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