using UnityEngine;
using System.Collections;

public class SceneOrientationManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SetLandscapeOrientation());
    }

    IEnumerator SetLandscapeOrientation()
    {
        yield return new WaitForSeconds(0.1f);  // Wait briefly to ensure the scene loads
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void OnDisable()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;  // Revert to default when exiting
    }
}
