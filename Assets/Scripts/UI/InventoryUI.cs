using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

namespace HiddenResidue.UI
{
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }

        [Header("Panel Root")]
        [SerializeField] private GameObject inventoryPanel;

        [Header("Grid / List")]
        [SerializeField] private Transform gridParent;
        [SerializeField] private GameObject evidenceSlotPrefab;

        [Header("Detail Popup")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private Image detailIcon;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDesc;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        // ─────────────────────────────────────────────
        private void Awake()
        {
            // Singleton setup
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initial state
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
                Debug.Log("[InventoryUI] Panel initialized - OFF");
            }
            else
            {
                Debug.LogError("[InventoryUI] Inventory Panel TIDAK diassign!");
            }

            if (detailPanel != null)
                detailPanel.SetActive(false);

            // Cek komponen penting
            if (gridParent == null)
                Debug.LogError("[InventoryUI] Grid Parent TIDAK diassign!");
                
            if (evidenceSlotPrefab == null)
                Debug.LogError("[InventoryUI] Evidence Slot Prefab TIDAK diassign!");
        }

        private void OnEnable()
        {
            // Subscribe ke event
            Evidence.EvidenceManager.OnEvidenceAdded += HandleEvidenceAdded;
            Debug.Log("[InventoryUI] Subscribed to OnEvidenceAdded");
        }

        private void OnDisable()
        {
            // Unsubscribe dari event
            Evidence.EvidenceManager.OnEvidenceAdded -= HandleEvidenceAdded;
            Debug.Log("[InventoryUI] Unsubscribed from OnEvidenceAdded");
        }

        private void Update()
        {
            // CEK TOMBOL I dengan New Input System
            if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
            {
                Debug.Log("========== Tombol I ditekan ==========");
                ToggleInventory();
            }
        }

        // ─────────────────────────────────────────────
        private void HandleEvidenceAdded(Evidence.EvidenceData data)
        {
            Debug.Log($"========== HandleEvidenceAdded Dipanggil ==========");
            Debug.Log($"[InventoryUI] Evidence ditambahkan: {data?.evidenceName ?? "NULL"}");
            
            // Cek apakah inventory sedang terbuka
            bool isPanelActive = inventoryPanel != null && inventoryPanel.activeSelf;
            Debug.Log($"[InventoryUI] Panel active: {isPanelActive}");
            
            if (isPanelActive)
            {
                // Refresh grid
                RefreshGrid();
            }
            else
            {
                Debug.Log("[InventoryUI] Panel tidak aktif, skip refresh");
            }
        }

        // ─────────────────────────────────────────────
        public void ToggleInventory()
        {
            if (inventoryPanel == null)
            {
                Debug.LogError("[InventoryUI] Inventory Panel TIDAK diassign!");
                return;
            }

            bool isOpen = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(isOpen);
            
            Debug.Log($"[InventoryUI] Inventory {(isOpen ? "DIBUKA" : "DITUTUP")}");

            if (isOpen)
            {
                // Refresh grid setiap kali dibuka
                StartCoroutine(DelayedRefresh());
                
                // Tutup detail panel jika ada
                if (detailPanel != null)
                    detailPanel.SetActive(false);
                    
                // Pause game jika perlu
                if (Core.GameManager.Instance != null)
                    Core.GameManager.Instance.PauseGame();
            }
            else
            {
                // Resume game jika perlu
                if (Core.GameManager.Instance != null)
                    Core.GameManager.Instance.ResumeGame();
            }
        }

        // Coroutine untuk delay refresh (pastikan UI sudah aktif)
        private IEnumerator DelayedRefresh()
        {
            yield return new WaitForEndOfFrame();
            RefreshGrid();
        }

        // ─────────────────────────────────────────────
        private void RefreshGrid()
        {
            Debug.Log("========== Refresh Grid Dimulai ==========");
            
            // Validasi komponen
            if (gridParent == null)
            {
                Debug.LogError("[InventoryUI] Grid Parent NULL!");
                return;
            }
            
            if (evidenceSlotPrefab == null)
            {
                Debug.LogError("[InventoryUI] Evidence Slot Prefab NULL!");
                return;
            }

            // Cek EvidenceManager
            var evidenceManager = Evidence.EvidenceManager.Instance;
            if (evidenceManager == null)
            {
                Debug.LogError("[InventoryUI] EvidenceManager.Instance NULL!");
                return;
            }

            // Ambil daftar evidence
            var evidenceList = evidenceManager.FoundEvidence;
            if (evidenceList == null)
            {
                Debug.LogError("[InventoryUI] FoundEvidence list NULL!");
                return;
            }

            Debug.Log($"[InventoryUI] Jumlah evidence di manager: {evidenceList.Count}");

            // Hapus semua child yang ada
            int childCount = gridParent.childCount;
            Debug.Log($"[InventoryUI] Menghapus {childCount} child lama");
            
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }

            // Buat slot untuk setiap evidence
            if (evidenceList.Count == 0)
            {
                Debug.Log("[InventoryUI] Tidak ada evidence untuk ditampilkan");
                
                // Optional: Tampilkan pesan "Kosong"
                // GameObject emptyMsg = Instantiate(emptySlotPrefab, gridParent);
                // emptyMsg.name = "EmptyMessage";
            }
            else
            {
                foreach (var evidence in evidenceList)
                {
                    Debug.Log($"[InventoryUI] Membuat slot untuk: {evidence.evidenceName}");
                    CreateEvidenceSlot(evidence);
                }
            }
            
            // Force layout rebuild
            if (gridParent.GetComponent<RectTransform>() != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gridParent.GetComponent<RectTransform>());
                Debug.Log("[InventoryUI] Layout rebuilt");
            }
            
            Debug.Log("========== Refresh Grid Selesai ==========");
        }

        private void CreateEvidenceSlot(Evidence.EvidenceData evidence)
        {
            if (evidence == null)
            {
                Debug.LogError("[InventoryUI] Evidence data NULL!");
                return;
            }

            // Instantiate slot
            GameObject slot = Instantiate(evidenceSlotPrefab, gridParent);
            
            // Set nama object
            slot.name = $"Slot_{evidence.evidenceName}";
            
            // Setup UI
            SetupSlotUI(slot, evidence);
            
            Debug.Log($"[InventoryUI] Slot created: {slot.name}");
        }

        private void SetupSlotUI(GameObject slot, Evidence.EvidenceData evidence)
        {
            // Setup Image (icon)
            Image[] images = slot.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                // Skip jika ini adalah background slot itu sendiri
                if (img.gameObject == slot) continue;
                
                if (evidence.icon != null)
                {
                    img.sprite = evidence.icon;
                    Debug.Log($"[InventoryUI] Icon set untuk {evidence.evidenceName}");
                }
                else
                {
                    Debug.LogWarning($"[InventoryUI] Icon untuk {evidence.evidenceName} null");
                }
                break;
            }

            // Setup Text
            TextMeshProUGUI tmp = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = evidence.evidenceName;
                Debug.Log($"[InventoryUI] Text set: {evidence.evidenceName}");
            }
            else
            {
                Debug.LogWarning("[InventoryUI] TextMeshProUGUI tidak ditemukan di prefab");
            }

            // Setup Button
            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => ShowDetail(evidence));
                Debug.Log($"[InventoryUI] Button listener added");
            }
            else
            {
                Debug.LogWarning("[InventoryUI] Button tidak ditemukan di prefab");
            }
        }

        private void ShowDetail(Evidence.EvidenceData evidence)
        {
            Debug.Log($"[InventoryUI] Show detail: {evidence.evidenceName}");
            
            if (detailPanel == null)
            {
                Debug.LogWarning("[InventoryUI] Detail Panel tidak diassign!");
                return;
            }

            detailPanel.SetActive(true);

            // Set icon
            if (detailIcon != null)
            {
                if (evidence.icon != null)
                    detailIcon.sprite = evidence.icon;
                else
                    Debug.LogWarning($"[InventoryUI] Icon untuk {evidence.evidenceName} null");
            }

            // Set nama
            if (detailName != null)
                detailName.text = evidence.evidenceName;

            // Set deskripsi
            if (detailDesc != null)
                detailDesc.text = evidence.description;
        }

        public void CloseDetail()
        {
            if (detailPanel != null)
                detailPanel.SetActive(false);
        }

        public void CloseInventory()
        {
            if (inventoryPanel != null && inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(false);
                
                if (Core.GameManager.Instance != null)
                    Core.GameManager.Instance.ResumeGame();
                    
                Debug.Log("[InventoryUI] Inventory ditutup manual");
            }
        }
    }
}