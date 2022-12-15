using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;


class ServerGpt : MonoBehaviour
{
    UdpClient client;
    IPEndPoint serverEndpoint;
   public string request;
    Thread thr;

    public InputField SendmsgIF;
    public Text OutputText;
    public bool isReceived;

    public List<string> ClientAips;
    public Text DebuList;


    private void Start()
    {
        Init();
    }
    public void Init()
    {
        client = new UdpClient(8051);

        // Create an IPEndPoint object to represent the server's endpoint
        serverEndpoint = new IPEndPoint(IPAddress.Parse("192.168.4.20"), 8051);
         thr = new Thread(new ThreadStart(ReceiveMsg));
        thr.IsBackground = true;
        thr.Start();
    }

    public void ReceiveMsg()
    {
        while (true)
        {
            // Receive data from the client
            byte[] data = client.Receive(ref serverEndpoint);

            // Convert the received data to a string
             request = Encoding.UTF8.GetString(data);
            if (request == "REQUEST")
            {
                byte[] responseBytes = Encoding.ASCII.GetBytes("RESPONSE");
                client.Send(responseBytes, responseBytes.Length, serverEndpoint);
            }
            // Print the received request to the console
            Debug.Log("Received request: " + request);
            isReceived= true;
            if (request.Substring(0, 2) == "IP")
            {
               
              //  ClientAips.Add(request.Remove(0, 2));
            }

            //// Process the request and generate a response
            //string response = ProcessRequest(request);

            //// Convert the response to a byte array
            //byte[] responseData = Encoding.UTF8.GetBytes(response);

            //// Send the response to the client
            //client.Send(responseData, responseData.Length, serverEndpoint);

        }
        
    }
    private void Update()
    {
        if (isReceived)
        {
           
            OutputText.text = request;
            isReceived = false;
        }
       
    }

    public void SendMsgButton()
    {
        client.EnableBroadcast = true;
        SendMsg(SendmsgIF.text);
    }
    public void SendMsg(string msg)
    {
        // Process the request and generate a response
        string Send = msg;

        // Convert the response to a byte array
        //  byte[] SendData = Encoding.UTF8.GetBytes(Send);

        // Send the response to the client
        //  client.Send(SendData, SendData.Length, serverEndpoint);
        
        if (request == "REQUEST")
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes("RESPONSE");
            client.Send(responseBytes, responseBytes.Length, serverEndpoint);
        }
        // Otherwise, broadcast the message to all clients
        else
        {
            byte[] broadcastBytes = Encoding.ASCII.GetBytes(Send);
            // client.Send(broadcastBytes, broadcastBytes.Length, new IPEndPoint(IPAddress.Broadcast, 8051));
            //for (int i = 0; i < ClientAips.Count; i++)
            //{
                client.Send(broadcastBytes, broadcastBytes.Length, serverEndpoint);

           // }
            //  client.Send(broadcastBytes, broadcastBytes.Length, new IPEndPoint(IPAddress.Parse("192.168.4.53"), 8051));
        }
    }
    

    static string ProcessRequest(string request)
    {
        // This is just a placeholder function that returns a dummy response
        return "This is the response to the request: " + request;
    }

   
   
}


