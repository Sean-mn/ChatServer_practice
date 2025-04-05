using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class : Client  
/// desc  : 서버와의 TCP 연결을 통해 채팅 기능을 수행하는 클라이언트 클래스
///
/// function:
/// ConnectToServer()         : 서버에 연결 시도
/// Update()                  : 수신 데이터 확인 및 처리
/// OnIncommingData(string)   : 수신된 메시지 처리
/// Send(string)              : 서버로 메시지 전송
/// OnSendBtn(InputField)     : 입력 필드에서 메시지를 보내는 버튼 기능
/// CloseSocket()             : 소켓 종료 및 정리
/// </summary>
public class Client : MonoBehaviour
{
    [SerializeField] private InputField _ipInput, _portInput, _nickNameInput;
    private string _clientName;

    private TcpClient _socket;
    private NetworkStream _stream;
    private StreamWriter _writer;
    private StreamReader _reader;

    private bool _isSocketReady = false;

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