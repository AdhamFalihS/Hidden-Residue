using UnityEngine;

using TMPro;

using System.Collections;

namespace HiddenResidue.UI

{

    public class NotificationUI : MonoBehaviour

    {

        public static NotificationUI Instance { get; private set; }

        [Header("Panel")]

        [SerializeField] private GameObject      notifPanel;

        [SerializeField] private TextMeshProUGUI notifText;

        [Header("Durasi")]

        [SerializeField] private float displayDuration = 3f;

        private Coroutine _hideRoutine;

        private void Awake()

        {

            Instance = this;

            if (notifPanel) notifPanel.SetActive(false);

        }

        public static void Show(string message)

        {

            if (Instance == null)

            {

                Debug.LogWarning("[NotificationUI] Instance belum ada di scene!");

                return;

            }

            Instance.ShowMessage(message);

        }

        public void ShowMessage(string message)

        {

            if (notifPanel) notifPanel.SetActive(true);

            if (notifText)  notifText.text = message;

            if (_hideRoutine != null) StopCoroutine(_hideRoutine);

            _hideRoutine = StartCoroutine(HideAfterDelay());

        }

        private IEnumerator HideAfterDelay()

        {

            yield return new WaitForSeconds(displayDuration);

            if (notifPanel) notifPanel.SetActive(false);

        }

    }

}