using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEditor;
using System.Security.Cryptography;
using System;

public class UserSaveLoad : Editor
{

    // An example where the encoding can be found is at 
    // http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
    // We will just use the KISS method and cheat a little and use 
    // the examples from the web page since they are fully described 

    // This is our local private members 
    Rect _Save, _Load, _SaveMSG, _LoadMSG;
    bool _ShouldSave, _ShouldLoad, _SwitchSave, _SwitchLoad;
    string _FileLocation, _FileName;
    public GameObject _Player;
    public UserData myData;
    string _UserName;
    string _AccessToken;
    string _RefreshToken;
    string _data;

    Vector3 VPosition;

    // When the EGO is instansiated the Start will trigger 
    // so we setup our initial values for our local members 
    void Start()
    {
    }

    private float m_LastEditorUpdateTime;

    protected virtual void OnEnable()
    {
//#if UNITY_EDITOR
        // We setup our rectangles for our messages 
        _Save = new Rect(10, 80, 100, 20);
        _Load = new Rect(10, 100, 100, 20);
        _SaveMSG = new Rect(10, 120, 400, 40);
        _LoadMSG = new Rect(10, 140, 400, 40);

        // Where we want to save and load to and from 
        _FileLocation = Application.dataPath;
        _FileName = "SaveData.xml";

        // for now, lets just set the name to Joe Schmoe 
        _UserName = "Default";
        
        // we need soemthing to store the information into 
        myData = new UserData();
        BestHTTP.HTTPManager.Setup();
        m_LastEditorUpdateTime = Time.realtimeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
        Debug.Log("Save Load On Create");
//#endif
    }

    protected virtual void OnDisable()
    {
        //#if UNITY_EDITOR
        EditorApplication.update -= OnEditorUpdate;
        //#endif
    }

    protected virtual void OnEditorUpdate()
    {
        // In here you can check the current realtime, see if a certain
        // amount of time has elapsed, and perform some task.
    }

    void Update() { }

    public void LoadData()
    {
        Debug.Log("Loading Data");
        //GUI.Label(_LoadMSG, "Loading from: " + _FileLocation);
        // Load our UserData into myData 
        LoadXML();
        if (_data.ToString() != "")
        {
            // notice how I use a reference to type (UserData) here, you need this 
            // so that the returned object is converted into the correct type 
            myData = (UserData)DeserializeObject(_data);
            // set the players position to the data we loaded 
            //VPosition = new Vector3(myData._iUser.x, myData._iUser.y, myData._iUser.z);y
            // just a way to show that we loaded in ok 
            Debug.Log(myData._iUser.accessToken);
        }
        //Debug.Log(MyWindow.user.username);
        //MyWindow.user.SetUser(myData._iUser.userID, myData._iUser.userNickName, myData._iUser.userName, myData._iUser.accessToken, myData._iUser.refreshToken);
        //Debug.LogError(MyWindow.user.name);
        //MyWindow.user.username = myData._iUser.userName;
        //MyWindow.user.id = myData._iUser.userID;
        //MyWindow.user.nickname = myData._iUser.userNickName;
        //MyWindow.user.accessToken = myData._iUser.accessToken;
        //MyWindow.user.refreshToken = myData._iUser.refreshToken;

    }

    public void SaveData(User user)
    {
        //GUI.Label(_SaveMSG, "Saving to: " + _FileLocation);
        myData._iUser.userName = user.username;
        myData._iUser.userID = user.id;
        myData._iUser.userNickName = user.nickname;
        myData._iUser.accessToken = user.accessToken;
        myData._iUser.refreshToken = user.refreshToken;

        // Time to creat our XML! 
        _data = SerializeObject(myData);
        // This is the final resulting XML from the serialization process 
        CreateXML();
        Debug.Log(_data);
    }
    
    public void ClearData()
    {
        myData._iUser.userName = "";
        myData._iUser.userID = "";
        myData._iUser.userNickName = "";
        myData._iUser.accessToken = "";
        myData._iUser.refreshToken = "";
        _data = "";

        CreateXML();
        Debug.Log("Clear User Data");
    }

