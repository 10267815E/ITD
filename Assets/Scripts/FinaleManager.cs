using UnityEngine;
using UnityEngine.SceneManagement;
// A script to link the finale scene button to go back to the main menu.
// Made by: Xander Foong  

public class FinaleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoHome()
    {
        SceneManager.LoadScene("MainMenu"); // Load the MainMenu scene
    }
}
