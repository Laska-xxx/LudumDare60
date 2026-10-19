using System;
using UnityEngine;

namespace Machines.Minigames.KillTheRobot
{
    [CreateAssetMenu(fileName = "KillTheRobotConfigSO", menuName = "Minigames/KillTheRobotConfig")]
    public class KillTheRobotConfigSO : ScriptableObject
    {
        [field: Header("Game")]
        [field: SerializeField] public float GameDuration { get; private set; } = 60f;
        [field: SerializeField] public float StartDelay { get; private set; } = 0.5f;
        [field: SerializeField] public RobotWave[] Waves { get; private set; }

        [field: Header("Reward")]
        [field: SerializeField, Range(0f, 1f)] public float GoodAccuracy { get; private set; } = 0.7f;
        [field: SerializeField] public int GoodTickets { get; private set; } = 20;
        [field: SerializeField, Range(0f, 1f)] public float PerfectAccuracy { get; private set; } = 0.95f;
        [field: SerializeField] public int PerfectTickets { get; private set; } = 50;
        [field: SerializeField] public float ResultDelay { get; private set; } = 1.5f;

        public float GetWaveDuration(RobotWave wave)
        {
            float totalShare = 0f;

            foreach (RobotWave w in Waves)
                totalShare += w.DurationShare;

            return GameDuration * wave.DurationShare / totalShare;
        }

        public int GetTickets(float accuracy)
        {
            float tickets = accuracy <= GoodAccuracy
                ? Mathf.Lerp(0f, GoodTickets, accuracy / GoodAccuracy)
                : Mathf.Lerp(GoodTickets, PerfectTickets, Mathf.InverseLerp(GoodAccuracy, PerfectAccuracy, accuracy));

            return Mathf.RoundToInt(tickets);
        }
    }

    [Serializable]
    public struct RobotWave
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public float DurationShare { get; private set; }
        [field: SerializeField] public float SpawnInterval { get; private set; }
        [field: SerializeField] public int RobotsPerSpawn { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float[] ExtraRobotChances { get; private set; }
        [field: SerializeField] public float UpTime { get; private set; }
    }
}
