using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options;
        public int correctIndex;
    }

    public GameObject quizBackground;
    public TMP_Text questionText;
    public Button[] optionButtons;
    public TMP_Text feedbackText;

    public Question[] questions;
    private int currentQuestionIndex = 0;

    void Start()
    {
        quizBackground.SetActive(false);
    }

    public void StartQuiz()
    {
        quizBackground.SetActive(true);
        currentQuestionIndex = 0;
        ShowQuestion();
    }

    void ShowQuestion()
    {
        Question q = questions[currentQuestionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i; // capture variable
            optionButtons[i].GetComponentInChildren<TMP_Text>().text = q.options[i];
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        feedbackText.text = "";
    }

    void CheckAnswer(int index)
    {
        if (index == questions[currentQuestionIndex].correctIndex)
        {
            feedbackText.text = "Correct!";
        }
        else
        {
            feedbackText.text = "Wrong!";
        }

        // Next question or end
        currentQuestionIndex++;
        if (currentQuestionIndex < questions.Length)
            Invoke(nameof(ShowQuestion), 1.5f);
        else
            Invoke(nameof(EndQuiz), 1.5f);
    }

    void EndQuiz()
    {
        feedbackText.text = "Quiz Complete!";
        
    }
}
