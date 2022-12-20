using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using static System.Net.Mime.MediaTypeNames;

public class NetworkLogicHandler : MonoBehaviour
{
    private static NetworkLogicHandler _instance;
    public static NetworkLogicHandler instance { get { return _instance; } }


    public IPEndPoint EPAddress;
    [System.Serializable]
    public class InitialJoin
    {
        public string ID;
        public string IP;
    }
    public InitialJoin initialJoin;

    [System.Serializable]
    public class RegisterUser
    {

        public string username;
        public string password;
        public RegisterUser(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
    public List<RegisterUser> registerUser;
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
    public void ProcessingData(string responce)
    {
        if (string.IsNullOrEmpty(responce))
        {
            return;
        }
        Debug.Log(responce + " " + responce.Substring(0, 4));
        if (responce.Substring(0, 4) == "INIT")
        {
            string test = responce.Remove(0, 4);
            initialJoin = JsonUtility.FromJson<InitialJoin>(test);


        }  
        if (responce.Substring(0, 4) == "Regi")
        {
            string test = responce.Remove(0, 4);
            var test2 = JsonUtility.FromJson<RegisterUser>(test);
            foreach (var item in registerUser)
            {
              
                if (item.username.Equals(test2.username))
                {
                    UDPServer.instance.sendToClient("USERTAKEN", EPAddress);
                   
                    return;
                }
                else
                {
                    UDPServer.instance.sendToClient("USERNOTAKEN", EPAddress);

                }


            }
          
            registerUser.Add(new RegisterUser(test2.username, test2.password));


        }
        if (responce.Substring(0, 5) == "Login")
        {
            string test = responce.Remove(0, 5);
           
            var test2 = JsonUtility.FromJson<RegisterUser>(test);
           
            foreach (var item in registerUser)
            {
                if (item.username.Equals(test2.username))
                {
                    UDPServer.instance.sendToClient("LOGINSUCESS", EPAddress);
                    return;
                }
               
            }


        }

    }
}
