using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AuthenticationHandler : MonoBehaviour
{
    private static AuthenticationHandler _instance;
    public static AuthenticationHandler instance { get { return _instance; } }

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

    [System.Serializable]
    public class RegisterUser
    {
        public string ID = " ";
        public string username;
        public string password;

        internal List<RegisterUser> ToList()
        {
            throw new NotImplementedException();
        }
    }
    public List<RegisterUser> registerUser;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
       

      //  registerUser.Add(new RegisterUser(RegisterUserName.text, RegisterPassword.text, isAdmin.isOn));

    }
}
