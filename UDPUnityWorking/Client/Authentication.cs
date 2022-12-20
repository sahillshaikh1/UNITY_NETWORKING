using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    private static Authentication _instance;
    public static Authentication instance { get { return _instance; } }
   


    [System.Serializable]
    public class RegisterUser
    {
       
        public string username;
        public string password;

    }
    public RegisterUser registerUser;

    public InputField ReNameIPF;
    public InputField RePasswordIPF;
    public Button RegistrationBtn;

    public bool userTaken;
    public Text ErrorText;
    public bool islogin;

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
        RegistrationBtn.onClick.AddListener(RegistrationBtnM);
    }

    #region RegistrationField

    private void Update()
    {
        registerUser.username = ReNameIPF.text;
        registerUser.password = RePasswordIPF.text;
    }
    public void RegistrationBtnM()
    {
      
       string RegistreUserJson = "Regi"+JsonUtility.ToJson(registerUser);
        UdpClientTest.instance.sendToServer(RegistreUserJson);

    }

    public void LoginBtn()
    {
        string RegistreUserJson = "Login" + JsonUtility.ToJson(registerUser);
        UdpClientTest.instance.sendToServer(RegistreUserJson);
    }


    #endregion



}
