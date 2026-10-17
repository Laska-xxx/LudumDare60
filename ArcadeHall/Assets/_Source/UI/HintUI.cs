using Player;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class HintUI : MonoBehaviour
    {
        [SerializeField] private GameObject hintContainer;
        [SerializeField] private TextMeshProUGUI hintText;

        private PlayerInteractor _playerInteractor;

        [Inject]
        private void Init(PlayerInteractor playerInteractor)
        {
            _playerInteractor = playerInteractor;
        }

        private void Awake()
        {
            HideHint();
        }

        private void OnEnable()
        {
            if (_playerInteractor != null)
            {
                _playerInteractor.OnInteractableFound += ShowHint;
                _playerInteractor.OnInteractableLost += HideHint;
            }
        }

        private void OnDisable()
        {
            if (_playerInteractor != null)
            {
                _playerInteractor.OnInteractableFound -= ShowHint;
                _playerInteractor.OnInteractableLost -= HideHint;
            }
        }

        private void ShowHint(string message)
        {
            hintText.text = $"[E] {message}";
            hintContainer.SetActive(true);
        }

        private void HideHint()
        {
            hintContainer.SetActive(false);
        }
    }
}