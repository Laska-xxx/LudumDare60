using DG.Tweening;
using System;
using UnityEngine;

namespace Machines.Minigames.HotBall
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;

        public event Action<int> Landed;

        private Vector3 _restPosition;
        private Quaternion _restRotation;
        private bool _isRolling;

        private void Awake()
        {
            _restPosition = transform.position;
            _restRotation = transform.rotation;
            Freeze();
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }

        public void PlaceAt(Transform point)
        {
            Freeze();
            transform.SetPositionAndRotation(point.position, point.rotation);
        }

        public void ReturnToRest()
        {
            Freeze();
            transform.SetPositionAndRotation(_restPosition, _restRotation);
        }

        public void Launch(Vector3 velocity, float maxRollTime)
        {
            body.isKinematic = false;
            body.AddForce(velocity, ForceMode.VelocityChange);
            _isRolling = true;

            DOVirtual.DelayedCall(maxRollTime, () => Land(0), false).SetId(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ScoreHole hole))
                Land(hole.Points);
        }

        private void Land(int points)
        {
            if (!_isRolling)
                return;

            _isRolling = false;
            DOTween.Kill(this);
            Landed?.Invoke(points);
        }

        private void Freeze()
        {
            _isRolling = false;
            DOTween.Kill(this);

            if (!body.isKinematic)
            {
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            body.isKinematic = true;
        }
    }
}