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
            // Cek instance
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
                return; 
            }
            
            Instance = this;
            
            // Hanya DontDestroyOnLoad untuk persistent manager
            // Jangan DontDestroyOnLoad dulu untuk testing
            // DontDestroyOnLoad(gameObject);
            
            Debug.Log("[EvidenceManager] Initialized");
        }

        private void Start()
        {
            Debug.Log($"[EvidenceManager] Started. Total evidence: {foundEvidence.Count}");
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public void AddEvidence(EvidenceData data)
        {
            if (data == null) 
            {
                Debug.LogError("[EvidenceManager] Cannot add null evidence!");
                return;
            }
            
            if (foundEvidence.Contains(data)) 
            {
                Debug.Log($"[EvidenceManager] Evidence {data.evidenceName} already exists!");
                return;  // Hindari duplikat
            }

            foundEvidence.Add(data);
            Debug.Log($"[EvidenceManager] Added: {data.evidenceName} | Total: {foundEvidence.Count}");
            
            // Broadcast event
            OnEvidenceAdded?.Invoke(data);
            
            // Tambahkan log untuk cek listener
            Debug.Log($"[EvidenceManager] OnEvidenceAdded invoked. Listeners: {(OnEvidenceAdded != null ? "Yes" : "No")}");
        }

        public bool HasEvidence(EvidenceData data) => foundEvidence.Contains(data);
        public bool HasEvidence(string id)         => foundEvidence.Exists(e => e.evidenceID == id);

        public void ClearAll() 
        {
            foundEvidence.Clear();
            Debug.Log("[EvidenceManager] All evidence cleared");
        }
    }
}