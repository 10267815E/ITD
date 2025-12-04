using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions; 

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestionData
    {
        [TextArea] public string questionText;
        public string[] answers; 
        public int correctAnswerIndex; 
    }

    public QuestionData[] questions; 

    [Header("UI Elements")]
    public TextMeshProUGUI questionTextUI;
    public TextMeshProUGUI[] answerButtonTexts; 
    
    [Header("Panels")]
    public GameObject tryAgainPanel;
    public GameObject winPanel;
    public GameObject questionPanel; 

    private int currentQuestionIndex = 0;

    void Start()
    {
        currentQuestionIndex = 0;
        LoadQuestion();
        
        tryAgainPanel.SetActive(false);
        winPanel.SetActive(false);
        questionPanel.SetActive(true);
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex < questions.Length)
        {
            QuestionData currentQ = questions[currentQuestionIndex];
            questionTextUI.text = currentQ.questionText;

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
            ShowWin();
        }
    }

    public void OnAnswerSelected(int index)
    {
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {
            currentQuestionIndex++;
            LoadQuestion();
        }
        else
        {
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

        // Update Firebase
        UpdateQuestProgress();
    }

    void UpdateQuestProgress()
    {
        // Get current logged-in User ID
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user != null)
        {
            string userId = user.UserId;
            
            // get Database ref
            DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;

            // Update the "library_completed" field to true
            //Path: users > [userID] > quests > library_completed
            dbRef.Child("users").Child(userId).Child("quests").Child("library_completed").SetValueAsync(true)
                .ContinueWithOnMainThread(task => 
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Quest Progress Saved to Firebase!");
                    }
                });
        }
        else
        {
            Debug.LogError("No user logged in! Cannot save progress.");
        }
    }
}