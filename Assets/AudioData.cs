using UnityEngine;

namespace HiddenResidue.Core
{
    // ─────────────────────────────────────────────────────────────────────────
    // SCRIPTABLE OBJECT: AudioData
    // Buat via: klik kanan di Project → Create → HiddenResidue → Audio Data
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// AudioData — ScriptableObject tempat menyimpan semua clip audio game.
    /// Satu file ini berisi semua BGM dan SFX. Assign clip di Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioData", menuName = "HiddenResidue/Audio Data")]
    public class AudioData : ScriptableObject
    {
        [Header("BGM — Background Music")]
        public AudioClip bgmMainMenu;
        public AudioClip bgmLevel1;
        public AudioClip bgmLevel2;
        public AudioClip bgmLevel3;
        // Tambah BGM level baru di sini

        [Header("SFX — Sound Effects")]
        public AudioClip sfxButtonClick;
        public AudioClip sfxPickupEvidence;
        public AudioClip sfxCleaning;
        public AudioClip sfxCleaningDone;
        public AudioClip sfxQuizCorrect;
        public AudioClip sfxQuizWrong;
        public AudioClip sfxLevelComplete;
        public AudioClip sfxFail;
        public AudioClip sfxDoorOpen;
        public AudioClip sfxDoorLocked;
        // Tambah SFX baru di sini
    }
}
