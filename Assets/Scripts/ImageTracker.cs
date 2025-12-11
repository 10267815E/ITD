using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
// A script to manage image tracking in the AR scene, spawning and updating AR content based on tracked images.
// Original from Brightspace. Modified by: Xander Foong
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
            GameObject spawnedPrefab = spawnedPrefabs[trackedImage.referenceImage.name];
            
            if (trackedImage.trackingState == TrackingState.Limited || trackedImage.trackingState == TrackingState.None)
            {
                //Disable the associated content
                spawnedPrefab.SetActive(false);
                
                // When tracking is lost, disable the Quiz Prompt as well
                ToggleQuizPrompt(spawnedPrefab, false);
            }
            else if (trackedImage.trackingState == TrackingState.Tracking)
            {
                // 1. Enable the associated content (Robot)
                spawnedPrefab.transform.position = trackedImage.transform.position;
                spawnedPrefab.transform.rotation = trackedImage.transform.rotation;
                spawnedPrefab.SetActive(true);

                
                // The canvas only appears when the image is clearly tracked.
                ToggleQuizPrompt(spawnedPrefab, true);
            }
        }
    }
    
   
    void ToggleQuizPrompt(GameObject parentObject, bool shouldActivate)
    {
        // Find the child canvas 
        
        Transform promptCanvas = parentObject.transform.Find("QuizPrompt");
        
        if (promptCanvas != null)
        {
            // Activate the canvas component (which contains the text and button)
            promptCanvas.gameObject.SetActive(shouldActivate);
            
            Debug.Log("Quiz Prompt toggled: " + shouldActivate);
            
        }
    }
}
