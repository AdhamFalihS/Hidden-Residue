using UnityEngine;
using UnityEngine.UI;

namespace HiddenResidue.UI
{
    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject settingsPanel;

        [Header("BGM Controls")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Toggle bgmMuteToggle;

        [Header("SFX Controls")]
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle sfxMuteToggle;

        private bool _isInitializing = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        private void OnEnable()
        {
            Core.AudioManager.OnAudioSettingsChanged += RefreshUI;
        }

        private void OnDisable()
        {
            Core.AudioManager.OnAudioSettingsChanged -= RefreshUI;
        }

        public void OpenSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
            RefreshUI();
        }

        public void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void ToggleSettings()
        {
            if (settingsPanel == null) return;
            if (settingsPanel.activeSelf) CloseSettings();
            else OpenSettings();
        }

        public void OnBGMSliderChanged(float value)
        {
            if (_isInitializing) return;
            Core.AudioManager.Instance?.SetBGMVolume(value);
        }

        public void OnSFXSliderChanged(float value)
        {
            if (_isInitializing) return;
            Core.AudioManager.Instance?.SetSFXVolume(value);
        }

        public void OnBGMMuteToggleChanged(bool mute)
        {
            if (_isInitializing) return;
            Core.AudioManager.Instance?.SetBGMMute(mute);
        }

        public void OnSFXMuteToggleChanged(bool mute)
        {
            if (_isInitializing) return;
            Core.AudioManager.Instance?.SetSFXMute(mute);
        }

        private void RefreshUI()
        {
            var am = Core.AudioManager.Instance;
            if (am == null) return;

            _isInitializing = true;

            if (bgmSlider     != null) bgmSlider.value        = am.BGMVolume;
            if (sfxSlider     != null) sfxSlider.value        = am.SFXVolume;
            if (bgmMuteToggle != null) bgmMuteToggle.isOn     = am.BGMMuted;
            if (sfxMuteToggle != null) sfxMuteToggle.isOn     = am.SFXMuted;

            _isInitializing = false;
        }
    }
}
