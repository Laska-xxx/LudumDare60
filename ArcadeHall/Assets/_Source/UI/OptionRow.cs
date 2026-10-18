using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class OptionRow : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private Button rowButton;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private string text;

        [Header("Prefixes")]
        [SerializeField] private string normalPrefix = "-";
        [SerializeField] private string selectedPrefix = "->";
        [SerializeField] private string editingPrefix = ">>";

        public bool IsEditing => EventSystem.current.currentSelectedGameObject == slider.gameObject;

        private string _currentPrefix;

        private void Awake()
        {
            rowButton.onClick.AddListener(StartEditing);
        }

        private void Update()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            string prefix = normalPrefix;

            if (selected == slider.gameObject)
                prefix = editingPrefix;
            else if (selected == rowButton.gameObject)
                prefix = selectedPrefix;

            if (prefix == _currentPrefix)
                return;

            _currentPrefix = prefix;
            label.text = prefix + text;
        }

        public bool TryStopEditing()
        {
            if (!IsEditing)
                return false;

            EventSystem.current.SetSelectedGameObject(rowButton.gameObject);
            return true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsEditing)
                EventSystem.current.SetSelectedGameObject(rowButton.gameObject);
        }

        private void StartEditing()
        {
            EventSystem.current.SetSelectedGameObject(slider.gameObject);
        }
    }
}