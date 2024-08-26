using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private Socket udpClientSocket;
    private Thread clientThread;
    private bool isRunning = false;
    public string serverIP = "192.168.0.114"; // Replace with your server's IP address
    public int serverPort = 3001;
    private int localPort = 3000; // Local port for the client to receive messages

    void Start()
    {
        StartClient();
        SendMessageToServer("Hii");
    }

    void StartClient()
    {
        try
        {
            // Initialize the UDP socket
            udpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Bind to a local port to receive messages
            udpClientSocket.Bind(new IPEndPoint(IPAddress.Any, localPort));

            isRunning = true;
            clientThread = new Thread(new ThreadStart(ClientThread));
            clientThread.IsBackground = true;
            clientThread.Start();
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e.Message);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SendMessageToServer("Hii");
        }
    }

    void ClientThread()
    {
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] buffer = new byte[1024];

        try
        {
            while (isRunning)
            {
                if (udpClientSocket.Available > 0)
                {
                    int receivedBytes = udpClientSocket.ReceiveFrom(buffer, ref remoteEndPoint);
                    string receivedText = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    Debug.Log("Received from Server: " + receivedText);
                }
            }
        }
        catch (SocketException e)
        {
            if (isRunning)
            {
                Debug.LogError("SocketException: " + e.Message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
    }

    public void SendMessageToServer(string message)
    {
        try
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            udpClientSocket.SendTo(messageBytes, serverEndPoint);
            Debug.Log("Sent to Server: " + message);
        }
        catch (SocketException e)
        {
            Debug.LogError("SocketException: " + e.Message);
        }
    }

    public void StopClient()
    {
        isRunning = false;

        if (udpClientSocket != null)
        {
            udpClientSocket.Close();
            udpClientSocket = null;
        }

        if (clientThread != null)
        {
            clientThread.Abort();
            clientThread = null;
        }
    }

    void OnApplicationQuit()
    {
        StopClient();
    }
}
