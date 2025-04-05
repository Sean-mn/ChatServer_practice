using UnityEngine;
using UnityEngine.UI;


public class Chat : MonoBehaviour
{
    private static Chat _instance;
    public static Chat Instance { get { return _instance; } }

    [SerializeField] private InputField _sendInput;
    [SerializeField] private RectTransform _chatContext;
    [SerializeField] private ScrollRect _chatScrollRect;
    [SerializeField] private Text _chatText;

    private void Awake()
    {
        _instance = this;
    }

    public void ShowMessage(string msg)
    {
        _chatText.text += _chatText.text == "" ? msg : "\n" + msg;

        Fit(_chatContext.GetComponent<RectTransform>());
        Fit(_chatContext);

        Invoke("ScrollDelay", 0.03f);
    }

    private void Fit(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    private void ScrollDelay()
    {
        _chatScrollRect.verticalScrollbar.value = 0;
    }
}
