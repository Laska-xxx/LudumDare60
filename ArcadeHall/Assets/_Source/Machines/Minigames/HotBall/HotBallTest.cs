using Core;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Machines.Minigames.HotBall
{
    public class HotBallTest : MonoBehaviour
    {
        [SerializeField] private Ball ball;
        [SerializeField] private Transform launchPoint;
        [SerializeField] private CinemachineCamera machineCamera;
        [SerializeField] private float launchSpeed = 6f;
        [SerializeField] private float maxRollTime = 6f;

        private InputReader _inputReader;
        private bool _isReady;

        [Inject]
        private void Init(InputReader inputReader)
        {
            _inputReader = inputReader;
        }

        private IEnumerator Start()
        {
            yield return null;

            machineCamera.gameObject.SetActive(true);
            _inputReader.SwitchActionMapType(ActionMapType.Minigame);

            ball.Landed += points => Debug.Log($"LANDED: {points}");
            ball.PlaceAt(launchPoint);
            _isReady = true;
        }

        private void Update()
        {
            if (!_isReady)
                return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                ball.Launch(launchPoint.forward * launchSpeed, maxRollTime);

            if (Keyboard.current.rKey.wasPressedThisFrame)
                ball.PlaceAt(launchPoint);
        }
    }
}
