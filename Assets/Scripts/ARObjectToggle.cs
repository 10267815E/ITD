using UnityEngine;

public class ARObjectToggle : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRendererToToggle;

    public void ToggleObject()
    {
        meshRendererToToggle.enabled = !meshRendererToToggle.enabled;
    }
}
