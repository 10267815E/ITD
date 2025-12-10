using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions; 

public class QuizManager : MonoBehaviour
{
    [Header("Database Settings")]
    public string questDatabaseName;

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

    [Header("Audio Settings")]
    public AudioSource sfxPlayer;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    private int currentQuestionIndex = 0;

    
    void OnEnable() 
    {
        // Resets the index to 0 every time this canvas appears
        currentQuestionIndex = 0; 
        
        // Load the first question
        LoadQuestion();
        
        // Reset panels
        if(tryAgainPanel != null) tryAgainPanel.SetActive(false);
        if(winPanel != null) winPanel.SetActive(false);
        if(questionPanel != null) questionPanel.SetActive(true);
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

        IntroManager manager = FindFirstObjectByType<IntroManager>(); // Find IntroManager in scene 
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {

            if (sfxPlayer != null && correctSound != null)
            {
                sfxPlayer.PlayOneShot(correctSound);
            }

            if (manager != null) manager.ModifyScore(1); // Award 1 point for correct answer
            
            currentQuestionIndex++;
            LoadQuestion();
        }
        else
        {

            if (sfxPlayer != null && wrongSound != null)
            {
                sfxPlayer.PlayOneShot(wrongSound);
            }
            if (manager != null) manager.ModifyScore(-1); // Deduct 1 point for wrong answer
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

            
            //Path: users > [userID] > quests > library_completed
            dbRef.Child("users").Child(user.UserId).Child("quests").Child(questDatabaseName).SetValueAsync(true)
                .ContinueWithOnMainThread(task => 
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Quest Progress Saved to Firebase!");
                    }
                });

            IntroManager manager = FindFirstObjectByType<IntroManager>();
            if (manager != null)
            {
                // Save Score
                dbRef.Child("users").Child(user.UserId).Child("score").SetValueAsync(manager.currentScore);
                // Save Time Taken
                dbRef.Child("users").Child(user.UserId).Child("time_taken").SetValueAsync(manager.currentTime);
            }
        }
        else
        {
            Debug.LogError("No user logged in! Cannot save progress.");
        }
    }
}