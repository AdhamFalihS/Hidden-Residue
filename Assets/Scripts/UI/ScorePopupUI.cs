using UnityEngine;

using TMPro;

using System.Collections;

namespace HiddenResidue.UI

{

    public class ScorePopupUI : MonoBehaviour

    {

        [Header("Animasi")]

        [SerializeField] private TextMeshProUGUI label;

        [SerializeField] private float floatSpeed   = 1.5f;

        [SerializeField] private float fadeDuration = 1.0f;

        private static GameObject _prefab;

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

                transform.position = startPos + Vector3.up * (floatSpeed * elapsed);

                if (label)

                    label.color = new Color(origColor.r, origColor.g, origColor.b,

                                            Mathf.Lerp(1f, 0f, t));

                yield return null;

            }

            Destroy(gameObject);

        }

    }

}