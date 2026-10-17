using Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float jumpHeight;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckDistance = 0.1f;

        private InputReader _inputReader;
        private Rigidbody _rb;
        private CapsuleCollider _capsule;

        private Vector2 _moveInput;
        private bool _jumpRequested;

        [Inject]
        private void Init(InputReader input)
        {
            _inputReader = input;

            _inputReader.GameInput.Player.Jump.performed += OnJump;
        }

        private void OnDestroy()
        {
            if (_inputReader?.GameInput != null)
                _inputReader.GameInput.Player.Jump.performed -= OnJump;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();

            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        private void Update()
        {
            if (_inputReader?.GameInput == null || _inputReader.CurrentActionMapType != ActionMapType.Player)
            {
                _moveInput = Vector2.zero;
                return;
            }

            _moveInput = _inputReader.GameInput.Player.Move.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            Vector3 wishDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            Vector3 targetVelocity = Vector3.ClampMagnitude(wishDir, 1f) * moveSpeed;

            Vector3 velocity = _rb.linearVelocity;
            Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
            horizontal = Vector3.MoveTowards(horizontal, targetVelocity, acceleration * Time.fixedDeltaTime);
            velocity.x = horizontal.x;
            velocity.z = horizontal.z;

            if (_jumpRequested && IsGrounded())
                velocity.y = Mathf.Sqrt(2f * jumpHeight * -Physics.gravity.y);

            _jumpRequested = false;

            _rb.linearVelocity = velocity;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _jumpRequested = true;
        }

        private bool IsGrounded()
        {
            float radius = _capsule.radius * 0.95f;
            Vector3 origin = transform.TransformPoint(_capsule.center);
            float distance = _capsule.height * 0.5f - radius + groundCheckDistance;

            return Physics.SphereCast(origin, radius, Vector3.down, out _, distance,
                Physics.AllLayers, QueryTriggerInteraction.Ignore);
        }
    }
}