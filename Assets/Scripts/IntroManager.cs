using UnityEngine;
using TMPro; // Needed for Text
using Firebase.Auth; // Needed for User ID
using Firebase.Database; // Needed for Database
using Firebase.Extensions;
using UnityEngine.SceneManagement;
// A script to manage the intro panel, HUD, quizzes, score, and timer.
// Made by: Xander Foong

public class IntroManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject introPanel;    // The initial panel where the robot explains the 'story'
    public GameObject hudPanel;     // The HUD panel showing "Start scanning" text
    
    [Header("Quiz Canvases")]
    // References to the specific quiz UIs for each location
    public GameObject libraryQuizScreen;   // Quiz screen for the library location
    public GameObject foodQuizScreen;      // Quiz screen for the food location
    public GameObject servicesQuizScreen;  // Quiz screen for the services location
    [Header("HUD Elements")]
    public TextMeshProUGUI progressText;    // Shows "1/3"
    public TextMeshProUGUI scoreText;     // Shows current score (0 at first)
    public TextMeshProUGUI timerText;      // Shows elapsed time in MM:SS format
    // --- Game State Variables ---
    public int currentScore = 0;
    public float currentTime = 0f;
    private bool isTimerRunning = false;



    void Start()
    {
        // Initial Scene Setup: Show Intro and hide HUD
        introPanel.SetActive(true); // Show the robot story
        hudPanel.SetActive(false);  // Hide the "Start scanning" prompt
     
        // Fetch initial data from Firebase
        UpdateProgressCounter(); // updates progress upon game start
        UpdateScoreUI();

    }

    void Update()
    {
        
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;
            
            // Format time as MM:SS
            int minutes = Mathf.FloorToInt(currentTime / 60F);
            int seconds = Mathf.FloorToInt(currentTime % 60F);
            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Called when the user clicks "Start Scanning" on the Intro Panel
    public void DismissIntro()
    {
        introPanel.SetActive(false); // Hide story
        hudPanel.SetActive(true);    // Show "Start scanning" text
        isTimerRunning = true; // Only start timer after the game starts
        
    }

    public void ModifyScore(int amount) // Score management
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
    }
 
    // Called by the robot when the user taps "Start Quiz"
    public void StartQuiz(GameObject spawnedRobot, int locationID) 
    {
      Destroy(spawnedRobot); // Remove robot
      // Activate the correct Quiz Canvas based on the location ID set in the Robot Prefab 
      switch (locationID)
      {
        case 1:
            libraryQuizScreen.SetActive(true);
            break;
        case 2:
            foodQuizScreen.SetActive(true);
            break;
        case 3:
            servicesQuizScreen.SetActive(true);
            break;
        default:
            Debug.LogError("Invalid location ID: " + locationID);
            break;
      }

      
    }
    // Called when the user finishes a quiz and clicks "Return"
    public void ReturnToScanningMode()
    {
        // Ensure all quiz screens are closed
        if (libraryQuizScreen != null) libraryQuizScreen.SetActive(false);
        if (foodQuizScreen != null) foodQuizScreen.SetActive(false);
        if (servicesQuizScreen != null) servicesQuizScreen.SetActive(false);
        // Re-enable the scanning HUD
        if (hudPanel != null) hudPanel.SetActive(true);

        UpdateProgressCounter(); // Refresh progress from Database to see if we reached 3/3
    }

    void UpdateProgressCounter()
    {
        if (progressText == null) return;

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            
            // Fetch the user's quest data from the quests node
            dbRef.Child("users").Child(user.UserId).Child("quests").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        // Count how many quests are completed
                        int count = 0;
                        if (IsQuestComplete(snapshot.Child("library_completed"))) count++;
                        if (IsQuestComplete(snapshot.Child("food_completed"))) count++;
                        if (IsQuestComplete(snapshot.Child("services_completed"))) count++;

                        // Update the text
                        progressText.text = $"Scanned images: {count}/3";

                        if (count >= 3) // if count is above 3 (all quests done), trigger finale helper fnction
                        {
                            Invoke ("TriggerFinale", 4f); // Delay to allow player to see 3/3 score
                        }
                    }
                    else
                    {
                        progressText.text = "Scanned images: 0/3";
                    }
                }
            });
        }
    }

    bool IsQuestComplete(DataSnapshot s) // Helper function to parse boolean from DataSnapshot
    {
        if (s.Exists && s.Value != null)
        {
            return bool.Parse(s.Value.ToString());
        }
        return false;
    }

    void TriggerFinale()
    {
        SceneManager.LoadScene("FinaleScene");
    }
}
