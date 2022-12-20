using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using System.Threading;

public class UdpClientTest : MonoBehaviour
{
    private static UdpClientTest _instance;
    public static UdpClientTest instance { get { return _instance; } }

    // public Text DebugText;
    public string response;
    public string request;
    IPEndPoint serverEndpoint;
    UdpClient client;
    public NetworkLogicHandler networkLogicHandler;

    [System.Serializable]
    public class InitialJoin
    {
        public string ID;
        public string IP;
    }
    public InitialJoin initialJoin;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        initialJoin.ID = "init";
        initialJoin.IP = GetLocalIPAddress();

    }
    void Start()
    {
        Init();
    }
    
    public void Init()
    {
        // Create a new UdpClient object to send a message
        client = new UdpClient();
        client.Connect(new IPEndPoint(IPAddress.Parse("192.168.4.20"), 12345));

        // Send a request to the server
         request = "INIT"+ JsonUtility.ToJson(initialJoin);

        byte[] requestBytes = Encoding.ASCII.GetBytes(request);
        client.Send(requestBytes, requestBytes.Length);

       
        Thread listenThread = new Thread(() =>
        {
            while (true)
            {
                // Receive a message from a client
                serverEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] message = client.Receive(ref serverEndpoint);
                response = Encoding.ASCII.GetString(message);

                NetworkLogicHandler.instance.ProcessingData(response);

                Debug.Log("Received request from " + serverEndpoint + ": " + response);
                networkLogicHandler.EPAddress = serverEndpoint;
            }
        });
        // Debugtext.text = "Received request from " + clientEndpoint + ": " + request; 
        listenThread.Start();
    }
    public void sendMsg(string responce, string Reqest, string ExpectedRequest)
    {
      
       
        if (Reqest == ExpectedRequest)
        {
            response = responce;
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            client.Send(responseBytes, responseBytes.Length);
        }
        

    }
      public void sendToServer(string responce)
      {
            response = responce;
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            client.Send(responseBytes, responseBytes.Length);

      }



    public string GetResponce()
    {
        return response;
    }

   
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {

                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}

