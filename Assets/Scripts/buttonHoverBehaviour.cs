using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
        cb.selectedColor = wantedColor;
        button.colors = cb;
    }

    public void ChangeWhenHoverLeaves()
    {
        cb.selectedColor = originalColor;
        button.colors = cb;
    }
}