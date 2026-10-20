using UnityEngine;

namespace Audio
{
    public class UISounds : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip selectSound;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip sliderSound;

        [Header("Slider")]
        [SerializeField] private float minSliderPitch = 0.8f;
        [SerializeField] private float maxSliderPitch = 1.4f;
        [SerializeField] private float sliderCooldown = 0.04f;

        private static UISounds _instance;

        private AudioSource _sliderSource;
        private float _nextSliderTime;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            source.ignoreListenerPause = true;

            _sliderSource = gameObject.AddComponent<AudioSource>();
            _sliderSource.playOnAwake = false;
            _sliderSource.ignoreListenerPause = true;
            _sliderSource.outputAudioMixerGroup = source.outputAudioMixerGroup;
            _sliderSource.volume = source.volume;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public static void PlaySelect()
        {
            if (_instance != null)
                _instance.Play(_instance.selectSound);
        }

        public static void PlayClick()
        {
            if (_instance != null)
                _instance.Play(_instance.clickSound);
        }

        public static void PlaySlider(float normalizedValue)
        {
            if (_instance != null)
                _instance.PlaySliderStep(normalizedValue);
        }

        private void Play(AudioClip clip)
        {
            if (clip != null)
                source.PlayOneShot(clip);
        }

        private void PlaySliderStep(float normalizedValue)
        {
            if (sliderSound == null || Time.unscaledTime < _nextSliderTime)
                return;

            _nextSliderTime = Time.unscaledTime + sliderCooldown;
            _sliderSource.pitch = Mathf.Lerp(minSliderPitch, maxSliderPitch, normalizedValue);
            _sliderSource.PlayOneShot(sliderSound);
        }
    }
}
