using Core;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Machines.Minigames.KillTheRobot
{
    public class KillTheRobotTest : MonoBehaviour
    {
        [SerializeField] private Hammer hammer;
        [SerializeField] private Robot[] robots;
        [SerializeField] private CinemachineCamera machineCamera;
        [SerializeField] private float upTime = 1.5f;

        private InputReader _inputReader;
        private CursorController _cursorController;
        private bool _isReady;

        [Inject]
        private void Init(InputReader inputReader, CursorController cursorController)
        {
            _inputReader = inputReader;
            _cursorController = cursorController;
        }

        private IEnumerator Start()
        {
            yield return null;

            machineCamera.gameObject.SetActive(true);
            _inputReader.SwitchActionMapType(ActionMapType.Minigame);
            _cursorController.UnlockCursor();

            hammer.Begin();
            hammer.RobotHit += robot => Debug.Log($"HIT {robot.name}");

            _isReady = true;
        }

        private void Update()
        {
            if (!_isReady)
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
                hammer.Swing();

            if (Keyboard.current.rKey.wasPressedThisFrame)
                robots[Random.Range(0, robots.Length)].Show(upTime);
        }
    }
}