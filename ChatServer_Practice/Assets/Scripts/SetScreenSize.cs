using UnityEngine;

public class SetScreenSize : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1980, 1080, FullScreenMode.Windowed);
    }
}
