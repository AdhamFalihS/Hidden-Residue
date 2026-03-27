using UnityEngine;
using System.Collections.Generic;

namespace HiddenResidue.Evidence
{
    public class EvidenceManager : MonoBehaviour
    {
        public static EvidenceManager Instance { get; private set; }

        private List<EvidenceData> foundEvidence = new List<EvidenceData>();

        public IReadOnlyList<EvidenceData> FoundEvidence => foundEvidence;
        public int Count => foundEvidence.Count;

        public static event System.Action<EvidenceData> OnEvidenceAdded;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Debug.Log("[EvidenceManager] Initialized");
        }
        private void Start()
        {
            Debug.Log($"[EvidenceManager] Started. Total evidence: {foundEvidence.Count}");
        }
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
                return;
            }
            foundEvidence.Add(data);
            Debug.Log($"[EvidenceManager] Added: {data.evidenceName} | Total: {foundEvidence.Count}");
            OnEvidenceAdded?.Invoke(data);
            Debug.Log($"[EvidenceManager] OnEvidenceAdded invoked. Listeners: {(OnEvidenceAdded != null ? "Yes" : "No")}");
        }
        public bool HasEvidence(EvidenceData data) => foundEvidence.Contains(data);

        public bool HasEvidence(string id) => foundEvidence.Exists(e => e.evidenceID == id);

        public void ClearAll()
        {
            foundEvidence.Clear();
            Debug.Log("[EvidenceManager] All evidence cleared");
        }

    }

}