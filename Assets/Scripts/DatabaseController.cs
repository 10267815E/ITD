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

    [Header("UI References")]
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

        if (string.IsNullOrEmpty(EmailInput.text) || string.IsNullOrEmpty(PasswordInput.text))
        {
            ShowError("Please enter both email and password.");
            return;
        }
        var signupTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        signupTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                
                Debug.Log("Can't sign up due to error!!!");
                return; // Stop here if it failed
            }
            if (task.IsFaulted)
            {
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
                        message = "Error: " + firebaseEx.Message; // Fallback
                        break;
                }
                ShowError(message);
                return; // Stop here if it failed
            }

            if (task.IsCompleted)
            {
                FirebaseUser newUser = task.Result.User;
                Debug.Log($"User signed up successfully, id: {newUser.UserId}");

                //  Create the database entry ---
                WriteNewUser(newUser.UserId, newUser.Email);

                ShowError("Success! Logging in...", Color.green);
                Invoke("GoToMainMenu", 1.0f);
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

        if (string.IsNullOrEmpty(EmailInput.text) || string.IsNullOrEmpty(PasswordInput.text))
        {
            ShowError("Please fill in all fields.");
            return;
        }
        var signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        signInTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                ShowError("Sign in canceled.");
            }

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
                SceneManager.LoadScene("MainMenu");
            }
        });
    }

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