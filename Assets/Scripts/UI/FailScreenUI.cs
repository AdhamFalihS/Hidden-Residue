using UnityEngine;

using TMPro;

using System.Collections;

namespace HiddenResidue.UI

{

    public class FailScreenUI : MonoBehaviour

    {

        public static FailScreenUI Instance { get; private set; }

        [Header("Panel")]

        [SerializeField] private GameObject  failPanel;

        private void Awake()

        {

            Instance = this;

            if (failPanel) failPanel.SetActive(false);

        }

        public void Show()

        {

            Debug.Log("FAIL PANEL SHOW DIPANGGIL");

            if (failPanel != null)

            {

                failPanel.SetActive(true);

                Debug.Log("FAIL PANEL AKTIF");

            }

            else

            {

                Debug.LogError("PANEL NULL!");

            }

        }

        public void OnRetryClicked()

        {

            Core.GameManager.Instance?.RetryLevel();

        }

        public void OnMainMenuClicked()

        {

            Core.GameManager.Instance?.GoToMainMenu();

        }

    }

}