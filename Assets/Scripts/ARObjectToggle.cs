using UnityEngine;
// script not used in assignment but kept for potential future use
public class ARObjectToggle : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRendererToToggle;

    public void ToggleObject()
    {
        meshRendererToToggle.enabled = !meshRendererToToggle.enabled;
    }
}
