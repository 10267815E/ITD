using UnityEngine;

public class RobotBehavior : MonoBehaviour
{
    void OnEnable()
    {
        FaceCamera();

        QuizManager quizManager = FindFirstObjectByType<QuizManager>();
        if(quizManager != null)
        {
            quizManager.StartQuiz();
            Debug.Log("Quiz started");
        }
        
    }

    void FaceCamera()
    {
        Camera arCamera = Camera.main; // AR Camera
        if (arCamera != null)
        {
            // Rotate the robot to face the camera
            Vector3 lookDirection = arCamera.transform.position - transform.position;
            lookDirection.y = 0; // Keep robot upright
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
