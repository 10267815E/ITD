using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class QuizManager : MonoBehaviour
{
    
    [System.Serializable]
    public class QuestionData
    {
        [TextArea] public string questionText;
        public string[] answers; 
        public int correctAnswerIndex; // 0, 1, or 2
    }

    public QuestionData[] questions; 

    // --- 2. UI REFERENCES (Drag these in Inspector) ---
    [Header("UI Elements")]
    public TextMeshProUGUI questionTextUI;
    public TextMeshProUGUI[] answerButtonTexts; 
    
    [Header("Panels")]
    public GameObject tryAgainPanel;
    public GameObject winPanel;
    public GameObject questionPanel; // The main panel holding questions

    
    private int currentQuestionIndex = 0;

    void Start()
    {
        // When the canvas turns on, start the first question
        currentQuestionIndex = 0;
        LoadQuestion();
        
        // Ensure popups are hidden
        tryAgainPanel.SetActive(false);
        winPanel.SetActive(false);
        questionPanel.SetActive(true);
    }

    

    void LoadQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            QuestionData currentQ = questions[currentQuestionIndex];

            // Set the question text
            questionTextUI.text = currentQ.questionText;

            // Set the answer button text
            for (int i = 0; i < answerButtonTexts.Length; i++)
            {
                if (i < currentQ.answers.Length)
                {
                    answerButtonTexts[i].text = currentQ.answers[i];
                }
            }
        }
        else
        {
            // No more questions means win
            ShowWin();
        }
    }

     
    // Button 1 sends index 0, Button 2 sends index 1, etc.
    public void OnAnswerSelected(int index)
    {
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {
            // CORRECT: Go to next question
            currentQuestionIndex++;
            LoadQuestion();
        }
        else
        {
            // WRONG: Show Try Again
            tryAgainPanel.SetActive(true);
        }
    }

    
    public void CloseTryAgain()
    {
        tryAgainPanel.SetActive(false);
    }

    void ShowWin()
    {
        questionPanel.SetActive(false);
        winPanel.SetActive(true);
        // Firebase later
    }
}