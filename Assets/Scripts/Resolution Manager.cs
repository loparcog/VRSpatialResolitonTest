using UnityEngine;
using UnityEngine.XR;

public class ResolutionManager : MonoBehaviour
{
    void Start()
    {
        // Scale up the default resolution
        XRSettings.eyeTextureResolutionScale = 2f;   
    }
}
