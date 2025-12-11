using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using TMPro;
// A script to manage the Firebase authentication for sign up and log in, including error handling and database entry creation.
// Made by: Xander Foong and Lucas Tan

public class TestFirebase : MonoBehaviour
{

    // --- UI References ---
    // These link to the Input Fields in the Unity Inspector
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;

    [Header("UI References")]
    // Reference to the text component used for displaying errors to the user
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;
    
    
    
    // Reference to the database
    DatabaseReference mDatabaseRef;

    

    private void Start()
    {
        // Initialize the Database Reference when the script starts
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Used by UI Buttons to switch between Login and Signup scenes
    public void SignUp()
    {
        SceneManager.LoadScene("Authentication scene (sign up)");
    }

    public void SwitchToLogIn()
    {
        SceneManager.LoadScene("Authentication scene (log in)");
    }

    // Sign up logic
    public void SignUpAccount()
    {
        // Ensure fields are not empty before contacting server
        if (string.IsNullOrEmpty(EmailInput.text) || string.IsNullOrEmpty(PasswordInput.text))
        {
            ShowError("Please enter both email and password.");
            return;
        }
        // Create user asynchronously
        var signupTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        signupTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                
                Debug.Log("Can't sign up due to error!!!");
                return; // Stop here if it failed
            }
            // Error Handling: Parse Firebase specific error codes
            if (task.IsFaulted)
            {
                // Extract the specific error code
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                string message = "Sign Up Failed!";
                switch (errorCode)
                {
                    case AuthError.EmailAlreadyInUse:
                        message = "This email is already registered.";
                        break;
                    case AuthError.WeakPassword:
                        message = "Password is too weak (min 6 chars).";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid email format.";
                        break;
                    default:
                        message = "Error: " + firebaseEx.Message; // Fallback for unknown errors
                        break;
                }
                ShowError(message);
                return; // Stop here if it failed
            }

            // Success logic
            if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result.User;
                Debug.Log($"User signed up successfully, id: {newUser.UserId}");

                // Create the initial user data structure in the Realtime Database
                WriteNewUser(newUser.UserId, newUser.Email);
                // Feedback and transition to main menu
                ShowError("Success! Logging in...", Color.green);
                Invoke("GoToMainMenu", 1.0f);
            }
        });
    }
    // Creates the default JSON entry for a new user in the Database
    private void WriteNewUser(string userId, string email)
    {
        // Create a new User object with default quest data
        User user = new User(email);
        
        // Turn C# object into JSON
        string json = JsonUtility.ToJson(user);
        
        // Save it to: users -> [userID]
        mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    // Log in logic
    public void SignIn()
    {
        // 1. Validation
        if (string.IsNullOrEmpty(EmailInput.text) || string.IsNullOrEmpty(PasswordInput.text))
        {
            ShowError("Please fill in all fields.");
            return;
        }
        // Sign in
        var signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        // Handle sign in result
        signInTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowError("Sign in canceled.");
            }
            // Error Handling
            if (task.IsFaulted)
            {
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Login Failed!";
                switch (errorCode)
                {
                    case AuthError.WrongPassword:
                        message = "Incorrect Password.";
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist.";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email.";
                        break;
                    default:
                        message = "Error: " + firebaseEx.Message;
                        break;
                }
                ShowError(message);
                return;
            }

            if (task.IsCompleted)
            {
                Debug.Log($"User logged in, id: {task.Result.User.UserId}");
                GoToMainMenu();
            }
        });
    }
    // Dislays feedback text to user
    void ShowError(string message, Color color = default)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = (color == default) ? Color.red : color;
            
            // Clear message after 3 seconds so UI stays clean
            CancelInvoke("ClearError");
            Invoke("ClearError", 3f);
        }
    }

    void ClearError()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    
}



// --- DATA CLASSES ---
public class User
{
    public string email;
    public QuestData quests; // Nested object for quest status

    public int score;

    public float time_taken;

    // Constructor to initialize default values for a new user
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
    // booleans to track quest completion, false by default
    public bool library_completed = false;
    public bool food_completed = false;
    public bool services_completed = false;
}