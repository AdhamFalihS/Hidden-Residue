using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiddenResidue.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public enum BGM  { None, MainMenu, Level1, Level2, Level3 }
        public enum SFX  { ButtonClick, PickupEvidence, Cleaning, CleaningDone,
                           QuizCorrect, QuizWrong, LevelComplete, Fail,
                           DoorOpen, DoorLocked, Typing,
                           Footstep }

        [Header("Audio Data (ScriptableObject)")]
        [SerializeField] private AudioData audioData;

        private const string KeyBGMVolume = "BGMVolume";
        private const string KeySFXVolume = "SFXVolume";

        private AudioSource _bgmSource;
        private AudioSource _sfxSource;
        private AudioSource _sfxLoopSource;

        private BGM _currentBGM = BGM.None;

        public float BGMVolume { get; private set; }
        public float SFXVolume { get; private set; }

        public static event System.Action OnAudioSettingsChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Bersihkan AudioSource lama (biar tidak double)
            foreach (var a in GetComponents<AudioSource>())
                Destroy(a);

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
            }
        }

        // =========================
        // 🎵 BGM
        // =========================
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

            _currentBGM = bgm;
            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.volume = BGMVolume;
            _bgmSource.Play();
        }

        public void StopBGM()
        {
            if (_bgmSource != null)
                _bgmSource.Stop();
        }

        public void SetBGMVolume(float volume)
        {
            BGMVolume = Mathf.Clamp01(volume);

            if (_bgmSource != null)
                _bgmSource.volume = BGMVolume;

            PlayerPrefs.SetFloat(KeyBGMVolume, BGMVolume);
            PlayerPrefs.Save();

            OnAudioSettingsChanged?.Invoke();
        }

        // =========================
        // 🔊 SFX
        // =========================
        public void PlaySFX(SFX sfx)
        {
            AudioClip clip = GetSFXClip(sfx);
            if (clip == null) return;

            _sfxSource.PlayOneShot(clip, SFXVolume); // ✅ ini yang dipakai slider
        }

        public void PlaySFXAtPoint(SFX sfx, Vector3 pos)
        {

            AudioClip clip = GetSFXClip(sfx);
            if (clip == null) return;

            AudioSource.PlayClipAtPoint(clip, pos, SFXVolume);
        }

        public void PlaySFXLoop(SFX sfx)
        {
            if (_sfxLoopSource == null) return;

            AudioClip clip = GetSFXClip(sfx);
            if (clip == null) return;

            if (_sfxLoopSource.isPlaying && _sfxLoopSource.clip == clip) return;

            _sfxLoopSource.clip = clip;
            _sfxLoopSource.volume = SFXVolume;
            _sfxLoopSource.loop = true;
            _sfxLoopSource.Play();
        }

        public void StopSFXLoop()
        {
            if (_sfxLoopSource != null && _sfxLoopSource.isPlaying)
                _sfxLoopSource.Stop();
        }

        public void SetSFXVolume(float volume)
        {
            SFXVolume = Mathf.Clamp01(volume);

            // Update kedua source: 1-shot dan loop
            if (_sfxSource != null)
                _sfxSource.volume = SFXVolume;

            if (_sfxLoopSource != null)
                _sfxLoopSource.volume = SFXVolume;

            PlayerPrefs.SetFloat(KeySFXVolume, SFXVolume);
            PlayerPrefs.Save();

            OnAudioSettingsChanged?.Invoke();
        }

        // =========================
        // ⚙️ INIT
        // =========================
        private void SetupAudioSources()
        {
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.spatialBlend = 0f;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.spatialBlend = 0f;

            _sfxLoopSource = gameObject.AddComponent<AudioSource>();
            _sfxLoopSource.loop = true;
            _sfxLoopSource.playOnAwake = false;
            _sfxLoopSource.spatialBlend = 0f;
        }

        private void LoadSettings()
        {
            BGMVolume = PlayerPrefs.GetFloat(KeyBGMVolume, 0.7f);
            SFXVolume = PlayerPrefs.GetFloat(KeySFXVolume, 0.7f);

            if (_bgmSource != null)
                _bgmSource.volume = BGMVolume;

            if (_sfxSource != null)
                _sfxSource.volume = SFXVolume;

            if (_sfxLoopSource != null)
                _sfxLoopSource.volume = SFXVolume;

            OnAudioSettingsChanged?.Invoke();
        }

        // =========================
        // 🎧 DATA
        // =========================
        private AudioClip GetBGMClip(BGM bgm)
        {
            if (audioData == null) return null;

            return bgm switch
            {
                BGM.MainMenu => audioData.bgmMainMenu,
                BGM.Level1   => audioData.bgmLevel1,
                BGM.Level2   => audioData.bgmLevel2,
                BGM.Level3   => audioData.bgmLevel3,
                _ => null
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
                SFX.Footstep        => audioData.sfxFootstep,
                _ => null
            };
        }
        
    }
}