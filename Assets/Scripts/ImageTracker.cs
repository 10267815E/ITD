using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



public class ImageTracker : MonoBehaviour

{
    private QuizManager quizManager;
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void Start()
    {

        quizManager = FindFirstObjectByType<QuizManager>();

        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
            SetupPrefabs();
        }
    }

    void SetupPrefabs()
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newPrefab.name = prefab.name;
            newPrefab.SetActive(false);
            spawnedPrefabs.Add(prefab.name, newPrefab);
        }
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        foreach (KeyValuePair<TrackableId, ARTrackedImage> lostObj in eventArgs.removed)
        {
            UpdateImage(lostObj.Value);
        }
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
     if (trackedImage == null || quizManager == null)
        return;

     string imageName = trackedImage.referenceImage.name;

    if (imageName == "book_logo")
      {
         if (trackedImage.trackingState == TrackingState.Tracking)
         {
            GameObject robot = spawnedPrefabs[imageName];
            robot.SetActive(true);
            robot.transform.SetParent(trackedImage.transform);
            robot.transform.localPosition = Vector3.zero;
            robot.transform.localRotation = Quaternion.identity;

            quizManager.StartQuiz();
            Debug.Log("Quiz started because image detected");
         }
         else
         {
            spawnedPrefabs[imageName].SetActive(false);
            quizManager.quizBackground.SetActive(false);
            Debug.Log("Quiz hidden because image lost");
         }
      }
    }

}

