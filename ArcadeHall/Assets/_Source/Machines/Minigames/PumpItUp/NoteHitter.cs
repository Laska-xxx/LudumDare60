using Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Machines.Minigames.PumpItUp
{
    public class NoteHitter : MonoBehaviour
    {
        [SerializeField] private NoteBoard board;
        [SerializeField] private SongClock clock;
        [SerializeField] private LaneGlow[] laneGlows;

        public event Action<Judgement> Judged;

        private InputReader _inputReader;
        private PumpItUpConfigSO _config;
        private InputAction[] _laneActions;

        [Inject]
        private void Init(InputReader inputReader)
        {
            _inputReader = inputReader;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void Begin(PumpItUpConfigSO config)
        {
            _config = config;

            var minigame = _inputReader.GameInput.Minigame;
            _laneActions = new[] { minigame.Left, minigame.CenterLeft, minigame.CenterRight, minigame.Right };

            foreach (InputAction action in _laneActions)
                action.performed += OnLanePressed;

            board.NoteMissed += OnNoteMissed;
        }

        public void End()
        {
            Unsubscribe();
        }

        private void OnLanePressed(InputAction.CallbackContext ctx)
        {
            int lane = Array.IndexOf(_laneActions, ctx.action);

            if (lane < 0)
                return;

            laneGlows[lane].Flash();

            NoteView note = board.GetFirstNote(lane);

            if (note == null)
                return;

            float offset = Mathf.Abs(note.Data.Time - clock.Time);

            if (offset > _config.GoodWindow)
                return;

            board.RemoveNote(note);
            Judged?.Invoke(_config.GetJudgement(offset));
        }

        private void OnNoteMissed(int lane)
        {
            Judged?.Invoke(Judgement.Miss);
        }

        private void Unsubscribe()
        {
            if (_laneActions == null)
                return;

            foreach (InputAction action in _laneActions)
                action.performed -= OnLanePressed;

            board.NoteMissed -= OnNoteMissed;
            _laneActions = null;
        }
    }
}
