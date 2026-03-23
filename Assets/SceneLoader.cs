using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace HiddenResidue.Core
{
    /// <summary>
    /// SceneLoader — Singleton DontDestroyOnLoad dengan loading screen.
    /// Menampilkan panel loading + progress bar saat perpindahan scene.
    ///
    /// Setup:
    ///   1. Buat GameObject "SceneLoader" di scene pertama (MainMenu atau scene awal)
    ///   2. Attach script ini
    ///   3. Buat Canvas dengan panel loading di dalamnya, assign ke field di Inspector
    ///   4. SceneLoader otomatis ikut ke semua scene karena DontDestroyOnLoad
    ///
    /// Struktur UI loading:
    ///   Canvas (Sort Order: 99 agar selalu di atas)
    ///   └── LoadingPanel
    ///       ├── BackgroundImage
    ///       ├── Slider (progress bar)
    ///       └── LoadingText (TextMeshProUGUI — "Memuat...")
    ///
    /// Cara pakai dari script lain:
    ///   SceneLoader.Instance.LoadScene(buildIndex);
    ///   SceneLoader.Instance.LoadSceneByName("MainMenu");
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Loading UI")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider     progressBar;
        [SerializeField] private TMPro.TextMeshProUGUI loadingText;

        [Header("Settings")]
        [SerializeField] private float minLoadTime = 0.5f; // Minimum durasi loading agar tidak terlalu kilat

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (loadingPanel != null) loadingPanel.SetActive(false);
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public void LoadScene(int buildIndex)
        {
            StartCoroutine(LoadAsync(buildIndex));
        }

        public void LoadSceneByName(string sceneName)
        {
            StartCoroutine(LoadAsyncByName(sceneName));
        }

        // ─── Private ─────────────────────────────────────────────────────────

        private IEnumerator LoadAsync(int buildIndex)
        {
            yield return DoLoad(SceneManager.LoadSceneAsync(buildIndex));
        }

        private IEnumerator LoadAsyncByName(string sceneName)
        {
            yield return DoLoad(SceneManager.LoadSceneAsync(sceneName));
        }

        private IEnumerator DoLoad(AsyncOperation op)
        {
            // Tampilkan loading panel
            if (loadingPanel != null) loadingPanel.SetActive(true);
            if (progressBar  != null) progressBar.value = 0f;
            if (loadingText  != null) loadingText.text  = "Memuat...";

            op.allowSceneActivation = false;

            float elapsed = 0f;

            while (!op.isDone)
            {
                elapsed += Time.unscaledDeltaTime;

                // Progress: 0–0.9 dari AsyncOperation, 0.9–1.0 dari minLoadTime
                float loadProgress = Mathf.Clamp01(op.progress / 0.9f);
                float timeProgress = Mathf.Clamp01(elapsed / minLoadTime);
                float display      = Mathf.Min(loadProgress, timeProgress);

                if (progressBar != null) progressBar.value = display;
                if (loadingText != null) loadingText.text  = $"Memuat... {Mathf.RoundToInt(display * 100)}%";

                // Aktifkan scene setelah load selesai DAN minimum time tercapai
                if (op.progress >= 0.9f && elapsed >= minLoadTime)
                {
                    if (progressBar != null) progressBar.value = 1f;
                    if (loadingText != null) loadingText.text  = "Memuat... 100%";

                    // Sedikit jeda agar 100% terlihat
                    yield return new WaitForSecondsRealtime(0.1f);
                    op.allowSceneActivation = true;
                }

                yield return null;
            }

            if (loadingPanel != null) loadingPanel.SetActive(false);
        }
    }
}
