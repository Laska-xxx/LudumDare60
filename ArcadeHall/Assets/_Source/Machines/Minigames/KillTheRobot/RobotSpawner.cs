using System.Collections.Generic;
using UnityEngine;

namespace Machines.Minigames.KillTheRobot
{
    public class RobotSpawner : MonoBehaviour
    {
        [SerializeField] private Robot[] robots;

        private readonly List<Robot> _freeRobots = new();

        public bool AllHidden
        {
            get
            {
                foreach (Robot robot in robots)
                {
                    if (robot.IsBusy)
                        return false;
                }

                return true;
            }
        }

        public int Spawn(RobotWave wave)
        {
            int count = GetSpawnCount(wave);
            int spawned = 0;

            for (int i = 0; i < count; i++)
            {
                Robot robot = GetFreeRobot();

                if (robot == null)
                    break;

                robot.Show(wave.UpTime);
                spawned++;
            }

            return spawned;
        }

        private int GetSpawnCount(RobotWave wave)
        {
            int count = wave.RobotsPerSpawn;

            foreach (float chance in wave.ExtraRobotChances)
            {
                if (Random.value < chance)
                    count++;
            }

            return count;
        }

        private Robot GetFreeRobot()
        {
            _freeRobots.Clear();

            foreach (Robot robot in robots)
            {
                if (!robot.IsBusy)
                    _freeRobots.Add(robot);
            }

            return _freeRobots.Count == 0 ? null : _freeRobots[Random.Range(0, _freeRobots.Count)];
        }
    }
}