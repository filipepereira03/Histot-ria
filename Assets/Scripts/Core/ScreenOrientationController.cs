using UnityEngine;

public class ScreenOrientationController : MonoBehaviour
{
    public void SetHorizontalOrientation()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft; // Define a orientação para horizontal
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false; // Desativa a rotação automática para retrato
        Screen.autorotateToPortraitUpsideDown = false;
    }

    public void SetPortraitOrientation()
    {
        Screen.orientation = ScreenOrientation.Portrait; // Define a orientação para vertical
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
    }
}
