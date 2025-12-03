using UnityEngine;

public class RobotBehavior : MonoBehaviour
{
    void OnEnable()
    {
        FaceCamera();
    }

    void FaceCamera()
    {
        Camera arCamera = Camera.main;
        if (arCamera != null)
        {
            Vector3 lookDirection = arCamera.transform.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

   
    public void TriggerQuizStart()
    {
        // 1. Find the IntroManager in the current scene
        IntroManager manager = Object.FindFirstObjectByType<IntroManager>();
        
        if (manager != null)
        {
            // 2. Tell the manager to start the quiz and destroy this robot
            manager.StartQuiz(this.gameObject);
        }
        else
        {
            Debug.LogError("IntroManager not found!");
        }
    }
}

