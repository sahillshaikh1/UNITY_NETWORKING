using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;


public class ClientGpt : MonoBehaviour
{
    UdpClient client;
    IPEndPoint serverEndpoint;
    Thread thr;
    public InputField SendmsgIF;

    public Text Debugtext;
    public bool isReceived;
    string response;


    private void Start()
    {
        Init();
    }
    public void Init()
    {
        // Create a new UDP client
        client = new UdpClient();

        // Create an IPEndPoint object to represent the server's endpoint
        serverEndpoint = new IPEndPoint(IPAddress.Parse("192.168.4.20"), 8051);
        Thread thr = new Thread(new ThreadStart(Main));
        thr.Start();
        thr.IsBackground = true;
    }
     void Main()
    {
       

        // Continuously send requests to the server
        while (true)
        {
            // Get the request data from the user
            Debug.Log("Enter request data: ");
            string requestData = GetLocalIPAddress() ;

            // Convert the request data to a byte array
            byte[] data = Encoding.UTF8.GetBytes(requestData);
            isReceived = true;
            // Send the request to the server
            client.Send(data, data.Length, serverEndpoint);

            // Receive the response from the server
            byte[] responseData = client.Receive(ref serverEndpoint);

            // Convert the response data to a string
             response = Encoding.UTF8.GetString(responseData);

            if(response == "hi back")
            {
                SendMsg("Whats up , bro");
            }
            // Print the response to the console
            Debug.Log("Response: " + response);
        }
    }
    private void Update()
    {
        if (isReceived)
        {
            Debugtext.text = response;
            isReceived = false;
        }
    }
    public void SendMsgButton()
    {
        SendMsg(SendmsgIF.text);
    }
    public void SendMsg(String msg) {
        Debug.Log("Enter request data: ");
        string requestData = msg;

        // Convert the request data to a byte array
        byte[] data = Encoding.UTF8.GetBytes(requestData);

        // Send the request to the server
        client.Send(data, data.Length, serverEndpoint);

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
