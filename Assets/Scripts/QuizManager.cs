using UnityEngine;
using TMPro;
using UnityEngine.UI;
using OpenCover.Framework.Model;

public class QuizManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class Question
    {
        public string questionText;
        public string[] options;
        public int correctOptionIndex;
    }

    public GameObject quizPanel;
    public TMP_Text questionText;
    public Button[] optionButtons;
    public TMP_Text feedbackText;

    public Question[] questions;
    private int currentQuestionIndex = 0;
    void Start()
    {
        quizPanel.SetActive(false); // Hide quiz panel at start
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartQuiz()
    {
        quizPanel.SetActive(true);
        currentQuestionIndex = 0;

    }
}
