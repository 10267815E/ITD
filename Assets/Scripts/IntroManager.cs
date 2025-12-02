using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject introPanel;   
    public GameObject hudPanel;     

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
}
