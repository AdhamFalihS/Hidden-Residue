using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace HiddenResidue.UI
{
    public class EvidenceSlot : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button button;

        private Evidence.EvidenceData evidenceData;
        private Action<Evidence.EvidenceData> onClickCallback;

        private void Awake()
        {
            // Auto-get components jika belum diassign
            if (button == null)
                button = GetComponent<Button>();
                
            if (iconImage == null)
                iconImage = GetComponentInChildren<Image>();
                
            if (nameText == null)
                nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Setup(Evidence.EvidenceData data, Action<Evidence.EvidenceData> callback)
        {
            evidenceData = data;
            onClickCallback = callback;

            // Set UI
            if (iconImage != null && data.icon != null)
                iconImage.sprite = data.icon;

            if (nameText != null)
                nameText.text = data.evidenceName;

            // Setup button
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnSlotClicked);
            }
        }

        public void OnSlotClicked()
        {
            onClickCallback?.Invoke(evidenceData);
        }

        public Evidence.EvidenceData CurrentEvidence => evidenceData;
    }
}