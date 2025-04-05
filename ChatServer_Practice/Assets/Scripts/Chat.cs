using UnityEngine;
using UnityEngine.UI;


public class Chat : MonoBehaviour
{
    private static Chat _intstance;
    public static Chat Instance { get { return _intstance; } }

    [SerializeField] private InputField _sendInput;
    [SerializeField] private RectTransform _chatContext;
}
