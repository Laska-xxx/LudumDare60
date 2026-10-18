using UnityEngine;

namespace Core
{
    public static class BestScore
    {
        private const string BestScoreKey = "BestScore";

        public static int Value
        {
            get
            {
                return PlayerPrefs.GetInt(BestScoreKey, 0);
            }
        }

        public static bool TrySave(int score)
        {
            if (score <= Value)
                return false;

            PlayerPrefs.SetInt(BestScoreKey, score);
            PlayerPrefs.Save();
            return true;
        }
    }
}