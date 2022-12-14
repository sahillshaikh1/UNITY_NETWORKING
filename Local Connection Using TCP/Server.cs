using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;


public class Server : MonoBehaviour
{
    private static string IP;  // define in init
    public static int port;  // define in init
    private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static List<Socket> _ClientSocket = new List<Socket> ();

    private static byte[] _Buffer = new byte[1024];
    [SerializeField] private  InputField IpAddress;
    // Start is called before the first frame update
    void Start()
    {
        SetupServer();
    }
    public void Init()
    {
       
    }
    // Update is called once per frame
    void Update()
    {

        
    }
    public void ConnectToServerBtn()
    {
        if (String.IsNullOrEmpty(IP) && String.IsNullOrEmpty(IpAddress.text)) return;
        
         SetupServer();

    }
    private  void SetupServer()
    {
        IP = "127.0.0.1";
        port = 8051;

        Debug.Log("Setting up server........");
        _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), port)); 
        _serverSocket.Listen(1);
        _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
    }

    private static void AcceptCallBack(IAsyncResult asyncResult)
    {
        Socket socket = _serverSocket.EndAccept(asyncResult);
        _ClientSocket.Add(socket);
        Debug.Log("Client Connects........");

        socket.BeginReceive(_Buffer,0,_Buffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallBAck) , socket);
        _serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);

    }
    private static void ReceiveCallBAck(IAsyncResult asyncResult)
    {
        Socket socket = (Socket)asyncResult.AsyncState;
        int receive = socket.EndReceive(asyncResult);

        //----------------Receive Data From Client Here-------------------------------
        byte[] dataBF = new byte[receive];
        Array.Copy(_Buffer, dataBF,  receive);
        string text = Encoding.ASCII.GetString(dataBF);
        Debug.Log(text + "Text Receive");

        //----------------Responding To Clint Request-------------------------------

        string responce = text;

        if (responce =="ip")
        {
            Debug.Log("Processing........");
        }
        switch (responce)
        {
            case "ip":
                // code block
                responce = GetLocalIPAddress();
                break;
            case "hi":
                // code block
                responce = " Hii Back";
                break;
           
        }

        //if (text.ToLower() != "ip")
        //{
        //    responce = "Invalid Request";

        //}
        //else
        //{
        //    responce = GetLocalIPAddress();

        //}
        //----------------Sendind Responce Data To Client From Here-------------------------------
        byte[] data = Encoding.ASCII.GetBytes(responce);
        socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        socket.BeginReceive(_Buffer, 0, _Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBAck), socket);

    }


    private static void SendCallback(IAsyncResult asyncResult)
    {
        Socket socket = (Socket)asyncResult.AsyncState;
        socket.EndSend(asyncResult);
    }

    /* Gets the Ip Address of your connected network and
  shows on the screen in order to let other players join
  by inputing that Ip in the input field */
    // ONLY FOR HOST SIDE 
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
