using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HiddenResidue.UI
{
    /// <summary>
    /// QuizUI — Panel kuis Bahasa Jepang.
    /// Mendengarkan event dari QuizManager dan menampilkan soal + pilihan jawaban.
    ///
    /// Attach ke: GameObject "QuizPanel" di Canvas.
    ///
    /// Struktur UI yang dibutuhkan di dalam QuizPanel:
    ///   ├── QuestionText       (TextMeshProUGUI)
    ///   ├── TimerSlider        (Slider)
    ///   ├── TimerText          (TextMeshProUGUI)
    ///   ├── AnswerButton_0     (Button + TMP child)
    ///   ├── AnswerButton_1     (Button + TMP child)
    ///   ├── AnswerButton_2     (Button + TMP child)
    ///   ├── AnswerButton_3     (Button + TMP child)
    ///   └── ResultPanel        (Panel, hidden awal)
    ///         └── ResultText   (TextMeshProUGUI)
    /// </summary>
    public class QuizUI : MonoBehaviour
    {
        public static QuizUI Instance { get; private set; }

        [Header("Panel Root")]
        [SerializeField] private GameObject quizPanel;

        [Header("Soal")]
        [SerializeField] private TextMeshProUGUI questionText;

        [Header("Jawaban — drag 4 Button ke sini (index 0-3)")]
        [SerializeField] private Button[]            answerButtons;   // 4 tombol
        [SerializeField] private TextMeshProUGUI[]   answerTexts;     // TMP di dalam tiap tombol

        [Header("Timer")]
        [SerializeField] private Slider          timerSlider;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Hasil / Feedback")]
        [SerializeField] private GameObject      resultPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Color correctColor = new Color(0.3f, 0.85f, 0.3f);
        [SerializeField] private Color wrongColor   = new Color(0.9f, 0.2f, 0.2f);

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;

            // Pastikan panel tersembunyi di awal
            if (quizPanel)   quizPanel.SetActive(false);
            if (resultPanel) resultPanel.SetActive(false);
        }

        private void OnEnable()
        {
            Quiz.QuizManager.OnQuestionShown += HandleQuestionShown;
            Quiz.QuizManager.OnTimerTick     += HandleTimerTick;
            Quiz.QuizManager.OnAnswerResult  += HandleAnswerResult;
            Quiz.QuizManager.OnQuizEnded     += HandleQuizEnded;
        }

        private void OnDisable()
        {
            Quiz.QuizManager.OnQuestionShown -= HandleQuestionShown;
            Quiz.QuizManager.OnTimerTick     -= HandleTimerTick;
            Quiz.QuizManager.OnAnswerResult  -= HandleAnswerResult;
            Quiz.QuizManager.OnQuizEnded     -= HandleQuizEnded;
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        private void HandleQuestionShown(Quiz.QuizQuestion question, float duration)
        {
            // Tampilkan panel
            if (quizPanel) quizPanel.SetActive(true);
            if (resultPanel) resultPanel.SetActive(false);

            // Isi teks soal
            if (questionText) questionText.text = question.questionText;

            // Set timer
            if (timerSlider)
            {
                timerSlider.maxValue = duration;
                timerSlider.value    = duration;
            }
            if (timerText)
            {
                timerText.text  = Mathf.CeilToInt(duration).ToString();
                timerText.color = Color.white;
            }

            // Isi dan setup tombol jawaban
            for (int i = 0; i < answerButtons.Length; i++)
            {
                bool valid = i < question.answers.Length;
                answerButtons[i].gameObject.SetActive(valid);
                answerButtons[i].interactable = valid;

                if (!valid) continue;

                // Isi teks jawaban
                if (i < answerTexts.Length && answerTexts[i] != null)
                    answerTexts[i].text = question.answers[i];

                // Reset warna tombol ke default
                var colors = answerButtons[i].colors;
                colors.normalColor    = Color.white;
                colors.disabledColor  = new Color(0.8f, 0.8f, 0.8f);
                answerButtons[i].colors = colors;

                // Pasang listener — capture index dengan variabel lokal
                int captured = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() =>
                {
                    Quiz.QuizManager.Instance?.SubmitAnswer(captured);
                });
            }
        }

        private void HandleTimerTick(float timeLeft)
        {
            if (timerSlider) timerSlider.value = timeLeft;
            if (timerText)
            {
                timerText.text  = Mathf.CeilToInt(timeLeft).ToString();
                // Merah saat sisa waktu < 5 detik
                timerText.color = timeLeft < 5f ? wrongColor : Color.white;
            }
        }

        private void HandleAnswerResult(bool correct, string explanation)
        {
            // Nonaktifkan semua tombol agar tidak bisa diklik lagi
            foreach (var btn in answerButtons)
                btn.interactable = false;

            // Tampilkan panel hasil
            if (resultPanel) resultPanel.SetActive(true);
            if (resultText)
            {
                string prefix    = correct ? "✓  Benar!" : "✗  Salah!";
                resultText.text  = $"{prefix}\n\n{explanation}";
                resultText.color = correct ? correctColor : wrongColor;
            }
        }

        private void HandleQuizEnded()
        {
            if (quizPanel) quizPanel.SetActive(false);
        }

        // ─── Public ───────────────────────────────────────────────────────────
        public void Show() => quizPanel?.SetActive(true);
        public void Hide() => quizPanel?.SetActive(false);
    }
}
