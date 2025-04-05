using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}

public class Server : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private InputField _portInput;

    [Header("clients")]
    private List<ServerClient> _clients;
    private List<ServerClient> _disconnetedClients;

    private TcpListener _server;
    private bool _isServerStarted = false;

    public void CreateServer()
    {
        _clients = new List<ServerClient>();
        _disconnetedClients = new List<ServerClient>();

        try
        {
            int port = _portInput.text == "" ? 7777 : int.Parse(_portInput.text);
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();

            StartListening();
            _isServerStarted = true;
            Chat.Instance.ShowMessage($"{port}에서 서버가 시작했습니다.");
        }
        catch (Exception e)
        {
            Chat.Instance.ShowMessage($"Socket Error : {e.Message}");
        }
    }

    private void Update()
    {
        if (!_isServerStarted) return;

        foreach (ServerClient client in _clients)
        {
            // 클라이언트 연결 유무
            if (!IsServerConnected(client.tcp))
            {
                client.tcp.Close();
                _disconnetedClients.Add(client);
                continue;
            }
            // 클라이언트에서 확인 후 메세지 받기
            else
            {
                NetworkStream stream = client.tcp.GetStream();

                if (stream.DataAvailable)
                {
                    string data = new StreamReader(stream, true).ReadLine();
                    if (data != null)
                    {
                        OnIncommingData(client, data);
                    }    
                }
            }
        }

        for (int i = 0; i < _disconnetedClients.Count - 1; i++)
        {
            Broadcast($"{_disconnetedClients[i].clientName}의 연결이 끊겼습니다.", _clients);

            _clients.Remove(_disconnetedClients[i]);
            _disconnetedClients.RemoveAt(i);
        }
    }

    private void StartListening()
    {
        _server.BeginAcceptTcpClient(AcceptTcpClient, _server);
    }

    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        _clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        // 메시지를 연결된 모두에게 보냄
        Broadcast("%NAME", new List<ServerClient>() { _clients[_clients.Count - 1] });
    }

    private void OnIncommingData(ServerClient client, string data)
    {
        if (data.Contains("&NAME"))
        {
            client.clientName = data.Split('|')[1];
            Broadcast($"{client.clientName}이 연결되었습니다.", _clients);
            return;
        }

        Broadcast($"{client.clientName} : {data}", _clients);
    }

    private void Broadcast(string data, List<ServerClient> clients)
    {
        foreach (var client in clients)
        {
            try
            {
                StreamWriter writer = new StreamWriter(client.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Chat.Instance.ShowMessage($"쓰기 에러 : {e.Message}를 클라이언트에게 {client.clientName}");
            }
        }
    }

    private bool IsServerConnected(TcpClient client)
    {
        try
        {
            if (client != null && client.Client != null && client.Client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    
}
