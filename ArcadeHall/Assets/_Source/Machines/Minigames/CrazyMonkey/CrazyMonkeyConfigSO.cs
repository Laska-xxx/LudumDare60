using UnityEngine;

namespace Machines.Minigames.CrazyMonkey
{
    [CreateAssetMenu(fileName = "CrazyMonkeyConfigSO", menuName = "Minigames/CrazyMonkeyConfig")]
    public class CrazyMonkeyConfigSO : ScriptableObject
    {
        [field: Header("Gameplay")]
        [field: SerializeField] public float GameDuration { get; private set; } = 10f;
        [field: SerializeField] public float DrainSpeed { get; private set; } = 0.20f;
        [field: SerializeField] public float ClickRestore { get; private set; } = 0.05f;
        [field: SerializeField, Range(0f, 1f)] public float StartFill { get; private set; } = 0.5f;

        [field: Header("Tickets")]
        [field: SerializeField] public int MinTickets { get; private set; } = 1;
        [field: SerializeField] public int MaxTickets { get; private set; } = 20;

        [field: Header("Countdown")]
        [field: SerializeField, Min(1)] public int CountdownFrom { get; private set; } = 3;
        [field: SerializeField] public float CountdownStep { get; private set; } = 1f;
        [field: SerializeField] public float GoDuration { get; private set; } = 0.5f;

        [field: Header("Result")]
        [field: SerializeField] public float ResultDelay { get; private set; } = 1.5f;
    }
}