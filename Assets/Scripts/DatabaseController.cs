using UnityEngine;
using Firebase;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.SceneManagement;


public class TestFirebase : MonoBehaviour
{

    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    DatabaseReference mDatabaseRef;

    public void SignUp()
    {
        var signupTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(EmailInput.text, PasswordInput.text);
        signupTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("Can't sign up due to error!!!");
            }

            if (task.IsCompleted)
            {
                Debug.Log($"User signed up successfully, id: {task.Result.User.UserId}");

                // Code to create user profile in database
            }
        });
    }

    public class SceneChanger
    {
        public static void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
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

                // Code to load the user profile
                SceneManager.LoadScene("Main Scene");
            }
        });


    }

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created




}
