using UnityEngine;

namespace HiddenResidue.Core
{
    public class LevelProgressManager : MonoBehaviour
    {
        public static LevelProgressManager Instance { get; private set; }

        private const string KeyPrefix = "LevelUnlocked_";
        private const int FirstLevelIdx = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!IsLevelUnlocked(FirstLevelIdx))
                UnlockLevel(FirstLevelIdx);
        }

        public bool IsLevelUnlocked(int buildIndex)
        {
            return PlayerPrefs.GetInt(KeyPrefix + buildIndex, 0) == 1;
        }

        public void UnlockLevel(int buildIndex)
        {
            PlayerPrefs.SetInt(KeyPrefix + buildIndex, 1);
            PlayerPrefs.Save();
            Debug.Log($"[LevelProgressManager] Level {buildIndex} di-unlock.");
        }

        public void ResetProgress()
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
                PlayerPrefs.DeleteKey(KeyPrefix + i);

            UnlockLevel(FirstLevelIdx);
            PlayerPrefs.Save();
            Debug.Log("[LevelProgressManager] Progress direset.");
        }
    }
}
