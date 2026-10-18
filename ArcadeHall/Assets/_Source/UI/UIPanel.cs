using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private CanvasGroup ownerGroup;
        [SerializeField] private GameObject firstSelected;
        [SerializeField] private OptionRow[] rows;

        public bool IsOpen { get { return optionsPanel.activeSelf; } }

        private GameObject _returnSelection;

        private void Awake()
        {
            optionsPanel.SetActive(false);
        }

        public void Open()
        {
            if (IsOpen)
                return;

            _returnSelection = EventSystem.current.currentSelectedGameObject;
            SetOwnerInteractable(false);

            optionsPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        public void Back()
        {
            foreach (OptionRow row in rows)
            {
                if (row.TryStopEditing())
                    return;
            }

            Close();
        }

        public void Close()
        {
            if (!IsOpen)
                return;

            optionsPanel.SetActive(false);
            SetOwnerInteractable(true);

            EventSystem.current.SetSelectedGameObject(_returnSelection);
        }

        private void SetOwnerInteractable(bool isInteractable)
        {
            if (ownerGroup == null)
                return;

            ownerGroup.interactable = isInteractable;
            ownerGroup.blocksRaycasts = isInteractable;
        }
    }
}