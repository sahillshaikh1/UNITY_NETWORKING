using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class NetworkLogicHandler : MonoBehaviour
{

    private static NetworkLogicHandler _instance;
    public static NetworkLogicHandler instance { get { return _instance; } }


    public IPEndPoint EPAddress;

    public Text textdEBUG;
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
    public void DataHandler()
    {
        num++;
        UdpClientTest.instance.sendMsg("Hii Sahil" + num, "Hii Back", "Hii Back");

    }

    private void Update()
    {
        textdEBUG.text = UdpClientTest.instance.GetResponce();
    }
}
