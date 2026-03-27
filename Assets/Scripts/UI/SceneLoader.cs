using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace HiddenResidue.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Loading UI")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMPro.TextMeshProUGUI loadingText;

        [Header("Settings")]
        [SerializeField] private float minLoadTime = 0.5f; 

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (loadingPanel != null) loadingPanel.SetActive(false);
        }

        public void LoadScene(int buildIndex)
        {
            StartCoroutine(LoadAsync(buildIndex));
        }

        public void LoadSceneByName(string sceneName)
        {
            StartCoroutine(LoadAsyncByName(sceneName));
        }

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
            if (loadingPanel != null) loadingPanel.SetActive(true);
            if (progressBar != null) progressBar.value = 0f;
            if (loadingText != null) loadingText.text = "Memuat...";

            op.allowSceneActivation = false;

            float elapsed = 0f;

            while (!op.isDone)
            {
                elapsed += Time.unscaledDeltaTime;

                float loadProgress = Mathf.Clamp01(op.progress / 0.9f);
                float timeProgress = Mathf.Clamp01(elapsed / minLoadTime);
                float display = Mathf.Min(loadProgress, timeProgress);

                if (progressBar != null) progressBar.value = display;
                if (loadingText != null) loadingText.text = $"Memuat... {Mathf.RoundToInt(display * 100)}%";

                if (op.progress >= 0.9f && elapsed >= minLoadTime)
                {
                    if (progressBar != null) progressBar.value = 1f;
                    if (loadingText != null) loadingText.text  = "Memuat... 100%";

                    yield return new WaitForSecondsRealtime(0.1f);
                    op.allowSceneActivation = true;
                }

                yield return null;
            }

            if (loadingPanel != null) loadingPanel.SetActive(false);
        }
    }
}
