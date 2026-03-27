using UnityEngine;
using System.Collections;

namespace HiddenResidue.Quiz
{
    public class QuizManager : MonoBehaviour
    {
        public static QuizManager Instance { get; private set; }

        [Header("Time-Based Score")]
        [Tooltip("Skor maksimum jika menjawab di detik pertama")]
        [SerializeField] private int maxTimeBonus = 100;

        [Tooltip("Skor minimum meski menjawab di detik terakhir")]
        [SerializeField] private int minTimeBonus = 10;

        private QuizData            currentQuiz;

        private int                 currentQuestionIndex;

        private bool                isRunning   = false;

        private float               timeLeft;

        private float               totalTime;

        private Coroutine           timerRoutine;

        private System.Action<bool> onQuizComplete;

        public static event System.Action<QuizQuestion, float> OnQuestionShown;

        public static event System.Action<float>               OnTimerTick;

        public static event System.Action<bool, string>        OnAnswerResult;

        public static event System.Action                      OnQuizEnded;

        private void Awake()

        {

            if (Instance != null && Instance != this) { Destroy(gameObject); return; }

            Instance = this;

        }

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

        public void SubmitAnswer(int answerIndex)

        {

            if (!isRunning) return;

            if (timerRoutine != null) { StopCoroutine(timerRoutine); timerRoutine = null; }

            var  question = currentQuiz.questions[currentQuestionIndex];

            bool correct  = answerIndex == question.correctAnswerIndex;

            OnAnswerResult?.Invoke(correct, question.explanationText);

            if (correct)

            {

                Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.QuizCorrect);

                int timeBonus = CalculateTimeBonus(timeLeft, totalTime);

                Core.ScoreManager.Instance?.AddScore(timeBonus);

                UI.ScorePopupUI.Show(Vector3.zero, timeBonus);

                Debug.Log($"[QuizManager] Benar! Sisa waktu: {timeLeft:F1}s → Bonus: {timeBonus}");

                StartCoroutine(DelayThenEnd(1.5f, true));

            }

            else

            {

                Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.QuizWrong);

                Debug.Log("[QuizManager] Salah!");

                StartCoroutine(DelayThenEnd(1.5f, false));

            }

        }

        private void ShowQuestion()

        {

            if (currentQuestionIndex >= currentQuiz.questions.Length)

            {

                EndQuiz(true);

                return;

            }

            var question = currentQuiz.questions[currentQuestionIndex];

            totalTime    = currentQuiz.timerDuration;

            timeLeft     = totalTime;

            OnQuestionShown?.Invoke(question, totalTime);

            if (timerRoutine != null) StopCoroutine(timerRoutine);

            timerRoutine = StartCoroutine(TimerCountdown());

        }

        private IEnumerator TimerCountdown()

        {

            while (timeLeft > 0f)

            {

                timeLeft -= Time.unscaledDeltaTime;

                timeLeft  = Mathf.Max(timeLeft, 0f);

                OnTimerTick?.Invoke(timeLeft);

                yield return null;

            }

            OnAnswerResult?.Invoke(false, "Waktu habis! Coba lagi dari awal.");

            StartCoroutine(DelayThenEnd(1.5f, false));

        }

        private IEnumerator DelayThenEnd(float delay, bool passed)

        {

            yield return new WaitForSecondsRealtime(delay);

            EndQuiz(passed);

        }

        private void EndQuiz(bool passed)

        {

            isRunning    = false;

            timerRoutine = null;

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

            Debug.Log($"[QuizManager] Quiz selesai — passed: {passed}");

        }

        private int CalculateTimeBonus(float remaining, float total)

        {

            if (total <= 0f) return minTimeBonus;

            float ratio = Mathf.Clamp01(remaining / total);

            return Mathf.RoundToInt(Mathf.Lerp(minTimeBonus, maxTimeBonus, ratio));

        }

    }

}