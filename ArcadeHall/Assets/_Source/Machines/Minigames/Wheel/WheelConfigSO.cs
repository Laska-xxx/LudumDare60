using System;
using UnityEngine;

namespace Machines.Minigames.Wheel
{
    [CreateAssetMenu(fileName = "WheelConfigSO", menuName = "Minigames/WheelConfig")]
    public class WheelConfigSO : ScriptableObject
    {
        [field: SerializeField] public WheelSector[] Sectors { get; private set; }
        [field: SerializeField] public float SpinDuration { get; private set; } = 4f;
        [field: SerializeField, Min(1)] public int MinExtraRotations { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int MaxExtraRotations { get; private set; } = 5;
        [field: SerializeField, Range(0f, 0.45f)] public float SectorOffsetRange { get; private set; } = 0.35f;
        [field: SerializeField] public float ResultDelay { get; private set; } = 1f;

        public int ExtraRotations
        {
            get
            {
                return UnityEngine.Random.Range(MinExtraRotations, MaxExtraRotations + 1);
            }
        
        }
    }

        [Serializable]
    public struct WheelSector
    {
        public int Tickets;
        [Min(0f)] public float Weight;
    }
}