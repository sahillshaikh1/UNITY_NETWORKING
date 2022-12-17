using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;

public class UdpClientWithoutSocket : MonoBehaviour
{
    public Text DebugText;
    public string response;
    IPEndPoint serverEndpoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Onnnected()
    {
        // Create a new UdpClient object to send a message
        UdpClient client = new UdpClient();
        client.Connect(new IPEndPoint(IPAddress.Parse("192.168.0.108"), 12345));

        // Send a request to the server
        string request = "This is a request from the client";

        byte[] requestBytes = Encoding.ASCII.GetBytes(request);
        client.Send(requestBytes, requestBytes.Length);

        // Receive a response from the server
        serverEndpoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] responseBytes = client.Receive(ref serverEndpoint);
        response = Encoding.ASCII.GetString(responseBytes);

        Debug.Log("Received response from " + serverEndpoint + ": " + response);

        Console.WriteLine("Press any key to exit...");
    }

    // Update is called once per frame
    void Update()
    {
        DebugText.text = response + " " + serverEndpoint;
    }
}
