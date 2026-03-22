using UnityEngine;
using System.Collections.Generic;
using HiddenResidue.UI;

namespace HiddenResidue.Core
{
    /// <summary>
    /// LevelManager — Melacak progres level: objek yang sudah dibersihkan
    /// dan barang bukti yang sudah ditemukan. Saat semua selesai → Level Complete.
    /// Attach ke GameObject "LevelManager" di setiap scene level.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // ─── Singleton (per-scene) ────────────────────────────────────────────
        public static LevelManager Instance { get; private set; }

        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Level Info")]
        [SerializeField] private string levelName = "Level 1 - Coffee Shop";

        [Header("Tasks")]
        [Tooltip("Jumlah objek yang perlu dibersihkan di level ini")]
        [SerializeField] private int totalCleanableObjects = 0;
        [Tooltip("Jumlah barang bukti yang tersembunyi di level ini")]
        [SerializeField] private int totalEvidenceItems    = 0;

        [Header("Settings")]
        [Tooltip("Apakah semua evidence harus ditemukan untuk complete?")]
        [SerializeField] private bool requireAllEvidence = false;

        // ─── State ───────────────────────────────────────────────────────────
        private int cleanedCount  = 0;
        private int evidenceCount = 0;

        public int CleanedCount   => cleanedCount;
        public int EvidenceCount  => evidenceCount;
        public int TotalCleanable => totalCleanableObjects;
        public int TotalEvidence  => totalEvidenceItems;

        // ─── Events ──────────────────────────────────────────────────────────
        public static event System.Action OnLevelProgressUpdated;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            if (totalCleanableObjects == 0)
                totalCleanableObjects = FindObjectsByType<Interaction.CleanableObject>(FindObjectsSortMode.None).Length;
            if (totalEvidenceItems == 0)
                totalEvidenceItems = FindObjectsByType<Evidence.EvidenceObject>(FindObjectsSortMode.None).Length;

            Debug.Log($"[LevelManager] {levelName} | Clean: {totalCleanableObjects} | Evidence: {totalEvidenceItems}");
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public void RegisterCleaned()
        {
            cleanedCount = Mathf.Min(cleanedCount + 1, totalCleanableObjects);
            OnLevelProgressUpdated?.Invoke();
            CheckLevelComplete();
        }

        public void RegisterEvidenceFound()
        {
            evidenceCount = Mathf.Min(evidenceCount + 1, totalEvidenceItems);
            OnLevelProgressUpdated?.Invoke();
            CheckLevelComplete();
        }

        public float GetCleanProgress()    => totalCleanableObjects > 0 ? (float)cleanedCount / totalCleanableObjects : 1f;
        public float GetEvidenceProgress() => totalEvidenceItems > 0 ? (float)evidenceCount / totalEvidenceItems : 1f;

        // ─── Private ─────────────────────────────────────────────────────────
        private void CheckLevelComplete()
        {
            bool cleanDone    = cleanedCount >= totalCleanableObjects;
            bool evidenceDone = !requireAllEvidence || evidenceCount >= totalEvidenceItems;

            if (cleanDone && evidenceDone)
            {
                Debug.Log($"[LevelManager] Level Complete: {levelName}");
                ScoreManager.Instance?.AddLevelBonus();

                // ── FIX: Hapus LevelCompleteUI.Instance?.Show() dari sini ────
                // Cukup panggil TriggerLevelComplete() — GameManager yang handle Show()
                // Memanggil Show() dua kali menyebabkan Instance null saat scene pertama load
                GameManager.Instance?.TriggerLevelComplete();
            }
        }
    }
}