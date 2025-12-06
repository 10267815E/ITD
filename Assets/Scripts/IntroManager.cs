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
    }
}
