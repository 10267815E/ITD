using UnityEngine;
// A script to control the robot's behaviour upon spawning in the AR scene. Key features include facing the camera and triggering quizzes.
// Made by: Xander Foong
public class RobotBehavior : MonoBehaviour
{

    [Header("Robot Identity")]
    public int locationID;
    void OnEnable()
    {
        FaceCamera(); // The moment the robot is enabled, it faces the camera
    }

    void FaceCamera()
    {
        Camera arCamera = Camera.main; // Get the main AR camera from the scene
        if (arCamera != null)
        {
            Vector3 lookDirection = arCamera.transform.position - transform.position; // Calculate direction to camera
            lookDirection.y = 0; // Keep the robot upright by ignoring vertical difference
            transform.rotation = Quaternion.LookRotation(lookDirection); // Rotate to face the camera
        }
    }

   
    public void TriggerQuizStart() // function to be called when the robot is interacted with
    {
        // 1. Find the IntroManager in the current scene
        IntroManager manager = Object.FindFirstObjectByType<IntroManager>();
        
        if (manager != null)
        {
            // 2. Tell the manager to start the quiz and destroy this robot
            manager.StartQuiz(this.gameObject, locationID);
        }
        else
        {
            Debug.LogError("IntroManager not found!"); // Log an error if the manager is not found
        }
    }
}

