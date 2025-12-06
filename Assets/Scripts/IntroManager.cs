using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject introPanel;   
    public GameObject hudPanel;  

    public GameObject libraryQuizScreen;   
    public GameObject foodQuizScreen;
    public GameObject servicesQuizScreen;

    void Start()
    {
        // Ensure the correct state when the scene loads
        introPanel.SetActive(true); // Show the robot story
        hudPanel.SetActive(false);  // Hide the "Search" prompt
    }

    
    public void DismissIntro()
    {
        introPanel.SetActive(false); // Hide story
        hudPanel.SetActive(true);    // Show "Search for poster" text
        
        
    }

    public void StartQuiz(GameObject spawnedRobot, int locationID) 
    {
      Destroy(spawnedRobot); // Remove robot
      libraryQuizScreen.SetActive(true); // Turns on the Canvas, triggering QuizManager.Start()
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

      if(hudPanel != null) hudPanel.SetActive(false);
    }

    public void ReturnToScanningMode()
    {
        if (libraryQuizScreen != null)
        {
            libraryQuizScreen.SetActive(false); // Hide the quiz UI
        }

        if (hudPanel != null)
        {
            hudPanel.SetActive(true); // Show the "Search for poster" text
        }
    }
}
