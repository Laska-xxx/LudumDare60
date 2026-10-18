using System;
using UnityEngine;

namespace Core
{
    public static class GameSettings
    {
        public const string SensitivityKey = "Sensitivity";
        public const string MasterVolumeKey = "MasterVolume";
        public const string SfxVolumeKey = "SFXVolume";
        public const string MusicVolumeKey = "MusicVolume";

        public const int MaxStep = 10;

        private const int DefaultSensitivityStep = 5;
        private const int DefaultVolumeStep = 5;

        public static event Action SensitivityChanged;

        public static int GetStep(string key)
        {
            int defaultStep = key == SensitivityKey ? DefaultSensitivityStep : DefaultVolumeStep;
            return PlayerPrefs.GetInt(key, defaultStep);
        }

        public static float GetNormalized(string key)
        {
            return GetStep(key) / (float)MaxStep;
        }

        public static void SetStep(string key, int step)
        {
            PlayerPrefs.SetInt(key, step);
            PlayerPrefs.Save();

            if (key == SensitivityKey)
                SensitivityChanged?.Invoke();
        }
    }
}