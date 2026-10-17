using Core;
using Interactable;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Transform view;
        [SerializeField] private float interactionDistance = 2.5f;
        [SerializeField] private LayerMask interactionMask = ~0;

        public event Action<string> OnInteractableFound;
        public event Action OnInteractableLost;

        private InputReader _inputReader;
        private IInteractable _currentInteractable;

        [Inject]
        private void Init(InputReader inputReader)
        {
            _inputReader = inputReader;

            _inputReader.GameInput.Player.Interact.performed += TryInteract;
        }

        private void OnDestroy()
        {
            if (_inputReader?.GameInput != null)
                _inputReader.GameInput.Player.Interact.performed -= TryInteract;
        }

        private void Update()
        {
            if (_inputReader.CurrentActionMapType != ActionMapType.Player)
            {
                LoseInteractable();
                return;
            }

            IInteractable interactable = FindInteractable();

            if (interactable == null)
            {
                LoseInteractable();
                return;
            }

            if (interactable != _currentInteractable)
            {
                _currentInteractable = interactable;
                OnInteractableFound?.Invoke(_currentInteractable.HintText);
            }
        }

        private IInteractable FindInteractable()
        {
            if (!Physics.Raycast(view.position, view.forward, out RaycastHit hit,
                    interactionDistance, interactionMask, QueryTriggerInteraction.Ignore))
                return null;

            return hit.collider.GetComponentInParent<IInteractable>();
        }

        private void LoseInteractable()
        {
            if (_currentInteractable == null)
                return;

            _currentInteractable = null;
            OnInteractableLost?.Invoke();
        }

        private void TryInteract(InputAction.CallbackContext ctx)
        {
            _currentInteractable?.Interact(this);
        }
    }
}