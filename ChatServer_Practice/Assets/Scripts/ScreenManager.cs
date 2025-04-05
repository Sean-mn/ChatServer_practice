using UnityEngine;

/// <summary>
/// 
/// class : SetScreenSize
/// desc  : 해상도 및 화면 모드 초기 설정
/// 
/// function:
/// SetScreenSize(int width, int height, FullScreenMode screenMode): 화면 크기 설정 및 모드 설정
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
        Screen.SetResolution(width, height, screenMode);
    }
}
