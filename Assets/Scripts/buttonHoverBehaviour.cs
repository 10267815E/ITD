using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// This script changes the button color when hovered over and reverts it back when the hover ends.
// Made by: Lucas Tan

public class buttonHoverBehaviour : MonoBehaviour
{
    public Button button;
    public Color wantedColor;
    private Color originalColor;
    private ColorBlock cb;

    void Start()
    {
        cb = button.colors;
        originalColor = cb.normalColor;
    }

    void Update()
    {
        
    }

    public void ChangeWhenHover()
    {
        cb.selectedColor = wantedColor; // Change the selected color to the wanted color
        button.colors = cb;
    }

    public void ChangeWhenHoverLeaves()
    {
        cb.selectedColor = originalColor; // Revert the selected color back to the original color
        button.colors = cb;
    }
}