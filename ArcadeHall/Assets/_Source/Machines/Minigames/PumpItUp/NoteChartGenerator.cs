using System.Collections.Generic;

namespace Machines.Minigames.PumpItUp
{
    public static class NoteChartGenerator
    {
        public static List<NoteData> Generate(PumpItUpConfigSO config, int laneCount)
        {
            var notes = new List<NoteData>();
            var random = config.Seed == 0 ? new System.Random() : new System.Random(config.Seed);
            var lanes = new List<int>(laneCount);

            float trackLength = config.Music != null ? config.Music.length : float.MaxValue;
            float waveStartBeat = config.CountdownBeats;

            foreach (NoteWave wave in config.Waves)
            {
                float waveBeats = wave.Bars * PumpItUpConfigSO.BeatsPerBar;
                int steps = (int)(waveBeats / wave.NoteStep);

                for (int i = 0; i < steps; i++)
                {
                    float time = config.BeatToTime(waveStartBeat + i * wave.NoteStep);

                    if (time > trackLength)
                        return notes;

                    if (random.NextDouble() >= wave.NoteChance)
                        continue;

                    int count = 1;

                    foreach (float chance in wave.ExtraNoteChances)
                    {
                        if (random.NextDouble() < chance)
                            count++;
                    }

                    FillShuffledLanes(lanes, laneCount, random);

                    for (int n = 0; n < count && n < laneCount; n++)
                        notes.Add(new NoteData(time, lanes[n]));
                }

                waveStartBeat += waveBeats;
            }

            return notes;
        }

        private static void FillShuffledLanes(List<int> lanes, int laneCount, System.Random random)
        {
            lanes.Clear();

            for (int i = 0; i < laneCount; i++)
                lanes.Add(i);

            for (int i = laneCount - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (lanes[i], lanes[j]) = (lanes[j], lanes[i]);
            }
        }
    }
}