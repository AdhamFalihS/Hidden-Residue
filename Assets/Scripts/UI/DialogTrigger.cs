using UnityEngine;

namespace HiddenResidue.Dialog

{

    public class DialogTrigger : MonoBehaviour

    {

        [Header("Dialog")]

        [SerializeField] private DialogData dialogData;

        [Header("Trigger Settings")]

        [Tooltip("Jika true: dialog hanya muncul sekali. Jika false: muncul setiap player masuk.")]

        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)

        {

            if (!other.CompareTag("Player")) return;

            if (triggerOnce && hasTriggered) return;

            if (dialogData == null) return;

            if (DialogManager.Instance == null) return;

            hasTriggered = true;

            DialogManager.Instance.StartDialog(dialogData);

        }

        public void ResetTrigger() => hasTriggered = false;

        private void OnDrawGizmos()

        {

            var col = GetComponent<BoxCollider2D>();

            if (col == null) return;

            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.25f);

            Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size);

            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.8f);

            Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.size);

        }

    }

}