using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Machines.Minigames.KillTheRobot
{
    public class Hammer : MonoBehaviour
    {
        [Header("Work Pose")]
        [SerializeField] private Vector3 workLocalPosition = new(0f, 0.845f, 0f);
        [SerializeField] private Vector3 workLocalRotation = Vector3.zero;
        [SerializeField] private float pickUpDuration = 0.4f;

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
        private Vector3 _restPosition;
        private Quaternion _restRotation;
        private Quaternion _workRotation;
        private Quaternion _handlerBaseRotation;
        private Quaternion _handlerHitRotation;
        private bool _isActive;
        private bool _isSwinging;

        private void Awake()
        {
            _restPosition = transform.localPosition;
            _restRotation = transform.localRotation;
            _workRotation = Quaternion.Euler(workLocalRotation);

            Vector3 workWorldPosition = transform.parent.TransformPoint(workLocalPosition);
            _plane = new Plane(Vector3.up, workWorldPosition);

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
            DOTween.Kill(this);

            DOTween.Sequence()
                .Append(transform.DOLocalMove(workLocalPosition, pickUpDuration).SetEase(Ease.OutQuad))
                .Join(transform.DOLocalRotateQuaternion(_workRotation, pickUpDuration).SetEase(Ease.OutQuad))
                .OnComplete(() => _isActive = true)
                .SetId(this);
        }

        public void End()
        {
            _isActive = false;
            _isSwinging = false;
            DOTween.Kill(this);

            hammerHandler.localRotation = _handlerBaseRotation;

            DOTween.Sequence()
                .Append(transform.DOLocalMove(_restPosition, pickUpDuration).SetEase(Ease.InOutQuad))
                .Join(transform.DOLocalRotateQuaternion(_restRotation, pickUpDuration).SetEase(Ease.InOutQuad))
                .SetId(this);
        }

        private void Update()
        {
            if (!_isActive || Time.timeScale == 0f)
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