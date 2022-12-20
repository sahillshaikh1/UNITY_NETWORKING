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
      //  UdpClientTest.instance.sendMsg("Hii Sahil" + num, "Hii Back", "Hii Back");

    }

    private void Update()
    {
        textdEBUG.text = UdpClientTest.instance.GetResponce();
        if (Authentication.instance.userTaken)
        {
            Authentication.instance.ErrorText.color = Color.red;
            Authentication.instance.ErrorText.text = "This username is already taken.Try Another";
        }
        else
        {
            Authentication.instance.ErrorText.text = "";
        }
    }
    public void ProcessingData(string responce)
    {
        if (string.IsNullOrEmpty(responce))
        {
            return;
        }
        Debug.Log(responce + " " + responce.Substring(0, 4));
        if (responce.Substring(0, 9) == "USERTAKEN") /// REGISTER LOGIC
        {
           
            Authentication.instance.userTaken = true;
          
            Debug.Log("BHAI NAAM TAKEN HAI");

        } 
        if (responce == "USERNOTAKEN") /// REGISTER LOGIC
        {
           
            Authentication.instance.userTaken = false;
          
            Debug.Log("REGISTER SUCESSFULLY");

        }
        if (responce == "LOGINSUCESS") /// Login LOGIC
        {

            Authentication.instance.islogin = true;
          
            Debug.Log("ILOGINSUCESS");

        }
      

    }
}
