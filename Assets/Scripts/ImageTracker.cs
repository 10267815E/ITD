using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void Start()
    {
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
        if(trackedImage != null)
        {
            if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
            {
                //Disable the associated content
                spawnedPrefabs[trackedImage.referenceImage.name].transform.SetParent(null);
                spawnedPrefabs[trackedImage.referenceImage.name].SetActive(false);
            }
            else if (trackedImage.trackingState == TrackingState.Tracking)
            {
                Debug.Log("Detected image: " + trackedImage.referenceImage.name);
                Debug.Log(trackedImage.gameObject.name + " is being tracked.");
                //Enable the associated content
                if(spawnedPrefabs[trackedImage.referenceImage.name].transform.parent != trackedImage.transform)
                {
                    var robot = spawnedPrefabs[trackedImage.referenceImage.name];
                    robot.transform.SetParent(trackedImage.transform);
                    robot.transform.localPosition = new Vector3(0f, 0.1f, 0f); // lift slightly above the image
                    robot.transform.localRotation = Quaternion.Euler(-90f, 180f, 0f); // make it upright and face user
                    robot.transform.localScale = Vector3.one * 0.2f; 
                    robot.SetActive(true);

                }
            }
        }
    }
}

