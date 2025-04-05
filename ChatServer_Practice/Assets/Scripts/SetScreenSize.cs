using UnityEngine;

public class SetScreenSize : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }
}
