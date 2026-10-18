using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Machines.Minigames.KillTheRobot
{
    public class Hammer : MonoBehaviour
    {
        [Header("Swing")]
        [SerializeField] private Transform hammerHandler;
        [SerializeField] private float hitAngleX = 0f;
        [SerializeField] private float swingDownDuration = 0.07f;
        [SerializeField] private float swingUpDuration = 0.12f;

        [Header("Hit")]
        [SerializeField] private LayerMask robotsMask;

        public event Action<Robot> RobotHit;

        private Camera _camera;
        private Plane _plane;
        private Vector3 _startPosition;
        private Quaternion _handlerBaseRotation;
        private Quaternion _handlerHitRotation;
        private bool _isActive;
        private bool _isSwinging;

        private void Awake()
        {
            _startPosition = transform.position;
            _plane = new Plane(Vector3.up, transform.position);

            _handlerBaseRotation = hammerHandler.localRotation;
            Vector3 euler = hammerHandler.localEulerAngles;
            _handlerHitRotation = Quaternion.Euler(hitAngleX, euler.y, euler.z);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void Begin()
        {
            _camera = Camera.main;
            _isActive = true;
        }

        public void End()
        {
            _isActive = false;
            _isSwinging = false;
            DOTween.Kill(this);

            hammerHandler.localRotation = _handlerBaseRotation;
            transform.position = _startPosition;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            Ray ray = GetMouseRay();

            if (_plane.Raycast(ray, out float distance))
                transform.position = ray.GetPoint(distance);
        }

        public void Swing()
        {
            if (!_isActive || _isSwinging)
                return;

            _isSwinging = true;

            DOTween.Sequence()
                .Append(hammerHandler.DOLocalRotateQuaternion(_handlerHitRotation, swingDownDuration).SetEase(Ease.InQuad))
                .AppendCallback(CheckHit)
                .Append(hammerHandler.DOLocalRotateQuaternion(_handlerBaseRotation, swingUpDuration).SetEase(Ease.OutQuad))
                .OnComplete(() => _isSwinging = false)
                .SetId(this);
        }

        private void CheckHit()
        {
            if (!Physics.Raycast(GetMouseRay(), out RaycastHit hit, Mathf.Infinity, robotsMask))
                return;

            Robot robot = hit.collider.GetComponentInParent<Robot>();

            if (robot != null && robot.TryHit())
                RobotHit?.Invoke(robot);
        }

        private Ray GetMouseRay()
        {
            return _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        }
    }
}