using UnityEngine;

namespace Machines.Minigames.PumpItUp
{
    public class SongClock : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        private double _startDspTime;
        private float _pausedTime;
        private bool _isPlaying;
        private bool _isPaused;

        public float Time
        {
            get
            {
                if (!_isPlaying)
                    return 0f;

                if (_isPaused)
                    return _pausedTime;

                return (float)(AudioSettings.dspTime - _startDspTime);
            }
        }

        public float Length => source.clip != null ? source.clip.length : 0f;
        public bool IsFinished => _isPlaying && Time >= Length;

        private void Update()
        {
            if (!_isPlaying)
                return;

            if (UnityEngine.Time.timeScale == 0f)
                Pause();
            else
                Resume();
        }

        public void Play(AudioClip clip)
        {
            source.clip = clip;
            source.Play();

            _startDspTime = AudioSettings.dspTime;
            _isPlaying = true;
            _isPaused = false;
        }

        public void Stop()
        {
            source.Stop();
            _isPlaying = false;
            _isPaused = false;
        }

        private void Pause()
        {
            if (_isPaused)
                return;

            _pausedTime = Time;
            source.Pause();
            _isPaused = true;
        }

        private void Resume()
        {
            if (!_isPaused)
                return;

            _startDspTime = AudioSettings.dspTime - _pausedTime;
            source.UnPause();
            _isPaused = false;
        }
    }
}