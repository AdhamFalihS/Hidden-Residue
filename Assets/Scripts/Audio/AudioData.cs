using UnityEngine;

namespace HiddenResidue.Core
{

    [CreateAssetMenu(fileName = "AudioData", menuName = "HiddenResidue/Audio Data")]
    public class AudioData : ScriptableObject
    {
        [Header("BGM — Background Music")]
        public AudioClip bgmMainMenu;
        public AudioClip bgmLevel1;
        public AudioClip bgmLevel2;
        public AudioClip bgmLevel3;

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

    }
}