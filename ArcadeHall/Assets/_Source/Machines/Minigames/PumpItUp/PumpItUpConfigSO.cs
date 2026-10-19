using System;
using System.Collections.Generic;
using UnityEngine;

namespace Machines.Minigames.PumpItUp
{
    [CreateAssetMenu(fileName = "PumpItUpConfigSO", menuName = "Minigames/PumpItUpConfig")]
    public class PumpItUpConfigSO : ScriptableObject
    {
        public const int BeatsPerBar = 4;

        [field: Header("Music")]
        [field: SerializeField] public AudioClip Music { get; private set; }
        [field: SerializeField] public float Bpm { get; private set; } = 160f;
        [field: SerializeField] public float Offset { get; private set; } = 0f;
        [field: SerializeField] public int CountdownBars { get; private set; } = 2;

        [field: Header("Notes")]
        [field: SerializeField] public float ApproachTime { get; private set; } = 1.2f;
        [field: SerializeField] public int Seed { get; private set; } = 0;
        [field: SerializeField] public NoteWave[] Waves { get; private set; }

        [field: Header("Judgement")]
        [field: SerializeField] public float PerfectWindow { get; private set; } = 0.05f;
        [field: SerializeField] public float GreatWindow { get; private set; } = 0.10f;
        [field: SerializeField] public float GoodWindow { get; private set; } = 0.15f;
        [field: SerializeField, Range(0f, 1f)] public float GreatWeight { get; private set; } = 0.7f;
        [field: SerializeField, Range(0f, 1f)] public float GoodWeight { get; private set; } = 0.4f;
        [field: SerializeField] public float JudgementShowTime { get; private set; } = 0.4f;

        [field: Header("Reward")]
        [field: SerializeField, Range(0f, 1f)] public float GoodAccuracy { get; private set; } = 0.7f;
        [field: SerializeField] public int GoodTickets { get; private set; } = 20;
        [field: SerializeField, Range(0f, 1f)] public float PerfectAccuracy { get; private set; } = 0.95f;
        [field: SerializeField] public int PerfectTickets { get; private set; } = 50;
        [field: SerializeField] public float ResultDelay { get; private set; } = 1.5f;

        public float SecondsPerBeat => 60f / Bpm;
        public int CountdownBeats => CountdownBars * BeatsPerBar;

        public float BeatToTime(float beat)
        {
            return Offset + beat * SecondsPerBeat;
        }

        public int GetTickets(float accuracy)
        {
            float tickets = accuracy <= GoodAccuracy
                ? Mathf.Lerp(0f, GoodTickets, accuracy / GoodAccuracy)
                : Mathf.Lerp(GoodTickets, PerfectTickets, Mathf.InverseLerp(GoodAccuracy, PerfectAccuracy, accuracy));

            return Mathf.RoundToInt(tickets);
        }

        public Judgement GetJudgement(float offset)
        {
            if (offset <= PerfectWindow)
                return Judgement.Perfect;

            if (offset <= GreatWindow)
                return Judgement.Great;

            return offset <= GoodWindow ? Judgement.Good : Judgement.Miss;
        }

        public float GetWeight(Judgement judgement)
        {
            return judgement switch
            {
                Judgement.Perfect => 1f,
                Judgement.Great => GreatWeight,
                Judgement.Good => GoodWeight,
                _ => 0f
            };
        }

        [ContextMenu("Log Chart")]
        private void LogChart()
        {
            List<NoteData> notes = NoteChartGenerator.Generate(this, 4);
            float lastTime = notes.Count > 0 ? notes[^1].Time : 0f;
            Debug.Log($"Notes: {notes.Count}, last note at {lastTime:0.00}s");
        }
    }

    [Serializable]
    public class NoteWave
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int Bars { get; private set; } = 4;
        [field: SerializeField] public float NoteStep { get; private set; } = 1f;
        [field: SerializeField, Range(0f, 1f)] public float NoteChance { get; private set; } = 0.6f;
        [field: SerializeField, Range(0f, 1f)] public float[] ExtraNoteChances { get; private set; }
    }
}
