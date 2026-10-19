namespace Machines.Minigames.PumpItUp
{
    public class ScoreCounter
    {
        private readonly PumpItUpConfigSO _config;
        private float _points;
        private int _notes;

        public float Accuracy => _notes == 0 ? 1f : _points / _notes;

        public ScoreCounter(PumpItUpConfigSO config)
        {
            _config = config;
        }

        public void Register(Judgement judgement)
        {
            _notes++;
            _points += _config.GetWeight(judgement);
        }
    }
}
