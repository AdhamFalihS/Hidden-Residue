using UnityEngine;
using System.Collections;

namespace HiddenResidue.Quiz
{
    /// <summary>
    /// QuizManager — Singleton yang mengelola alur kuis Bahasa Jepang.
    /// Dipanggil oleh LockedDoor saat pemain berinteraksi.
    /// Setelah selesai (benar/salah) → callback ke LockedDoor.
    /// </summary>
    public class QuizManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────
        public static QuizManager Instance { get; private set; }

        // ─── State ───────────────────────────────────────────────────────────
        private QuizData     currentQuiz;
        private int          currentQuestionIndex;
        private bool         isRunning   = false;
        private float        timeLeft;
        private Coroutine    timerRoutine;

        // Callback setelah kuis selesai: true = passed, false = failed
        private System.Action<bool> onQuizComplete;

        // ─── Events (untuk UI) ────────────────────────────────────────────────
        public static event System.Action<QuizQuestion, float> OnQuestionShown;   // (question, timerDuration)
        public static event System.Action<float>               OnTimerTick;        // timeLeft
        public static event System.Action<bool, string>        OnAnswerResult;     // (correct, explanation)
        public static event System.Action                      OnQuizEnded;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ─── Public API ──────────────────────────────────────────────────────

        /// <summary>Mulai kuis. Dipanggil oleh LockedDoor.</summary>
        public void StartQuiz(QuizData quizData, System.Action<bool> callback)
        {
            if (isRunning) return;

            currentQuiz          = quizData;
            currentQuestionIndex = 0;
            onQuizComplete       = callback;
            isRunning            = true;

            Core.GameManager.Instance?.SetState(Core.GameManager.GameState.Quiz);
            ShowQuestion();
        }

        /// <summary>Dipanggil oleh QuizUI saat pemain memilih jawaban.</summary>
        public void SubmitAnswer(int answerIndex)
        {
            if (!isRunning) return;
            if (timerRoutine != null) StopCoroutine(timerRoutine);

            var question = currentQuiz.questions[currentQuestionIndex];
            bool correct = answerIndex == question.correctAnswerIndex;

            OnAnswerResult?.Invoke(correct, question.explanationText);

            if (correct)
            {
                Core.ScoreManager.Instance?.AddQuizScore();
                // Setelah delay tampilkan hasil → end quiz
                StartCoroutine(DelayThenEnd(1.5f, true));
            }
            else
            {
                StartCoroutine(DelayThenEnd(1.5f, false));
            }
        }

        // ─── Private ─────────────────────────────────────────────────────────

        private void ShowQuestion()
        {
            if (currentQuestionIndex >= currentQuiz.questions.Length)
            {
                EndQuiz(true);
                return;
            }

            var question = currentQuiz.questions[currentQuestionIndex];
            timeLeft     = currentQuiz.timerDuration;

            OnQuestionShown?.Invoke(question, currentQuiz.timerDuration);

            if (timerRoutine != null) StopCoroutine(timerRoutine);
            timerRoutine = StartCoroutine(TimerCountdown());
        }

        private IEnumerator TimerCountdown()
        {
            while (timeLeft > 0f)
            {
                timeLeft -= Time.unscaledDeltaTime;  // unscaled: jalan meski Time.timeScale = 0
                OnTimerTick?.Invoke(timeLeft);
                yield return null;
            }

            // Waktu habis → gagal
            OnAnswerResult?.Invoke(false, "Waktu habis! Coba lagi.");
            StartCoroutine(DelayThenEnd(1.5f, false));
        }

        private IEnumerator DelayThenEnd(float delay, bool passed)
        {
            yield return new WaitForSecondsRealtime(delay);
            EndQuiz(passed);
        }

        private void EndQuiz(bool passed)
        {
            isRunning = false;
            OnQuizEnded?.Invoke();

            if (passed)
            {
                Core.GameManager.Instance?.SetState(Core.GameManager.GameState.Playing);
                onQuizComplete?.Invoke(true);
            }
            else
            {
                onQuizComplete?.Invoke(false);
                Core.GameManager.Instance?.TriggerFail();
            }

            Debug.Log($"[QuizManager] Quiz ended — passed: {passed}");
        }
    }
}
