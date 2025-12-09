using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using TMPro;

public class TestFirebase : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    
    
    
    // Reference to the database
    DatabaseReference mDatabaseRef;

    private void Start()
    {
        // Initialize the Database Reference when the script starts
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SignUp()
    {
        SceneManager.LoadScene("Authentication scene (sign up)");
    }

    public void SwitchToLogIn()
    {
        SceneManager.LoadScene("Authentication scene (log in)");
    }

    public void SignUpAccount()
    {
        var signupTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        signupTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Can't sign up due to error!!!");
                return; // Stop here if it failed
            }

            if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result.User;
                Debug.Log($"User signed up successfully, id: {newUser.UserId}");

                //  Create the database entry ---
                WriteNewUser(newUser.UserId, newUser.Email);
            }
        });
    }

    private void WriteNewUser(string userId, string email)
    {
        // Create a new User object with default quest data
        User user = new User(email);
        
        // Turn it into JSON
        string json = JsonUtility.ToJson(user);

        // Save it to: users -> [userID]
        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    public void SignIn()
    {
        var signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        signInTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Can't sign in due to error!!!");
            }

            if (task.IsCompleted)
            {
                Debug.Log($"User logged in, id: {task.Result.User.UserId}");
                SceneManager.LoadScene("MainMenu");
            }
        });
    }
    
    
}


public class User
{
    public string email;
    public QuestData quests;

    public int score;

    public float time_taken;

    public User(string email)
    {
        this.email = email;
        this.quests = new QuestData(); // Creates default false values
        this.score = 0;
        this.time_taken = 0f;
    }
}

public class QuestData
{
    public bool library_completed = false;
    public bool food_completed = false;
    public bool services_completed = false;
}