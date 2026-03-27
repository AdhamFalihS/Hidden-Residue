using UnityEngine;
using UnityEngine.UI;

namespace HiddenResidue.UI
{
    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject settingsPanel;

        [Header("Sliders")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private bool _isUpdatingUI = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }

        private void Start()
        {
            var am = Core.AudioManager.Instance;
            if (am == null)
            {
                Debug.LogError("AudioManager not found!");
                return;
            }

            // ✅ Set initial value
            _isUpdatingUI = true;
            bgmSlider.value = am.BGMVolume;
            sfxSlider.value = am.SFXVolume;
            _isUpdatingUI = false;

            // ✅ Listener (WAJIB)
            bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        }

        private void OnEnable()
        {
            Core.AudioManager.OnAudioSettingsChanged += RefreshUI;
        }

        private void OnDisable()
        {
            Core.AudioManager.OnAudioSettingsChanged -= RefreshUI;
        }

        private void OnBGMSliderChanged(float value)
        {
            if (_isUpdatingUI) return;

            Core.AudioManager.Instance.SetBGMVolume(value);
        }

        private void OnSFXSliderChanged(float value)
        {
            if (_isUpdatingUI) return;

            Core.AudioManager.Instance.SetSFXVolume(value);

            // 🔥 BIAR TERASA REALTIME → play test sound
            Core.AudioManager.Instance.PlaySFX(Core.AudioManager.SFX.ButtonClick);
        }

        private void RefreshUI()
        {
            if (_isUpdatingUI) return;

            var am = Core.AudioManager.Instance;
            if (am == null) return;

            _isUpdatingUI = true;

            bgmSlider.value = am.BGMVolume;
            sfxSlider.value = am.SFXVolume;

            _isUpdatingUI = false;
        }

        public void OpenSettings()
        {
            Core.AudioManager.Instance.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            settingsPanel.SetActive(true);
            RefreshUI();
        }

        public void CloseSettings()
        {
            Core.AudioManager.Instance.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            settingsPanel.SetActive(false);
        }
    }
}