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
}

