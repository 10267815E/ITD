using UnityEngine;

public class RobotBehavior : MonoBehaviour
{
    void OnEnable()
    {
        FaceCamera();
        
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
