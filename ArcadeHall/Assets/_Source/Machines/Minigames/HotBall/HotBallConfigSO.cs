using UnityEngine;

namespace Machines.Minigames.HotBall
{
    [CreateAssetMenu(fileName = "HotBallConfigSO", menuName = "Minigames/HotBallConfig")]
    public class HotBallConfigSO : ScriptableObject
    {
        [field: Header("Game")]
        [field: SerializeField] public int BallsCount { get; private set; } = 10;
        [field: SerializeField] public float NextBallDelay { get; private set; } = 0.5f;

        [field: Header("Aim")]
        [field: SerializeField] public float MaxAngle { get; private set; } = 15f;
        [field: SerializeField] public float AngleSwingDuration { get; private set; } = 1f;
        [field: SerializeField] public float PowerSwingDuration { get; private set; } = 0.8f;

        [field: Header("Launch")]
        [field: SerializeField] public float MinLaunchSpeed { get; private set; } = 3f;
        [field: SerializeField] public float MaxLaunchSpeed { get; private set; } = 8f;
        [field: SerializeField] public float MaxRollTime { get; private set; } = 6f;

        [field: Header("Reward")]
        [field: SerializeField] public int PointsPerTicket { get; private set; } = 5000;
        [field: SerializeField] public int MaxTickets { get; private set; } = 20;
        [field: SerializeField] public float ResultDelay { get; private set; } = 1.5f;

        public float GetLaunchSpeed(float power)
        {
            return Mathf.Lerp(MinLaunchSpeed, MaxLaunchSpeed, power);
        }

        public int GetTickets(int score)
        {
            return Mathf.Min(score / PointsPerTicket, MaxTickets);
        }
    }
}
