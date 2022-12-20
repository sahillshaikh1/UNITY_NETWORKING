using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;

public class UDPServer : MonoBehaviour
{
    private static UDPServer _instance;
    public static UDPServer instance { get { return _instance; } }

   
    //  public Text Debugtext;
    public string request;
    public string response;
    IPEndPoint clientEndpoint;
    UdpClient listener;

   public NetworkLogicHandler networkLogicHandler;

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
    }
    void Start()
    {
        // Create a new UdpClient object to listen for incoming messages
         listener = new UdpClient(12345);

        // Start a new thread to listen for incoming messages
        Thread listenThread = new Thread(() =>
        {
            while (true)
            {
                // Receive a message from a client
                clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] message = listener.Receive(ref clientEndpoint);
                request = Encoding.ASCII.GetString(message);
                Debug.Log(request);
                // Process the request here...
                if (request == "Hi")
                {
                    response = "Hi Back";
                }


                // Send a response back to the client
                // response = "This is the response from the server";
                byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                listener.Send(responseBytes, responseBytes.Length, clientEndpoint);
                networkLogicHandler.EPAddress =clientEndpoint;

            }
        });
       // Debugtext.text = "Received request from " + clientEndpoint + ": " + request;
        listenThread.Start();

        Debug.Log("Press any key to stop the server...");

    }

    public void sendMsg(string responce, string Reqest, string ExpectedRequest, IPEndPoint clientEndpointAddress)
    {
        Debug.Log(clientEndpoint.Address.ToString() + " EPPPPPPP");

        if (Reqest == ExpectedRequest)
        {
            response = responce;
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            listener.Send(responseBytes, responseBytes.Length, clientEndpointAddress);
        }
        // Send a response back to the client
      
    }
}