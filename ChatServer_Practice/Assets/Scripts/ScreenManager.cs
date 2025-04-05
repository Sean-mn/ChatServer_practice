using UnityEngine;

/// <summary>
/// 
/// class : SetScreenSize
/// 
/// 
/// 
/// </summary>
public class ScreenManager : MonoBehaviour
{
    private void Start()
    {
        SetScreenSize(1280, 720, FullScreenMode.Windowed);
    }

    public void SetScreenSize(int width, int height, FullScreenMode screenMode)
    {
        Screen.SetResolution(width, width, screenMode);
    }
}
