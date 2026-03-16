using UnityEngine;

namespace HiddenResidue.Quiz
{
    /// <summary>
    /// QuizQuestion — Data satu pertanyaan kuis Bahasa Jepang.
    /// </summary>
    [System.Serializable]
    public class QuizQuestion
    {
        [TextArea(1, 3)]
        public string questionText           = "Apa arti kata 'きれい' (kirei)?";
        public string[] answers              = { "Kotor", "Bersih / Cantik", "Berbahaya", "Besar" };
        [Range(0, 3)]
        public int correctAnswerIndex        = 1;
        [TextArea(1, 3)]
        public string explanationText        = "'Kirei' (きれい) berarti bersih atau cantik dalam Bahasa Jepang.";
    }

    /// <summary>
    /// QuizData — ScriptableObject yang menyimpan satu set pertanyaan untuk
    /// satu pintu/area terkunci.
    /// Buat via: klik kanan → Create → HiddenResidue → Quiz Data
    /// </summary>
    [CreateAssetMenu(fileName = "QuizData_", menuName = "HiddenResidue/Quiz Data")]
    public class QuizData : ScriptableObject
    {
        [Header("Info")]
        public string quizTitle  = "Kuis Bahasa Jepang — Level 1";

        [Header("Questions")]
        public QuizQuestion[] questions;

        [Header("Settings")]
        public float timerDuration = 15f;  // Detik per soal
    }
}
