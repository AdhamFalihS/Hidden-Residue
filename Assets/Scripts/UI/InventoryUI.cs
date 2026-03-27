using UnityEngine;

using UnityEngine.UI;

using TMPro;

using UnityEngine.InputSystem;

namespace HiddenResidue.UI

{

    public class InventoryUI : MonoBehaviour

    {

        public static InventoryUI Instance { get; private set; }

        [Header("Panel Root ï¿½ assign InventoryUI_Panel di sini")]

        [SerializeField] private GameObject inventoryPanel;

        [Header("Grid / List ï¿½ assign Content (RectTransform) di sini")]

        [SerializeField] private Transform  gridParent;

        [SerializeField] private GameObject evidenceSlotPrefab;

        [Header("Detail Popup")]

        [SerializeField] private GameObject      detailPanel;

        [SerializeField] private Image           detailIcon;

        [SerializeField] private TextMeshProUGUI detailName;

        [SerializeField] private TextMeshProUGUI detailDesc;

        private void Awake()

        {

            if (Instance != null && Instance != this) { Destroy(gameObject); return; }

            Instance = this;

            if (inventoryPanel == null)

                Debug.LogError("[InventoryUI] 'Inventory Panel' belum di-assign! Drag InventoryUI_Panel ke slot ini.");

            if (gridParent == null)

                Debug.LogError("[InventoryUI] 'Grid Parent' belum di-assign! Drag Content ke slot ini.");

            if (evidenceSlotPrefab == null)

                Debug.LogError("[InventoryUI] 'Evidence Slot Prefab' belum di-assign!");

            if (inventoryPanel != null) inventoryPanel.SetActive(false);

            if (detailPanel != null)    detailPanel.SetActive(false);

        }

        private void OnEnable()

        {

            Evidence.EvidenceManager.OnEvidenceAdded += OnEvidenceAdded;

            Debug.Log("[InventoryUI] Subscribed to OnEvidenceAdded");

        }

        private void OnDisable()

        {

            Evidence.EvidenceManager.OnEvidenceAdded -= OnEvidenceAdded;

        }

        private void Update()

        {

            if (Keyboard.current == null) return;

            if (Keyboard.current.bKey.wasPressedThisFrame)

                ToggleInventory();

        }

        private void OnEvidenceAdded(Evidence.EvidenceData data)

        {

            Debug.Log($"[InventoryUI] OnEvidenceAdded: {data?.evidenceName}");

            if (inventoryPanel != null && inventoryPanel.activeSelf)

                RefreshGrid();

        }

        public void ToggleInventory()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            if (inventoryPanel == null)
            {
                Debug.LogError("[InventoryUI] inventoryPanel NULL â€” assign di Inspector!");
                return;
            }
            bool buka = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(buka);
            Debug.Log($"[InventoryUI] Panel {(buka ? "DIBUKA" : "DITUTUP")}");
            if (buka)
            {
                if (detailPanel != null) detailPanel.SetActive(false);
                RefreshGrid();
            }
        }

        public void RefreshGrid()

        {

            if (gridParent == null)

            {

                Debug.LogError("[InventoryUI] gridParent NULL! Assign 'Content' ke slot Grid Parent.");

                return;

            }

            if (evidenceSlotPrefab == null)

            {

                Debug.LogError("[InventoryUI] evidenceSlotPrefab NULL!");

                return;

            }

            var mgr = Evidence.EvidenceManager.Instance;

            if (mgr == null) { Debug.LogError("[InventoryUI] EvidenceManager NULL!"); return; }

            for (int i = gridParent.childCount - 1; i >= 0; i--)

                Destroy(gridParent.GetChild(i).gameObject);

            var list = mgr.FoundEvidence;

            Debug.Log($"[InventoryUI] RefreshGrid ï¿½ {list.Count} evidence");

            foreach (var ev in list)

            {

                if (ev != null) BuatSlot(ev);

            }

            var rt = gridParent.GetComponent<RectTransform>();

            if (rt != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

        }

        private void BuatSlot(Evidence.EvidenceData ev)

        {

            var slot = Instantiate(evidenceSlotPrefab, gridParent);

            slot.name = "Slot_" + ev.evidenceName;

            foreach (var img in slot.GetComponentsInChildren<Image>(true))

            {

                if (img.gameObject == slot) continue;

                if (ev.icon != null) img.sprite = ev.icon;

                break;

            }

            var tmp = slot.GetComponentInChildren<TextMeshProUGUI>(true);

            if (tmp != null) tmp.text = ev.evidenceName;

            var btn = slot.GetComponent<Button>();

            if (btn != null)

            {

                btn.onClick.RemoveAllListeners();

                btn.onClick.AddListener(() => {
                    Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
                    TampilkanDetail(ev);
                });

            }

        }

        private void TampilkanDetail(Evidence.EvidenceData ev)

        {

            if (detailPanel == null) return;

            detailPanel.SetActive(true);

            if (detailIcon != null && ev.icon != null) detailIcon.sprite = ev.icon;

            if (detailName != null) detailName.text = ev.evidenceName;

            if (detailDesc != null) detailDesc.text = ev.description;

        }

        public void CloseDetail()    { 
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            if (detailPanel != null) detailPanel.SetActive(false); 
        }

        public void CloseInventory() { 
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            if (inventoryPanel != null) inventoryPanel.SetActive(false); 
        }

    }

}

