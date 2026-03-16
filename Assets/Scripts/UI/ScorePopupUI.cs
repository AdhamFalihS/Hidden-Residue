using UnityEngine;
using TMPro;
using System.Collections;

namespace HiddenResidue.UI
{
    /// <summary>
    /// ScorePopupUI — Teks "+10", "+25", "+50" yang muncul mengambang
    /// di atas objek lalu menghilang.
    ///
    /// Cara pakai:
    ///   ScorePopupUI.Show(transform.position, 10);
    ///
    /// Setup:
    ///   1. Buat prefab "ScorePopup":
    ///      - Canvas (World Space, Sort Order tinggi)
    ///        └── TextMeshProUGUI (teks "+XX", warna kuning, bold)
    ///   2. Letakkan prefab di folder Resources/Prefabs/ScorePopup
    ///   3. Attach script ini ke prefab tersebut
    /// </summary>
    public class ScorePopupUI : MonoBehaviour
    {
        [Header("Animasi")]
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private float floatSpeed   = 1.5f;   // Kecepatan naik
        [SerializeField] private float fadeDuration = 1.0f;   // Lama sebelum hilang

        // ─── Static prefab cache ──────────────────────────────────────────────
        private static GameObject _prefab;

        // ─── Static Factory ───────────────────────────────────────────────────

        /// <summary>Spawn popup skor di posisi dunia tertentu.</summary>
        public static void Show(Vector3 worldPosition, int amount)
        {
            if (_prefab == null)
                _prefab = Resources.Load<GameObject>("Prefabs/ScorePopup");

            if (_prefab == null)
            {
                Debug.LogWarning("[ScorePopupUI] Prefab 'Resources/Prefabs/ScorePopup' tidak ditemukan!");
                return;
            }

            var go  = Instantiate(_prefab, worldPosition + Vector3.up * 0.5f, Quaternion.identity);
            var pop = go.GetComponent<ScorePopupUI>();
            pop?.Init(amount);
        }

        // ─── Instance ─────────────────────────────────────────────────────────

        public void Init(int amount)
        {
            if (label) label.text = $"+{amount}";
            StartCoroutine(AnimateAndDestroy());
        }

        private IEnumerator AnimateAndDestroy()
        {
            float   elapsed   = 0f;
            Vector3 startPos  = transform.position;
            Color   origColor = label != null ? label.color : Color.yellow;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t  = elapsed / fadeDuration;

                // Naik ke atas
                transform.position = startPos + Vector3.up * (floatSpeed * elapsed);

                // Fade out
                if (label)
                    label.color = new Color(origColor.r, origColor.g, origColor.b,
                                            Mathf.Lerp(1f, 0f, t));

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
