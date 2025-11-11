using UnityEngine;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Extensions;
//using Unity.VisualScripting;
using TMPro;
using Firebase.Auth;
using System;

public class Crud : MonoBehaviour
{
    DatabaseReference db; 

    void Start()
    {
        // Initialize Firebase Database reference when the script starts
        db = FirebaseDatabase.DefaultInstance.RootReference;

        // Create a new inventory for the user "Player01"
        WriteNewInventory("Player01", "Apple", "Bread", "Cheese", "FishingRod", "Dagger");
    }

    void Update()
    {

    }

    // Creates a new inventory record
    private void WriteNewInventory(string userId, string invSlot1, string invSlot2, string invSlot3, string offHand, string mainHand)
    {
        // Create a new Inventory object with the given slots
        Inventory inventory = new Inventory(invSlot1, invSlot2, invSlot3, offHand, mainHand);

        // Convert the inventory object into a JSON string
        string json = JsonUtility.ToJson(inventory);

        // Save the JSON string under "Players/userId"
        db.Child("Players").Child(userId).SetRawJsonValueAsync(json);
    }

    // Retrieves the inventory of a user 
    public void RetrieveInventory(string userId)
    {
        db.Child("Players").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error retrieving data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Retrieved Data: " + snapshot.GetRawJsonValue());
            }
        });
    }

    // Updates specific inventory slots (main hand and off hand)
    public void UpdateInventory(string userId, string newMainHand, string newOffhand)
    {
        // Dictionary to store only the fields we want to update
        Dictionary<string, object> updates = new Dictionary<string, object>();
        updates["mainHand"] = newMainHand;
        updates["offHand"] = newOffhand;

        // Send the updates to Firebase
        db.Child("Players").Child(userId).UpdateChildrenAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Inventory updated successfully!");
            }
            else
            {
                Debug.LogError("Update failed: " + task.Exception);
            }
        });
    }

    // This method can be linked to a UI button to trigger an update
    public void OnUpdateButton()
    {
        // Changes main hand and off hand items for user "Player01"
        UpdateInventory("Player01", "Knife", "Spoon");
    }

    // Deletes a specific user's inventory 
    public void DeleteInventory(string userId)
    {
        db.Child("Players").Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Inventory deleted");
            }
            else
            {
                Debug.LogError("Delete failed: " + task.Exception);
            }
        });
    }
}
