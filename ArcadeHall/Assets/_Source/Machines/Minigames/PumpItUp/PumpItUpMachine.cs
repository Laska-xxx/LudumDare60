using DG.Tweening;
using Machines;
using Machines.Minigames.PumpItUp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PumpItUpMachine : ArcadeMachine
{
    [Header("Pump It Up")]
    [SerializeField] private PumpItUpConfigSO config;
    [SerializeField] private NoteBoard board;
    [SerializeField] private NoteHitter hitter;
    [SerializeField] private SongClock clock;
    [SerializeField] private Camera screenCamera;

    [Header("Screen")]
    [SerializeField] private JudgementView judgementView;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private string readyText = "READY?";
    [SerializeField] private string goText = "GO!";

    [Header("Sounds")]
    [SerializeField] private AudioClip endSound;

    private ScoreCounter _score;
    private bool _isRunning;

    private void Start()
    {
        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1f;
        progressSlider.interactable = false;
        progressSlider.value = 0f;

        countdownText.text = string.Empty;
        judgementView.Clear();

        screenCamera.enabled = false;
        screenCamera.Render();
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
        hitter.Judged -= OnJudged;
    }

    protected override void StartGame()
    {
        _score = new ScoreCounter(config);

        judgementView.Clear();
        progressSlider.value = 0f;
        screenCamera.enabled = true;

        board.Begin(NoteChartGenerator.Generate(config, board.LaneCount), config.ApproachTime, config.GoodWindow);

        hitter.Judged += OnJudged;
        hitter.Begin(config);

        clock.Play(config.Music);
        _isRunning = true;
    }

    private void Update()
    {
        if (!_isRunning)
            return;

        float time = clock.Time;

        board.Tick(time);
        UpdateCountdown(time);
        progressSlider.value = Mathf.Clamp01(time / clock.Length);

        if (clock.IsFinished)
            EndGame();
    }

    private void UpdateCountdown(float time)
    {
        int beat = Mathf.FloorToInt((time - config.Offset) / config.SecondsPerBeat);
        int beatsLeft = config.CountdownBeats - beat;

        string text;

        if (beatsLeft > 4)
            text = readyText;
        else if (beatsLeft > 1)
            text = (beatsLeft - 1).ToString();
        else if (beatsLeft == 1)
            text = goText;
        else
            text = string.Empty;

        if (countdownText.text != text)
            countdownText.text = text;
    }

    private void OnJudged(Judgement judgement)
    {
        _score.Register(judgement);
        judgementView.Show(judgement, _score.Accuracy, config.JudgementShowTime);
    }

    private void EndGame()
    {
        _isRunning = false;

        hitter.Judged -= OnJudged;
        hitter.End();
        clock.Stop();
        board.Clear();

        int tickets = config.GetTickets(_score.Accuracy);
        countdownText.text = $"+{tickets}";
        PlaySound(endSound);

        DOVirtual.DelayedCall(config.ResultDelay, () =>
        {
            countdownText.text = string.Empty;
            judgementView.Clear();
            progressSlider.value = 0f;

            screenCamera.Render();
            screenCamera.enabled = false;

            FinishGame(tickets);
        }, false)
            .SetId(this);
    }
}