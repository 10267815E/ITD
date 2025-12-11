using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions; 
// A script to manage the quiz functionality, including question display, answer checking, score updating, and Firebase integration. Also includes SFX.
// Made by: Xander Foong and Lucas Tan
public class QuizManager : MonoBehaviour
{
    [Header("Database Settings")] // Used to specify which quest to update in Firebase (e.g., "library_completed")
    public string questDatabaseName;

    [System.Serializable] // Defines the structure for a single question object in the Inspector
    public class QuestionData
    {
        [TextArea] public string questionText;
        public string[] answers;  // Array of possible answers shown on buttons
        public int correctAnswerIndex;  // Index of the correct answer in the answers array
    }

    public QuestionData[] questions;  // Array to hold all questions for this specific quiz (e.g., Library Quiz)

    [Header("UI Elements")]
    public TextMeshProUGUI questionTextUI; // Reference to the main question text display
    public TextMeshProUGUI[] answerButtonTexts; // Array of TextMeshPro components on the answer buttons
    
    [Header("Panels")]
    public GameObject tryAgainPanel; // Panel shown when the player answers incorrectly
    public GameObject winPanel; // Panel shown when the player completes the quiz
    public GameObject questionPanel; // Panel containing the current question and answers

    [Header("Audio Settings")]
    public AudioSource sfxPlayer; // Audio source for playing sound effects
    public AudioClip correctSound; // Sound played on correct answer
    public AudioClip wrongSound; // Sound played on wrong answer

    private int currentQuestionIndex = 0;

    
    void OnEnable() 
    {
        // Resets the index to 0 every time this canvas appears
        currentQuestionIndex = 0; 
        
        // Load the first question
        LoadQuestion();
        
        // Reset panels to default state
        if(tryAgainPanel != null) tryAgainPanel.SetActive(false);
        if(winPanel != null) winPanel.SetActive(false);
        if(questionPanel != null) questionPanel.SetActive(true);
    }

    void LoadQuestion() // Handles displaying the current question and populating the answer buttons
    {
        if (currentQuestionIndex < questions.Length) // Check if there are more questions to display
        {
            QuestionData currentQ = questions[currentQuestionIndex]; // Display question text
            questionTextUI.text = currentQ.questionText;
            // Display answer options on buttons
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
            ShowWin(); // All questions answered correctly, tirgger win
        }
    }

    public void OnAnswerSelected(int index) // Called by the button click event (passes the button's index)
    {

        IntroManager manager = FindFirstObjectByType<IntroManager>(); // Find IntroManager in scene so that it can update score variable
        if (index == questions[currentQuestionIndex].correctAnswerIndex) // Check if the selected answer index matches the correct answer index
        {

            if (sfxPlayer != null && correctSound != null)
            {
                sfxPlayer.PlayOneShot(correctSound); // Play correct answer sound
            }

            if (manager != null) manager.ModifyScore(1); // Award 1 point for correct answer
            
            currentQuestionIndex++; // Move to next qn
            LoadQuestion();
        }
        else
        {

            if (sfxPlayer != null && wrongSound != null)
            {
                sfxPlayer.PlayOneShot(wrongSound);
            }
            if (manager != null) manager.ModifyScore(-1); // Deduct 1 point for wrong answer
            tryAgainPanel.SetActive(true); // Show try again panel 
        }
    }

    public void CloseTryAgain() // Called by the "Try Again" button to dismiss the feedback panel
    {
        tryAgainPanel.SetActive(false);
    }

    void ShowWin()
    {
        questionPanel.SetActive(false);
        winPanel.SetActive(true);

        // Update the completion status, score, and time to the database
        UpdateQuestProgress();
    }

    void UpdateQuestProgress() // Writes the user's progress and current score/time to Firebase
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

            IntroManager manager = FindFirstObjectByType<IntroManager>(); // Save the updated Score and Time (taken from IntroManager)
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