    void OnGUI()
    {

        //*************************************************** 
        // Loading The Player... 
        // **************************************************       
        //if (GUI.Button(_Load, "Load"))
        //{

        //    GUI.Label(_LoadMSG, "Loading from: " + _FileLocation);
        //    // Load our UserData into myData 
        //    LoadXML();
        //    if (_data.ToString() != "")
        //    {
        //        // notice how I use a reference to type (UserData) here, you need this 
        //        // so that the returned object is converted into the correct type 
        //        myData = (UserData)DeserializeObject(_data);
        //        // set the players position to the data we loaded 
        //        VPosition = new Vector3(myData._iUser.x, myData._iUser.y, myData._iUser.z);
        //        _Player.transform.position = VPosition;
        //        // just a way to show that we loaded in ok 
        //        Debug.Log(myData._iUser.name);
        //    }

        //}

        //*************************************************** 
        // Saving The Player... 
        // **************************************************    
        //if (GUI.Button(_Save, "Save"))
        //{

        //    GUI.Label(_SaveMSG, "Saving to: " + _FileLocation);
        //    myData._iUser.x = _Player.transform.position.x;
        //    myData._iUser.y = _Player.transform.position.y;
        //    myData._iUser.z = _Player.transform.position.z;
        //    myData._iUser.name = _PlayerName;

        //    // Time to creat our XML! 
        //    _data = SerializeObject(myData);
        //    // This is the final resulting XML from the serialization process 
        //    CreateXML();
        //    Debug.Log(_data);
        //}


    }

    /* The following metods came from the referenced URL */
    string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return (constructedString);
    }

    byte[] StringToUTF8ByteArray(string pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }

    // Here we serialize our UserData object of myData 
    string SerializeObject(object pObject)
    {
        string XmlizedString = null;
        MemoryStream memoryStream = new MemoryStream();
        XmlSerializer xs = new XmlSerializer(typeof(UserData));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        xs.Serialize(xmlTextWriter, pObject);
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
        XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
        return XmlizedString;
    }

    // Here we deserialize it back into its original form 
    object DeserializeObject(string pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(UserData));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        return xs.Deserialize(memoryStream);
    }

    // Finally our save and load methods for the file itself 
    void CreateXML()
    {
        StreamWriter writer;
        FileInfo t = new FileInfo(_FileLocation + "\\" + _FileName);
        if (!t.Exists)
        {
            writer = t.CreateText();
        }
        else
        {
            t.Delete();
            writer = t.CreateText();
        }
        writer.Write(_data);
        writer.Close();
        Debug.Log("File written.");
    }


    void LoadXML()
    {
        StreamReader r = File.OpenText(_FileLocation + "\\" + _FileName);
        string _info = r.ReadToEnd();
        r.Close();
        _data = _info;
        
        Debug.Log("File Read");
    }

    static string encryptKey = "Oyea";    //定义密钥  

    #region 加密字符串  
    /// <summary> /// 加密字符串   
    /// </summary>  
    /// <param name="str">要加密的字符串</param>  
    /// <returns>加密后的字符串</returns>  
    static string Encrypt(string str)
    {
        DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   

        byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

        byte[] data = Encoding.Unicode.GetBytes(str);//定义字节数组，用来存储要加密的字符串  

        MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

        //使用内存流实例化加密流对象   
        CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);

        CStream.Write(data, 0, data.Length);  //向加密流中写入数据      

        CStream.FlushFinalBlock();              //释放加密流      

        return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
    }
    #endregion

    #region 解密字符串   
    /// <summary>  
    /// 解密字符串   
    /// </summary>  
    /// <param name="str">要解密的字符串</param>  
    /// <returns>解密后的字符串</returns>  
    static string Decrypt(string str)
    {
        DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    

        byte[] key = Encoding.Unicode.GetBytes(encryptKey); //定义字节数组，用来存储密钥    

        byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  

        MemoryStream MStream = new MemoryStream(); //实例化内存流对象      

        //使用内存流实例化解密流对象       
        CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);

        CStream.Write(data, 0, data.Length);      //向解密流中写入数据     

        CStream.FlushFinalBlock();               //释放解密流      

        return Encoding.Unicode.GetString(MStream.ToArray());       //返回解密后的字符串  
    }
    #endregion
}

// UserData is our custom class that holds our defined objects we want to store in XML format 
public class UserData
{
    // We have to define a default instance of the structure 
    public DemoData _iUser;
    // Default constructor doesn't really do anything at the moment 
    public UserData() { }

    // Anything we want to store in the XML file, we define it here 
    public struct DemoData
    {
        public string userName;
        public string userID;
        public string accessToken;
        public string refreshToken;
        public string userNickName;
    }
}