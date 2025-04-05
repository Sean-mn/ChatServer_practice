using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    [SerializeField] private InputField _ipInput, _portInput, _nickNameInput;
    private string _clientName;

    private TcpClient _socket;
    private NetworkStream _stream;
    private StreamWriter _writer;
    private StreamReader _reader;

    private bool _isSocketReady = false;

    private void Start() => Screen.SetResolution(1980, 1080, FullScreenMode.Windowed);
    public void ConnectToServer()
    {
        if (_isSocketReady) return;

        string ip = _ipInput.text == "" ? "127.0.0.1" : _ipInput.text;
        int port = _portInput.text == "" ? 7777 : int.Parse(_portInput.text);

        // 소켓 생성
        try
        {
            _socket = new TcpClient(ip, port);
            _stream = _socket.GetStream();
            _writer = new StreamWriter(_stream);
            _reader = new StreamReader(_stream);
            _isSocketReady = true;
        }
        catch (Exception e)
        {
            Chat.Instance.ShowMessage($"Socket Error : {e.Message}");
        }
    }

    private void Update()
    {
        if (_isSocketReady && _stream.DataAvailable)
        {
            string data = _reader.ReadLine();
            if (data != null)
            {
                OnIncommingData(data);
            }
        }
    }

    private void OnIncommingData(string data)
    {
        if (data == "%NAME")
        {
            _clientName = _nickNameInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : _nickNameInput.text;
            Send($"&NAME|{_clientName}");
            return;
        }

        Chat.Instance.ShowMessage(data);
    }

    private void Send(string data)
    {
        if (!_isSocketReady) return;

        _writer.WriteLine(data);
        _writer.Flush();
    }

    public void OnSendBtn(InputField sendInput)
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        if (!Input.GetButtonDown("Submit")) return;
        sendInput.ActivateInputField();
#endif
        if (sendInput.text.Trim() == "") return;

        string msg = sendInput.text;
        sendInput.text = "";
        Send(msg);
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void CloseSocket()
    {
        if (!_isSocketReady) return;

        _writer.Close();
        _reader.Close();
        _socket.Close();
        _isSocketReady = false;
    }
}