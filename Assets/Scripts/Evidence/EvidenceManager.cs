using UnityEngine;
using System.Collections.Generic;

namespace HiddenResidue.Evidence
{
    /// <summary>
    /// EvidenceManager — Singleton yang menyimpan daftar semua barang bukti
    /// yang sudah ditemukan pemain di session ini.
    /// </summary>
    public class EvidenceManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────
        public static EvidenceManager Instance { get; private set; }

        // ─── State ───────────────────────────────────────────────────────────
        private List<EvidenceData> foundEvidence = new List<EvidenceData>();

        public IReadOnlyList<EvidenceData> FoundEvidence => foundEvidence;
        public int Count => foundEvidence.Count;

        // ─── Events ──────────────────────────────────────────────────────────
        public static event System.Action<EvidenceData> OnEvidenceAdded;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public void AddEvidence(EvidenceData data)
        {
            if (data == null) return;
            if (foundEvidence.Contains(data)) return;  // Hindari duplikat

            foundEvidence.Add(data);
            OnEvidenceAdded?.Invoke(data);
            Debug.Log($"[EvidenceManager] Added: {data.evidenceName} | Total: {foundEvidence.Count}");
        }

        public bool HasEvidence(EvidenceData data) => foundEvidence.Contains(data);
        public bool HasEvidence(string id)         => foundEvidence.Exists(e => e.evidenceID == id);

        public void ClearAll() => foundEvidence.Clear();
    }
}
