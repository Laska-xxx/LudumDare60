using Core;
using Machines.Minigames.PumpItUp;
using System.Collections;
using UnityEngine;
using Zenject;

public class PumpItUpTest : MonoBehaviour
{
    [SerializeField] private PumpItUpConfigSO config;
    [SerializeField] private NoteBoard board;
    [SerializeField] private SongClock clock;
    [SerializeField] private NoteHitter hitter;
    [SerializeField] private JudgementView judgementView;

    private InputReader _inputReader;
    private ScoreCounter _score;
    private bool _isRunning;

    [Inject]
    private void Init(InputReader inputReader)
    {
        _inputReader = inputReader;
    }

    private IEnumerator Start()
    {
        yield return null;

        _inputReader.SwitchActionMapType(ActionMapType.Minigame);

        _score = new ScoreCounter(config);
        judgementView.Clear();

        hitter.Judged += judgement =>
        {
            _score.Register(judgement);
            judgementView.Show(judgement, _score.Accuracy, config.JudgementShowTime);
        };

        board.Begin(NoteChartGenerator.Generate(config, board.LaneCount), config.ApproachTime, config.GoodWindow);
        hitter.Begin(config);
        clock.Play(config.Music);

        _isRunning = true;
    }

    private void Update()
    {
        if (_isRunning)
            board.Tick(clock.Time);
    }
}