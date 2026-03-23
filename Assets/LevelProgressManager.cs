using UnityEngine;

namespace HiddenResidue.Core
{
    /// <summary>
    /// LevelProgressManager — Menyimpan level mana saja yang sudah di-unlock.
    /// Menggunakan PlayerPrefs sehingga data tersimpan antar sesi.
    ///
    /// Cara pakai:
    ///   - LevelProgressManager.Instance.IsLevelUnlocked(buildIndex) → bool
    ///   - LevelProgressManager.Instance.UnlockLevel(buildIndex)
    ///   - LevelProgressManager.Instance.ResetProgress() — untuk debug
    ///
    /// Level 1 (build index 1) selalu unlocked secara default.
    /// Main Menu diasumsikan build index 0.
    /// </summary>
    public class LevelProgressManager : MonoBehaviour
    {
        public static LevelProgressManager Instance { get; private set; }

        private const string KeyPrefix     = "LevelUnlocked_";
        private const int    FirstLevelIdx = 1; // Build index scene level pertama

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Level pertama selalu terbuka
            if (!IsLevelUnlocked(FirstLevelIdx))
                UnlockLevel(FirstLevelIdx);
        }

        /// <summary>Cek apakah level (berdasarkan build index) sudah di-unlock.</summary>
        public bool IsLevelUnlocked(int buildIndex)
        {
            return PlayerPrefs.GetInt(KeyPrefix + buildIndex, 0) == 1;
        }

        /// <summary>Unlock level berdasarkan build index.</summary>
        public void UnlockLevel(int buildIndex)
        {
            PlayerPrefs.SetInt(KeyPrefix + buildIndex, 1);
            PlayerPrefs.Save();
            Debug.Log($"[LevelProgressManager] Level {buildIndex} di-unlock.");
        }

        /// <summary>Reset semua progress (untuk debug / new game).</summary>
        public void ResetProgress()
        {
            // Hapus semua key level
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
                PlayerPrefs.DeleteKey(KeyPrefix + i);

            // Level pertama selalu terbuka
            UnlockLevel(FirstLevelIdx);
            PlayerPrefs.Save();
            Debug.Log("[LevelProgressManager] Progress direset.");
        }
    }
}
