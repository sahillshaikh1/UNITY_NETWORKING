using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class UDPServer1 : MonoBehaviour
{
    private static UDPServer1 _instance;
    public static UDPServer1 Instance { get { return _instance; } }

    public NewDataBase dataBase;

    private Socket udpServerSocket;
    private bool isRunning = false;
    public int port = 3000;

    private bool getReport;
    private string reportJson;
    

    bool isCleared = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    void Start()
    {
        StartServer();
    }
    void StartServer()
    {
        udpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udpServerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
        isRunning = true;

        // Start continuously listening for incoming messages
        BeginReceive();

        Debug.Log($"UDP Server started and listening on port {port}");
    }
    // Begin asynchronous listening for data from any client

    void BeginReceive()
    {
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] buffer = new byte[1024];
        // Begin asynchronous receiving
        if (isRunning)
        {
            udpServerSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEndPoint, new AsyncCallback(ReceiveCallback), buffer);
        }
    }
    // Callback method called when data is received from a client

    void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = (byte[])result.AsyncState;

            int dataLength = udpServerSocket.EndReceiveFrom(result, ref remoteEndPoint);
            string receivedMessage = Encoding.ASCII.GetString(receivedData, 0, dataLength);

          

            // Handle client data asynchronously to avoid blocking
            ThreadPool.QueueUserWorkItem(ProcessClientData, new ClientData(receivedData, dataLength, (IPEndPoint)remoteEndPoint));

            // Immediately begin listening for the next message
            BeginReceive();
        }
        catch (Exception e)
        {
            Debug.LogError("ReceiveCallback Exception: " + e.Message);
        }
    }
    // Process client data asynchronously

    private void ProcessClientData(object clientDataObj)
    {
        ClientData clientData = (ClientData)clientDataObj;
        string receivedMessage = Encoding.ASCII.GetString(clientData.Data, 0, clientData.Length);
        foreach (var Users in NewDataBase.Instance.CurrentUserReport.usersReport)
        {
            if (receivedMessage == Users.Name)
            {
                Users.Packets = string.Empty;
            }
        }
        // Handle specific commands from the client
        if (receivedMessage == "StopSending")
        {
           
            getReport = false;
            Debug.Log("Main file full: " + reportJson);

            NewDataBase.Instance.CurrentUserReport = JsonUtility.FromJson<UserReport>(reportJson);
            Debug.Log("End report");


           

         
            reportJson = ""; // Reset report after processing
        }
        else if (receivedMessage == "GetUserdata")
        {
            // Send user data back to the client
            string data = JsonUtility.ToJson(dataBase.updatedSavedData);
            Debug.Log("Data Length: " + data.Length);
            string toJson = "UserData" + JsonUtility.ToJson(dataBase.updatedSavedData);
            SendMessageToClient(toJson, clientData.ClientEndPoint);

           
        }
        else if (receivedMessage == "StartSending")
        {
            getReport = true;
           
        } 
     
        else if (getReport) //Getting data from user in format of NameID{Json}
        {
            reportJson += receivedMessage; // Append report JSON data
            foreach (var item in NewDataBase.Instance.CurrentUserReport.usersReport)
            {

                Debug.Log("GetNames"+ GetNameFromPacket(receivedMessage));
                if (GetNameFromPacket(receivedMessage) == item.Name)
                {
                    ClearData(item.Packets);
                    item.Packets += CleanPackets(receivedMessage);
                    Debug.Log(receivedMessage);
                }
            }
        }
    }
    void ClearData(string data) //Clearing Old Data 
    {
        data = string.Empty;
    }
    string CleanPackets(string receivedMessage) //Clean Packets as in removing name and ID from packets
    {
        // Find the position of "ID"
        int idIndex = receivedMessage.IndexOf("ID");

        // Check if "ID" exists in the message
        if (idIndex >= 0)
        {
            // Remove everything before and including the "ID"
            string cleanMessage = receivedMessage.Substring(idIndex + "ID".Length);
            return cleanMessage; // Return the cleaned message
        }
        else
        {
            Debug.Log("ID not found in the message.");
            return receivedMessage; // Return the original message if "ID" is not found
        }
    }
    public  string GetNameFromPacket(string packet)
    {
        // Find the index of "ID" in the packet
        int idIndex = packet.IndexOf("ID");

        // If "ID" is found, extract the substring before it (which is the name)
        if (idIndex > 0)
        {
            // The name will be the substring from the start to the character right before "ID"
            return packet.Substring(0, idIndex);
        }

        // Return empty or null if "ID" is not found
        return string.Empty;
    }
    // Send a message back to the client asynchronously

    public void SendMessageToClient(string message, IPEndPoint clientEndPoint)
    {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        udpServerSocket.BeginSendTo(messageBytes, 0, messageBytes.Length, SocketFlags.None, clientEndPoint, new AsyncCallback(SendCallback), null);
        Debug.Log($"Sent to Client: {message}");
    }
    // Callback method after sending data to the client

    void SendCallback(IAsyncResult result)
    {
        try
        {
            udpServerSocket.EndSendTo(result);
        }
        catch (Exception e)
        {
            Debug.LogError("SendCallback Exception: " + e.Message);
        }
    }
    // Stop the server when the application is closed

    public void StopServer()
    {
        isRunning = false;

        if (udpServerSocket != null)
        {
            udpServerSocket.Close();
            udpServerSocket = null;
        }
    }
    void OnApplicationQuit()
    {
        StopServer();
        string systemData = JsonUtility.ToJson(NewDataBase.Instance.CurrentUserReport);

        using StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/UserReports.json");
        writer.Write(systemData);
    }
    // Helper class to encapsulate client data

    private class ClientData
    {
        public byte[] Data { get; }
        public int Length { get; }
        public IPEndPoint ClientEndPoint { get; }

        public ClientData(byte[] data, int length, IPEndPoint clientEndPoint)
        {
            Data = new byte[length];
            Array.Copy(data, Data, length);
            Length = length;
            ClientEndPoint = clientEndPoint;
        }
    }
}
