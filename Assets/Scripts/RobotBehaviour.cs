using UnityEngine;

public class RobotBehavior : MonoBehaviour
{
    
    [SerializeField] private GameObject quizPromptCanvas;

    void OnEnable()
    {
        // robot faces user
        FaceCamera();

        // force canvas to appear
        if (quizPromptCanvas != null)
        {
            quizPromptCanvas.SetActive(true);
        }
        
    }

    void FaceCamera()
    {
        Camera arCamera = Camera.main;
        if (arCamera != null)
        {
            Vector3 lookDirection = arCamera.transform.position - transform.position;
            lookDirection.y = 0; // Keep the robot upright
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}

