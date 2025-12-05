using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    [Header("UI References")]
    public TextMeshProUGUI welcomeText;
    public TextMeshProUGUI libraryStatusText;
    public TextMeshProUGUI foodStatusText;
    public TextMeshProUGUI servicesStatusText;

    private Color incompleteColor = Color.red;

    private Color completeColor = Color.green;

    
    public void ARController()
    {
        SceneManager.LoadScene("ARScene");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
