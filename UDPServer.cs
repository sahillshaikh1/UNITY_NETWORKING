using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private Socket udpServerSocket;
    private Thread serverThread;
    private bool isRunning = false;
    public int port = 3001;

    void Start()
    {
        StartServer();
    }

    void StartServer()
    {
        udpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udpServerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        serverThread = new Thread(new ThreadStart(ServerThread));
        isRunning = true;
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    void ServerThread()
    {
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] buffer = new byte[1024];

        while (isRunning)
        {
            try
            {
                int receivedBytes = udpServerSocket.ReceiveFrom(buffer, ref remoteEndPoint);
                string receivedText = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                Debug.Log("Received: " + receivedText);

                if (receivedText == "Hii")
                {
                    SendMessageToClient("Hii", (IPEndPoint)remoteEndPoint);
                }
            }
            catch (SocketException e)
            {
                Debug.LogError("SocketException: " + e.Message);
            }
        }
    }

    void SendMessageToClient(string message, IPEndPoint clientEndPoint)
    {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        udpServerSocket.SendTo(messageBytes, clientEndPoint);
        Debug.Log("Sent to Client: " + message);
    }

    public void StopServer()
    {
        isRunning = false;

        if (udpServerSocket != null)
        {
            udpServerSocket.Close();
            udpServerSocket = null;
        }

        if (serverThread != null)
        {
            serverThread.Abort();
            serverThread = null;
        }
    }

    void OnApplicationQuit()
    {
        StopServer();
    }
}
