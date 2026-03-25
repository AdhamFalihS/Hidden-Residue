using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace HiddenResidue.Core
{

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public enum BGM  { None, MainMenu, Level1, Level2, Level3 }
        public enum SFX  { ButtonClick, PickupEvidence, Cleaning, CleaningDone,
                           QuizCorrect, QuizWrong, LevelComplete, Fail,
                           DoorOpen, DoorLocked, Typing }

        [Header("Audio Data (ScriptableObject)")]
        [SerializeField] private AudioData audioData;

        private const string KeyBGMVolume = "BGMVolume";
        private const string KeySFXVolume = "SFXVolume";
        private const string KeyBGMMute   = "BGMMute";
        private const string KeySFXMute   = "SFXMute";

        private AudioSource _bgmSource;
        private AudioSource _sfxSource;
        private AudioSource _sfxLoopSource;
        private BGM         _currentBGM = BGM.None;

        public float BGMVolume { get; private set; }
        public float SFXVolume { get; private set; }
        public bool  BGMMuted  { get; private set; }
        public bool  SFXMuted  { get; private set; }

        public static event System.Action OnAudioSettingsChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioSources();
            LoadSettings();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            switch (scene.name)
            {
                case "MainMenu":
                    PlayBGM(BGM.MainMenu);
                    break;
                case "Level1":
                case "Level 1 - Coffee Shop":
                    PlayBGM(BGM.Level1);
                    break;
                case "Level 2 - Apartemen":
                    PlayBGM(BGM.Level2);
                    break;
                case "Level3":
                    PlayBGM(BGM.Level3);
                    break;
                default:

                    break;
            }
        }

        public void PlayBGM(BGM bgm)
        {
            if (_currentBGM == bgm && _bgmSource.isPlaying) return;

            AudioClip clip = GetBGMClip(bgm);
            if (clip == null)
            {
                _bgmSource.Stop();
                _currentBGM = BGM.None;
                return;
            }

            _currentBGM       = bgm;
            _bgmSource.clip   = clip;
            _bgmSource.loop   = true;
            _bgmSource.volume = BGMMuted ? 0f : BGMVolume;
            _bgmSource.Play();
        }

        public void StopBGM() => _bgmSource.Stop();

        public void PlaySFX(SFX sfx)
        {
            if (SFXMuted) return;

            AudioClip clip = GetSFXClip(sfx);
            if (clip == null) return;

            _sfxSource.PlayOneShot(clip, SFXVolume);
        }

        public void PlaySFXAtPoint(SFX sfx, Vector3 worldPosition)
        {
            if (SFXMuted) return;

            AudioClip clip = GetSFXClip(sfx);
            if (clip == null) return;

            AudioSource.PlayClipAtPoint(clip, worldPosition, SFXVolume);
        }

        public void PlaySFXLoop(SFX sfx)
        {
            if (SFXMuted || _sfxLoopSource == null) return;

            AudioClip clip = GetSFXClip(sfx);
            if (clip == null) return;
            if (_sfxLoopSource.isPlaying && _sfxLoopSource.clip == clip) return;

            _sfxLoopSource.clip = clip;
            _sfxLoopSource.volume = SFXVolume;
            _sfxLoopSource.Play();
        }

        public void StopSFXLoop()
        {
            if (_sfxLoopSource == null) return;
            if (_sfxLoopSource.isPlaying)
                _sfxLoopSource.Stop();
        }

        public void SetBGMVolume(float volume)
        {
            BGMVolume             = Mathf.Clamp01(volume);
            _bgmSource.volume     = BGMMuted ? 0f : BGMVolume;
            PlayerPrefs.SetFloat(KeyBGMVolume, BGMVolume);
            PlayerPrefs.Save();
            OnAudioSettingsChanged?.Invoke();
        }

        public void SetSFXVolume(float volume)
        {
            SFXVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(KeySFXVolume, SFXVolume);
            PlayerPrefs.Save();
            OnAudioSettingsChanged?.Invoke();
        }

        public void SetBGMMute(bool mute)
        {
            BGMMuted          = mute;
            _bgmSource.volume = mute ? 0f : BGMVolume;
            PlayerPrefs.SetInt(KeyBGMMute, mute ? 1 : 0);
            PlayerPrefs.Save();
            OnAudioSettingsChanged?.Invoke();
        }

        public void SetSFXMute(bool mute)
        {
            SFXMuted = mute;
            PlayerPrefs.SetInt(KeySFXMute, mute ? 1 : 0);
            PlayerPrefs.Save();
            OnAudioSettingsChanged?.Invoke();
        }

        public void ToggleBGMMute() => SetBGMMute(!BGMMuted);
        public void ToggleSFXMute() => SetSFXMute(!SFXMuted);

        private void SetupAudioSources()
        {

            _bgmSource             = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop        = true;
            _bgmSource.spatialBlend = 0f;

            _sfxSource             = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop        = false;
            _sfxSource.spatialBlend = 0f;

            _sfxLoopSource         = gameObject.AddComponent<AudioSource>();
            _sfxLoopSource.loop    = true;
            _sfxLoopSource.spatialBlend = 0f;
            _sfxLoopSource.playOnAwake = false;
        }

        private void LoadSettings()
        {
            BGMVolume = PlayerPrefs.GetFloat(KeyBGMVolume, 0.8f);
            SFXVolume = PlayerPrefs.GetFloat(KeySFXVolume, 1.0f);
            BGMMuted  = PlayerPrefs.GetInt(KeyBGMMute, 0) == 1;
            SFXMuted  = PlayerPrefs.GetInt(KeySFXMute, 0) == 1;

            _bgmSource.volume = BGMMuted ? 0f : BGMVolume;
        }

        private AudioClip GetBGMClip(BGM bgm)
        {
            if (audioData == null) return null;
            return bgm switch
            {
                BGM.MainMenu => audioData.bgmMainMenu,
                BGM.Level1   => audioData.bgmLevel1,
                BGM.Level2   => audioData.bgmLevel2,
                BGM.Level3   => audioData.bgmLevel3,
                _            => null
            };
        }

        private AudioClip GetSFXClip(SFX sfx)
        {
            if (audioData == null) return null;
            return sfx switch
            {
                SFX.ButtonClick     => audioData.sfxButtonClick,
                SFX.PickupEvidence  => audioData.sfxPickupEvidence,
                SFX.Cleaning        => audioData.sfxCleaning,
                SFX.CleaningDone    => audioData.sfxCleaningDone,
                SFX.QuizCorrect     => audioData.sfxQuizCorrect,
                SFX.QuizWrong       => audioData.sfxQuizWrong,
                SFX.LevelComplete   => audioData.sfxLevelComplete,
                SFX.Fail            => audioData.sfxFail,
                SFX.DoorOpen        => audioData.sfxDoorOpen,
                SFX.DoorLocked      => audioData.sfxDoorLocked,
                SFX.Typing          => audioData.sfxTyping,
                _                   => null
            };
        }
    }
}