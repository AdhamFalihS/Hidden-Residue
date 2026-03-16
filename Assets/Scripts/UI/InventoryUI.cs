using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace HiddenResidue.UI
{
    /// <summary>
    /// InventoryUI — Panel daftar barang bukti yang sudah ditemukan.
    /// Buka/tutup dengan tombol I.
    /// Klik slot → tampilkan detail item.
    ///
    /// Attach ke: GameObject "InventoryPanel" di Canvas.
    ///
    /// Struktur UI:
    ///   InventoryPanel
    ///   ├── ScrollView → Viewport → Content  ← ini "gridParent"
    ///   ├── DetailPanel (hidden awal)
    ///   │   ├── DetailIcon     (Image)
    ///   │   ├── DetailName     (TextMeshProUGUI)
    ///   │   ├── DetailDesc     (TextMeshProUGUI)
    ///   │   └── CloseButton    (Button → OnClick: InventoryUI.CloseDetail)
    ///   └── CloseInventoryButton (Button → OnClick: InventoryUI.ToggleInventory)
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public static InventoryUI Instance { get; private set; }

        [Header("Panel Root")]
        [SerializeField] private GameObject inventoryPanel;

        [Header("Grid / List")]
        [SerializeField] private Transform  gridParent;           // Content dalam ScrollView
        [SerializeField] private GameObject evidenceSlotPrefab;   // Prefab: Image + TMP + Button

        [Header("Detail Popup")]
        [SerializeField] private GameObject      detailPanel;
        [SerializeField] private Image           detailIcon;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDesc;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            if (inventoryPanel) inventoryPanel.SetActive(false);
            if (detailPanel)    detailPanel.SetActive(false);
        }

        private void OnEnable()
        {
            Evidence.EvidenceManager.OnEvidenceAdded += HandleEvidenceAdded;
        }

        private void OnDisable()
        {
            Evidence.EvidenceManager.OnEvidenceAdded -= HandleEvidenceAdded;
        }

        private void Update()
        {
            if (Keyboard.current == null) return;
            // Hanya bisa buka saat Playing atau saat inventory sudah terbuka
            bool canToggle = (Core.GameManager.Instance != null &&
                              Core.GameManager.Instance.IsPlaying) ||
                             (inventoryPanel != null && inventoryPanel.activeSelf);

            if (canToggle && Keyboard.current.iKey.wasPressedThisFrame)
                ToggleInventory();
        }

        // ─── Event Handler ────────────────────────────────────────────────────
        private void HandleEvidenceAdded(Evidence.EvidenceData _)
        {
            // Auto-refresh grid jika inventory sedang terbuka
            if (inventoryPanel != null && inventoryPanel.activeSelf)
                RefreshGrid();
        }

        // ─── Public API ───────────────────────────────────────────────────────

        public void ToggleInventory()
        {
            if (inventoryPanel == null) return;

            bool nowOpen = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(nowOpen);

            if (nowOpen)
            {
                RefreshGrid();
                if (detailPanel) detailPanel.SetActive(false);
            }
        }

        public void CloseDetail()
        {
            if (detailPanel) detailPanel.SetActive(false);
        }

        // ─── Private ──────────────────────────────────────────────────────────

        private void RefreshGrid()
        {
            if (gridParent == null || evidenceSlotPrefab == null) return;

            // Bersihkan slot lama
            foreach (Transform child in gridParent)
                Destroy(child.gameObject);

            var list = Evidence.EvidenceManager.Instance?.FoundEvidence;
            if (list == null) return;

            foreach (var ev in list)
            {
                var slot = Instantiate(evidenceSlotPrefab, gridParent);

                // Isi icon
                var img = slot.GetComponentInChildren<Image>();
                if (img != null && ev.icon != null) img.sprite = ev.icon;

                // Isi nama
                var txt = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = ev.evidenceName;

                // Klik slot → tampilkan detail
                var btn = slot.GetComponent<Button>();
                if (btn != null)
                {
                    var captured = ev;
                    btn.onClick.AddListener(() => ShowDetail(captured));
                }
            }
        }

        private void ShowDetail(Evidence.EvidenceData ev)
        {
            if (detailPanel == null) return;
            detailPanel.SetActive(true);

            if (detailIcon != null && ev.icon != null) detailIcon.sprite = ev.icon;
            if (detailName != null) detailName.text = ev.evidenceName;
            if (detailDesc != null) detailDesc.text  = ev.description;
        }
    }
}
