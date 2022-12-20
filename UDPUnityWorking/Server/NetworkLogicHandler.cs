using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
public class NetworkLogicHandler : MonoBehaviour
{
    private static NetworkLogicHandler _instance;
    public static NetworkLogicHandler instance { get { return _instance; } }


    public IPEndPoint EPAddress;
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
    int num;
    public void DataHandler() {

       
        num++;
        UDPServer.instance.sendMsg("Hii Back"+ num, "Hi", "Hi", EPAddress);

    }

}
