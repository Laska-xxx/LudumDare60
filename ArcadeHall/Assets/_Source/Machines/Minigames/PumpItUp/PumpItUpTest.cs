using Machines.Minigames.PumpItUp;
using System.Collections;
using UnityEngine;

public class PumpItUpTest : MonoBehaviour
{
    [SerializeField] private PumpItUpConfigSO config;
    [SerializeField] private NoteBoard board;
    [SerializeField] private SongClock clock;

    private bool _isRunning;

    private IEnumerator Start()
    {
        yield return null;

        board.NoteMissed += lane => Debug.Log($"MISS lane {lane}");
        board.Begin(NoteChartGenerator.Generate(config, board.LaneCount), config.ApproachTime, config.GoodWindow);
        clock.Play(config.Music);

        _isRunning = true;
    }

    private void Update()
    {
        if (_isRunning)
            board.Tick(clock.Time);
    }
}