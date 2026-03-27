using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HiddenResidue.UI
{
    public class QuizUI : MonoBehaviour
    {
        public static QuizUI Instance { get; private set; }

        [Header("Panel Root")]
        [SerializeField] private GameObject quizPanel;

        [Header("Soal")]
        [SerializeField] private TextMeshProUGUI questionText;

        [Header("Jawaban — drag 4 Button ke sini (index 0-3)")]
        [SerializeField] private Button[] answerButtons;
        [SerializeField] private TextMeshProUGUI[] answerTexts;

        [Header("Timer")]
        [SerializeField] private Slider timerSlider;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Hasil / Feedback")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Color correctColor = new Color(0.3f, 0.85f, 0.3f);
        [SerializeField] private Color wrongColor = new Color(0.9f, 0.2f, 0.2f);

        private void Awake()
        {
            Instance = this;
            if (quizPanel) quizPanel.SetActive(false);
            if (resultPanel) resultPanel.SetActive(false);
        }

        private void OnEnable()
        {
            Quiz.QuizManager.OnQuestionShown += HandleQuestionShown;
            Quiz.QuizManager.OnTimerTick += HandleTimerTick;
            Quiz.QuizManager.OnAnswerResult += HandleAnswerResult;
            Quiz.QuizManager.OnQuizEnded += HandleQuizEnded;
        }

        private void OnDisable()
        {
            Quiz.QuizManager.OnQuestionShown -= HandleQuestionShown;
            Quiz.QuizManager.OnTimerTick -= HandleTimerTick;
            Quiz.QuizManager.OnAnswerResult -= HandleAnswerResult;
            Quiz.QuizManager.OnQuizEnded -= HandleQuizEnded;
        }

        private void HandleQuestionShown(Quiz.QuizQuestion question, float duration)
        {
            if (quizPanel) quizPanel.SetActive(true);
            if (resultPanel) resultPanel.SetActive(false);
            if (questionText) questionText.text = question.questionText;

            if (timerSlider)
            {
                timerSlider.maxValue = duration;
                timerSlider.value = duration;
            }

            if (timerText)
            {
                timerText.text = Mathf.CeilToInt(duration).ToString();
                timerText.color = Color.white;
            }

            for (int i = 0; i < answerButtons.Length; i++)
            {
                bool valid = i < question.answers.Length;
                answerButtons[i].gameObject.SetActive(valid);
                answerButtons[i].interactable = valid;

                if (!valid) continue;

                if (i < answerTexts.Length && answerTexts[i] != null)
                    answerTexts[i].text = question.answers[i];

                var colors = answerButtons[i].colors;
                colors.normalColor = Color.white;
                colors.disabledColor = new Color(0.8f, 0.8f, 0.8f);
                answerButtons[i].colors = colors;

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
                timerText.text = Mathf.CeilToInt(timeLeft).ToString();
                timerText.color = timeLeft < 5f ? wrongColor : Color.white;
            }
        }

        private void HandleAnswerResult(bool correct, string explanation)
        {
            foreach (var btn in answerButtons)
                btn.interactable = false;

            if (resultPanel) resultPanel.SetActive(true);
            if (resultText)
            {
                string prefix = correct ? "✓  Benar!" : "✗  Salah!";
                resultText.text = $"{prefix}\n\n{explanation}";
                resultText.color = correct ? correctColor : wrongColor;
            }
        }

        private void HandleQuizEnded()
        {
            if (quizPanel) quizPanel.SetActive(false);
        }

        public void Show() => quizPanel?.SetActive(true);
        public void Hide() => quizPanel?.SetActive(false);
    }
}