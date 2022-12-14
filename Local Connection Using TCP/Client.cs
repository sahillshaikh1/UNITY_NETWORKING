using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
using System.Text;
using UnityEngine.UI;


public class Client : MonoBehaviour
{
    private static string IP;  // define in init
    public static int port;  // define in init
    // receiving Thread
    Thread receiveThread;
    Thread SendThread;
    [SerializeField] private InputField IpAddress;

    public static  Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    // Start is called before the first frame update
    void Start()
    {
       

        receiveThread = new Thread(
           new ThreadStart(LoopConnect));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        SendThread = new Thread(
           new ThreadStart(SendLoop));
        SendThread.IsBackground = true;
        SendThread.Start();
        
    }
    public void Init()
    {
       
    }

    public static void SendLoop()
    {
        while (true)
        {
            Thread.Sleep(500);
           //------------------Sending Data from here -----------------
            Debug.Log("Enter your request");
            string req = "hi";
            byte[] buffer =  Encoding.ASCII.GetBytes(req);
            _clientSocket.Send(buffer);

            byte[] receiveBuf = new byte[1024];


            //Getting Data Here
            int rec = _clientSocket.Receive(receiveBuf);
            byte[] data = new byte[rec];
            Array.Copy(receiveBuf, data, rec);
            Debug.Log("Received: " + Encoding.ASCII.GetString(data));

            string receiveedData = Encoding.ASCII.GetString(data);
           

        }
    }

    public static void LoopConnect()
    {
        int attempts = 0;
        IP = "127.0.0.1";
        port = 8051;
        while (!_clientSocket.Connected)
        {
            try
            {
                attempts++;
                _clientSocket.Connect("127.0.0.1", port);

            }
            catch (SocketException)
            {
                Debug.Log("Connection Attempts" + attempts.ToString());
               
            }

        }
        Debug.Log("Connected");

    }
    private void OnDisable()
    {
        SendThread.Abort();
        receiveThread.Abort();
    }
    private void OnApplicationQuit()
    {
        SendThread.Abort();
        receiveThread.Abort();

    }


}
