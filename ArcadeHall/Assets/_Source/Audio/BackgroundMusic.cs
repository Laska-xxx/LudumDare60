using DG.Tweening;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusic : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.5f;

        private static BackgroundMusic _instance;

        private AudioSource _source;
        private float _volume;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _source = GetComponent<AudioSource>();
            _source.loop = true;
            _source.ignoreListenerPause = true;
            _volume = _source.volume;

            if (!_source.isPlaying)
                _source.Play();
        }

        private void OnDestroy()
        {
            if (_instance != this)
                return;

            DOTween.Kill(this);
            _instance = null;
        }

        public static void SetMuted(bool isMuted)
        {
            if (_instance == null)
                return;

            _instance.FadeTo(isMuted ? 0f : _instance._volume);
        }

        private void FadeTo(float volume)
        {
            DOTween.Kill(this);

            DOTween.To(() => _source.volume, value => _source.volume = value, volume, fadeDuration)
                .SetUpdate(true)
                .SetId(this);
        }
    }
}
