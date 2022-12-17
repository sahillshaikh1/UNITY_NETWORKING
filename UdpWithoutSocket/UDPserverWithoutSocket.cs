using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
public class UDPserverWithoutSocket : MonoBehaviour
{
    public Text Debugtext;
    public string request;
    IPEndPoint clientEndpoint;
    // Start is called before the first frame update
    private void Awake()
    {
        Debugtext.text = ".............";
    }
    void Start()
    {
        // Create a new UdpClient object to listen for incoming messages
        UdpClient listener = new UdpClient(12345);

        // Start a new thread to listen for incoming messages
        Thread listenThread = new Thread(() =>
        {
            while (true)
            {
                // Receive a message from a client
                 clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] message = listener.Receive(ref clientEndpoint);
                 request = Encoding.ASCII.GetString(message);

                Debug.Log("Received request from " + clientEndpoint + ": " + request);
                // Process the request here...

                // Send a response back to the client
                string response = "This is the response from the server";
                byte[] responseBytes = Encoding.ASCII.GetBytes(response);
                listener.Send(responseBytes, responseBytes.Length, clientEndpoint);
            }
        });
        Debugtext.text = "Received request from " + clientEndpoint + ": " + request;
        listenThread.Start();

        Debug.Log("Press any key to stop the server...");
      
    }

    // Update is called once per frame
    void Update()
    {
      

    }
}
