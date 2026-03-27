using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace HiddenResidue.UI
{
    public class DialogBoxUI : MonoBehaviour
    {
        public static DialogBoxUI Instance { get; private set; }

        [Header("Panel Root")]

        [SerializeField] private GameObject dialogPanel;

        [Header("Konten Dialog")]

        [SerializeField] private TextMeshProUGUI speakerNameText;

        [SerializeField] private TextMeshProUGUI dialogText;

        [SerializeField] private Image           portrait;

        [Header("Indikator Lanjut")]

        [SerializeField] private GameObject continueIndicator;

        private void Awake()

        {

            Instance = this;

            if (dialogPanel) dialogPanel.SetActive(false);

        }

        private void OnEnable()

        {

            Dialog.DialogManager.OnLineStarted  += HandleLineStarted;

            Dialog.DialogManager.OnCharTyped    += HandleCharTyped;

            Dialog.DialogManager.OnLineComplete += HandleLineComplete;

            Dialog.DialogManager.OnDialogEnd    += HandleDialogEnd;

        }

        private void OnDisable()

        {

            Dialog.DialogManager.OnLineStarted  -= HandleLineStarted;

            Dialog.DialogManager.OnCharTyped    -= HandleCharTyped;

            Dialog.DialogManager.OnLineComplete -= HandleLineComplete;

            Dialog.DialogManager.OnDialogEnd    -= HandleDialogEnd;

        }

        private void Update()

        {

            if (dialogPanel == null || !dialogPanel.activeSelf) return;

            if (Keyboard.current == null) return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame ||

                Keyboard.current.enterKey.wasPressedThisFrame)

            {

                Dialog.DialogManager.Instance?.Advance();

            }

        }

        private void HandleLineStarted(Dialog.DialogLine line)

        {

            if (dialogPanel) dialogPanel.SetActive(true);

            if (speakerNameText) speakerNameText.text = line.speakerName;

            if (dialogText)      dialogText.text      = "";

            if (portrait != null)

            {

                portrait.gameObject.SetActive(line.portrait != null);

                if (line.portrait != null) portrait.sprite = line.portrait;

            }

            if (continueIndicator) continueIndicator.SetActive(false);

        }

        private void HandleCharTyped(string currentText)

        {

            if (dialogText) dialogText.text = currentText;

        }

        private void HandleLineComplete()

        {

            if (continueIndicator) continueIndicator.SetActive(true);

        }

        private void HandleDialogEnd()

        {

            if (dialogPanel) dialogPanel.SetActive(false);

        }

        public void OnPanelClicked()

        {

            Dialog.DialogManager.Instance?.Advance();

        }

    }

}