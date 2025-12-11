using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using Firebase.Auth;     
using Firebase.Database; 
using Firebase.Extensions; 
// A script to manage the main menu UI, fetching user quest completion status from Firebase and updating the display.
// Made by: Xander Foong

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI welcomeText; 
    public TextMeshProUGUI libraryStatusText; 
    public TextMeshProUGUI foodStatusText;    
    public TextMeshProUGUI servicesStatusText; 

    // Colors for visual feedback
    private Color incompleteColor = Color.red;
    private Color completeColor = Color.green;

    void Start()
    {
        // 1. Check if we have a user logged in
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user != null)
        {
            

            // 2. Go fetch their quest data
            FetchQuestProgress(user.UserId);
        }
        else
        {
            Debug.LogError("No user found! Redirecting to login...");
            SceneManager.LoadScene("Authentication scene (log in)");
        }
    }

    void FetchQuestProgress(string userId)
    {
        // Get the reference to the database
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
        // Path: users -> [userID] -> quests
        dbRef.Child("users").Child(userId).Child("quests").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load quest data.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists)
                {
                    // 3. Extract the data safely
                    // Check if child exists then parse data
                    bool libDone = GetBoolFromSnapshot(snapshot.Child("library_completed"));
                    bool foodDone = GetBoolFromSnapshot(snapshot.Child("food_completed"));
                    bool servicesDone = GetBoolFromSnapshot(snapshot.Child("services_completed"));

                    // 4. Update the visual UI
                    UpdateUI(libDone, foodDone, servicesDone);
                }
                else
                {
                    // If no data exists yet, all quests marked incomplete
                    UpdateUI(false, false, false);
                }
            }
        });
    }

    // Helper function to safely read boolean values from Firebase
    bool GetBoolFromSnapshot(DataSnapshot s)
    {
        if (s.Exists && s.Value != null)
        {
            // Firebase stores as Object, convert to string then Parse
            return bool.Parse(s.Value.ToString());
        }
        return false;
    }

    void UpdateUI(bool lib, bool food, bool services)
    {
        SetStatusStyle(libraryStatusText, lib);
        SetStatusStyle(foodStatusText, food);
        SetStatusStyle(servicesStatusText, services);
    }

    // Helper to change text and color
    void SetStatusStyle(TextMeshProUGUI textObj, bool isComplete)
    {
        if (textObj == null) return;

        if (isComplete) // Controls the text colour and message
        {
            textObj.text = "Status: Completed";
            textObj.color = completeColor;
        }
        else
        {
            textObj.text = "Status: Incomplete";
            textObj.color = incompleteColor;
        }
    }

    public void ARController() // Connected to the 'Open AR Camera' button
    {
        SceneManager.LoadScene("ARScene");
    }
}
