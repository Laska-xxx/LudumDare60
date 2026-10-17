using Core;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Transform view;
        [SerializeField] private float sensitivity = 0.1f;
        [SerializeField] private float minAnglY = -80f;
        [SerializeField] private float maxAnglY = 80f;

        private InputReader _inputReader;
        private Rigidbody _rb;
        private CursorController _cursorController;
        private float _anglX;
        private float _anglY;

        [Inject]
        private void Init(InputReader input, CursorController cursorController)
        {
            _inputReader = input;
            _cursorController = cursorController;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _anglX = transform.eulerAngles.y;
        }

        private void Start()
        {
            _cursorController.LockCursor();
        }

        private void Update()
        {
            if (_inputReader?.GameInput == null || _inputReader.CurrentActionMapType != ActionMapType.Player)
                return;

            Vector2 look = _inputReader.GameInput.Player.Look.ReadValue<Vector2>() * sensitivity;

            _anglX += look.x;
            _anglY = Mathf.Clamp(_anglY - look.y, minAnglY, maxAnglY);

            view.localRotation = Quaternion.Euler(_anglY, 0f, 0f);
        }

        private void FixedUpdate()
        {
            _rb.MoveRotation(Quaternion.Euler(0f, _anglX, 0f));
        }
    }
}