using Core;
using UnityEngine;
using UnityEngine.Audio;

namespace UI
{
    public class OptionsSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

        [Header("Sliders")]
        [SerializeField] private AsciiSlider sensitivitySlider;
        [SerializeField] private AsciiSlider masterSlider;
        [SerializeField] private AsciiSlider sfxSlider;
        [SerializeField] private AsciiSlider musicSlider;

        private void Start()
        {
            SetupSlider(sensitivitySlider, GameSettings.SensitivityKey);
            SetupSlider(masterSlider, GameSettings.MasterVolumeKey);
            SetupSlider(sfxSlider, GameSettings.SfxVolumeKey);
            SetupSlider(musicSlider, GameSettings.MusicVolumeKey);

            ApplyVolume(GameSettings.MasterVolumeKey);
            ApplyVolume(GameSettings.SfxVolumeKey);
            ApplyVolume(GameSettings.MusicVolumeKey);
        }

        private void SetupSlider(AsciiSlider asciiSlider, string key)
        {
            asciiSlider.Slider.SetValueWithoutNotify(GameSettings.GetStep(key));
            asciiSlider.Redraw();
            asciiSlider.Slider.onValueChanged.AddListener(value => OnSliderChanged(key, value));
        }

        private void OnSliderChanged(string key, float value)
        {
            GameSettings.SetStep(key, Mathf.RoundToInt(value));

            if (key != GameSettings.SensitivityKey)
                ApplyVolume(key);
        }

        private void ApplyVolume(string key)
        {
            float normalized = GameSettings.GetNormalized(key);
            float dB = normalized > 0.001f ? Mathf.Log10(normalized) * 20f : -80f;
            audioMixer.SetFloat(key, dB);
        }
    }
}