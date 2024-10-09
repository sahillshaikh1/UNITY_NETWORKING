using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private Socket udpClientSocket;
    private Thread clientThread;
    private bool isRunning = false;
    bool isConnected = false;
    public string serverIP = "";
    public int serverPort = 3001;
    private int localPort = 3000;
    public Report report;

    public TMP_InputField ipAddress;
    public GameObject _ipAddressOP;

    void Start()
    {
        StartClient();
        ipAddress.text = GetString("IP");
        report = GetComponent<Report>();
    }

    void StartClient()
    {
        Debug.Log("Start Connecting..");
        try
        {
            udpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
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
        if (isConnected)
        {
            SetString("IP", serverIP);
            if (ipAddress)
            {
                _ipAddressOP.SetActive(false);
            }
        }
    }

    public void ConnectToServerLogin()
    {
        if (ipAddress)
        {
            if (!string.IsNullOrEmpty(ipAddress.text))
            {
                serverIP = ipAddress.text;
            }
            GetData();
        }
    }

    public void GetData()
    {
        Debug.Log("Getting User data");
        SendMessageToServer("GetUserdata");
    }
  public  string data;
    public async void SendReport() //REPORT
    {

        SendMessageToServer(Report.instance.Users.Name);

        data = JsonUtility.ToJson(report.Users);
        Debug.Log(data + " DATA");
        SendMessageToServer("StartSending"); // Inform the server that report transmission is starting
        await Task.Delay(5);
      

        IEnumerable<string> packets = data.SplitThis(100).ToArray(); // Splitting data into 100-byte packets
        int packetNumber = 0; // Starting packet number

        foreach (var packet in packets)
        {
            await Task.Delay(200); // Small delay between packets
                                   // string packetWithNumber = $"{packetNumber}:{packet}"; // Adding the packet number as a prefix
            string packetWithNumber =packet; // Adding the packet number as a prefix
            SendMessageToServer(Report.instance.Users.Name + "ID" +packetWithNumber); // Send the numbered packet to the server
           // SendMessageToServer(packetWithNumber); // Send the numbered packet to the server
            packetNumber++;
            await Task.Delay(100); // Small delay between packets
            data = "";
            Debug.Log( "PACKETS: "+packetWithNumber);
        }
        SendMessageToServer("StopSending"); // Inform the server that report transmission has ended
        await Task.Delay(5);
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
                    Debug.Log("Received from Server: " + receivedText + remoteEndPoint);

                    if (receivedText.StartsWith("UserData"))
                    {
                        isConnected = true;
                        Debug.Log("Processed received data.");
                        NewDataBase.Instance.DataIndex = JsonUtility.FromJson<DataIndex>(receivedText.Substring(8)); // Remove 'UserData' prefix
                    }
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

    #region Functions

    public void SetString(string KeyName, string Value)
    {
        PlayerPrefs.SetString(KeyName, Value);
    }

    public string GetString(string KeyName)
    {
        return PlayerPrefs.GetString(KeyName);
    }

    #endregion
}
