using UnityEngine;
using TMPro; // Needed for Text
using Firebase.Auth; // Needed for User ID
using Firebase.Database; // Needed for Database
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject introPanel;   
    public GameObject hudPanel;  

    public GameObject libraryQuizScreen;   
    public GameObject foodQuizScreen;
    public GameObject servicesQuizScreen;

    [Header("HUD Elements")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    public int currentScore = 0;
    public float currentTime = 0f;
    private bool isTimerRunning = false;



    void Start()
    {
        // Ensure the correct state when the scene loads
        introPanel.SetActive(true); // Show the robot story
        hudPanel.SetActive(false);  // Hide the "Search" prompt

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


    public void DismissIntro()
    {
        introPanel.SetActive(false); // Hide story
        hudPanel.SetActive(true);    // Show "Search for poster" text
        isTimerRunning = true; // Only start timer after the game starts
        
    }

    public void ModifyScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
    }

    public void StartQuiz(GameObject spawnedRobot, int locationID) 
    {
      Destroy(spawnedRobot); // Remove robot
       
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

    public void ReturnToScanningMode()
    {
        
        if (libraryQuizScreen != null) libraryQuizScreen.SetActive(false);
        if (foodQuizScreen != null) foodQuizScreen.SetActive(false);
        if (servicesQuizScreen != null) servicesQuizScreen.SetActive(false);

        if (hudPanel != null) hudPanel.SetActive(true);

        UpdateProgressCounter();
    }

    void UpdateProgressCounter()
    {
        if (progressText == null) return;

        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            
            // Fetch the user's quest data
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
                            Invoke ("TriggerFinale", 4f); // Delay to allow player to see 3/3
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

    bool IsQuestComplete(DataSnapshot s)
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
