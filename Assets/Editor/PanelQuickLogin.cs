using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using UnityEditor;
//using UnityEngine.EventSystems;
using System.Security.Cryptography;
using System.Text;
using System;
using BestHTTP;
using BestHTTP.JSON;

[ExecuteInEditMode]
public class PanelQuickLogin : Editor
{
    //public static PanelQuickLogin mPanelQuickLogin = new PanelQuickLogin();
    public PanelQuickLogin() { }

    //static public PanelQuickLogin GetOrCreate(GameObject gameObject)
    //{
    //    if (gameObject == null) { return new PanelQuickLogin(); }
    //    var existed = gameObject.GetComponent<PanelQuickLogin>();
    //    return existed ?? gameObject.AddComponent<PanelQuickLogin>();
    //}
    //private UISceneWidget mButton_Register;
    //private UISceneWidget mButton_Login;
    //private UISceneWidget mButton_Back;
    //private GameObject mButton_Loading;
    //private UIInput mInput_Account;
    //private UIInput mInput_Password;
    //private UILabel mLabel_ErrorMessage;
    //private UILabel mLabel_LoginTips;
    private WWWHelp wwwh = new WWWHelp();
    //private TweenPosition panelQuickLoginTween;
    //private UIPopupList mPopuplist;
    private bool isSubmit = false;
    
    public static string EncryptionPass(string oldPass)
    {
        string newPass = "";
        MD5 md5 = new MD5CryptoServiceProvider();
        //第一次md5
        byte[] bPass = md5.ComputeHash(Encoding.UTF8.GetBytes(oldPass));
        // newPass = Encoding.UTF8.GetString(bPass);错误做法
        newPass = BitConverter.ToString(bPass).Replace("-", "").ToLower();
        Debug.Log("MD5:" + newPass);
        //加盐
        newPass += GlobalConst.salt;
        //第二次md5
        bPass = md5.ComputeHash(Encoding.UTF8.GetBytes(newPass));
        newPass = BitConverter.ToString(bPass).Replace("-", "").ToLower();
        Debug.Log("saltMD5:" + newPass);
        return newPass;
    }
    ///验证邮箱格式
    public static bool ValidateEmail(string email)
    {
        //验证邮箱正则表达式
        Regex regex = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
        if (!regex.IsMatch(email))
            return false;
        return true;
    }

    ///验证手机号码
    public static bool ValidateNumber(string number)
    {
        Regex regex = new Regex("^[0-9]{11}$");
        if (!regex.IsMatch(number))
            return false;
        return true;
    }

    ///生成count位的随机字符串
    public static string RandString(int count)
    {
        string str = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~!@#$%^&*()_+";//75个字符
        System.Random r = new System.Random();
        string result = string.Empty;

        //生成一个count位长的随机字符
        for (int i = 0; i < count; i++)
        {
            int m = r.Next(0, 75);
            string s = str.Substring(m, 1);
            result += s;
        }
        return result;
    }

    public void LoginOnClick()
    {
        //StartCoroutine(PanelQuickLogin.GetOrCreate(gameObject).ButtonLoginOnClick());
        ButtonLoginOnClick();
    }

    void ButtonLoginOnClick()
    {

        string email = MyWindow.window.GetInputAccount();
        string password = MyWindow.window.GetInputPassword();

        if (Validation()) { 
            password = EncryptionPass(password);    
            HTTPRequest request = new HTTPRequest(new Uri(GlobalConst.UserLoginPath), onRequestFinished);
            request.MethodType = BestHTTP.HTTPMethods.Post;
            request.AddHeader("Content-Type", "application/json;charset=utf-8");
            //request.AddHeader("Accept", "*/*");
            User user = User.CreateInstance("User") as User;
            //user = MyWindow.user;
            UserLogin userlogin = new UserLogin(email, password, GlobalConst.ClientFlag + "-" + user.deviceFlag, GlobalConst.ClientVersion);
            string parmJson = JsonConvert.SerializeObject(userlogin);
            Debug.Log("Json: " + parmJson);
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(parmJson);
            request.RawData = data;
            
            request.Send();
        }
        else
        {
            Debug.Log("登录格式错误");
        }
        email = "";
        password = "";
    }

    void onRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        Debug.Log("status code: " + resp.StatusCode);
        Debug.Log("req head: " + req.DumpHeaders());
        Debug.Log("req url: " + req.Uri);
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if(resp.IsSuccess)
                {
                    Debug.Log(resp.DataAsText);
                    
                    Debug.Log("Success");
                    UserLoginResponse loginResponse = JsonConvert.DeserializeObject<UserLoginResponse>(resp.DataAsText);
                    if (loginResponse.result.code == 0)
                    {
                        Debug.Log("登录成功!");
                        Debug.Log("result: " + loginResponse.result.code + " | " + loginResponse.result.description);
                        Debug.Log("userid: " + loginResponse.user.userId);
                        Debug.Log("accessToken: " + loginResponse.accessToken);
                        User user = User.CreateInstance("User") as User;
                        user.SetUser(loginResponse.user.userId, loginResponse.user.nickname, loginResponse.user.username, loginResponse.accessToken, loginResponse.refreshToken);
                        user.SaveUser();
                    }
                }
                else
                {
                    Debug.Log(resp.DataAsText);

                    Debug.Log("Fail");
                }
                break;
        }
    }

    IEnumerator sButtonLoginOnClick()
    {
        if (MyWindow.window.IsSubmit)
            yield break;
        Debug.Log("登录按钮");
        string email = MyWindow.window.GetInputAccount();
        string password = MyWindow.window.GetInputPassword();
        
        //验证账号密码格式
        if (Validation())
        {
            password = EncryptionPass(password);
            UserLogin userlogin = new UserLogin(email, password, GlobalConst.ClientFlag + "-" + User.Instance.deviceFlag, GlobalConst.ClientVersion);
            string parmJson = JsonConvert.SerializeObject(userlogin);
            // UIManager.Instance.ShowWaitLoading(true);
            MyWindow.window.IsSubmit = true;

            //Json parmJson = Json.Decode(JsonConvert.SerializeObject(userlogin)) as Json;
            
            //yield return StartCoroutine(wwwh.WWWPostLoad(GlobalConst.UserLoginPath, parmJson));
            MyWindow.window.IsSubmit = false;
            // UIManager.Instance.ShowWaitLoading(false);
            if (wwwh.CheckWWW())
            {
                //根据返回数据继续处理
                UserLoginResponse loginResponse = JsonConvert.DeserializeObject<UserLoginResponse>(wwwh.text);
                Debug.Log(loginResponse.result.code);
                if (loginResponse.result.code == 0)
                {
                    //登录成功，
                    Debug.Log("登录成功");
                    MyWindow.window.ClearInputAccountValue();
                    MyWindow.window.ClearInputPasswordValue();
                    User user = User.CreateInstance("User") as User;
                    user.SetUser(loginResponse.user.userId, loginResponse.user.nickname, loginResponse.user.username, loginResponse.accessToken, loginResponse.refreshToken);
                    user.SaveUser();
                    //SetPopupType(PopupType.CenterToTop);
                    //UIManager.Instance.SetVisible(UIName.PanelQuickLogin, false);
                    //UIManager.Instance.ShowMessageBox("登录成功", 2.5f);
                    //添加app通知用户Key值
                    //OneSignal.SendTags(new Dictionary<string, string>() { { "UserId", User.Instance.id }, { "Login", "true" } });
                    //if (UIManager.Instance.IsVisible(UIName.panelMyPlans))
                    //    PanelMyPlans.mPanelMyPlans.LoadMyPlans();
                }
                else if (loginResponse.result.code == 1004)
                {
                    //用户名不存在
                    Debug.Log("用户名不存在");
                    //mLabel_ErrorMessage.text = "*用户名不存在";
                    //mLabel_ErrorMessage.gameObject.SetActive(true);
                }
                else if (loginResponse.result.code == 1005)
                {
                    //用户名密码错误z
                    Debug.Log("用户名密码错误");
                    //mLabel_ErrorMessage.text = "*用户名或密码错误";
                    //mLabel_ErrorMessage.gameObject.SetActive(true);
                }
                else if (loginResponse.result.code == 1008)
                {
                    //未验证邮箱
                    Debug.Log("未验证邮箱");
                    //mLabel_ErrorMessage.text = "*未验证邮箱";
                    //mLabel_ErrorMessage.gameObject.SetActive(true);
                }
            }
        }
    }

    bool Validation()
    {
        string account = MyWindow.window.GetInputAccount();
        string password = MyWindow.window.GetInputPassword();
        Debug.Log("email:" + account);
        Debug.Log("pass:" + password);
        if (!account.Contains("@"))
        {
            if (!ValidateNumber(account))
            {
                //手机号码格式不正确
                //mLabel_ErrorMessage.text = "*手机号码或邮箱格式不正确";
                //mLabel_ErrorMessage.gameObject.SetActive(true);
                return false;
            }
        }
        else if (account.Contains("@"))
        {
            if (!ValidateEmail(account))
            {
                //邮箱格式错误处理
                //mLabel_ErrorMessage.text = "*手机号码或邮箱格式不正确";
                //mLabel_ErrorMessage.gameObject.SetActive(true);
                return false;
            }
        }
        if (String.IsNullOrEmpty(password))
        {
            //mLabel_ErrorMessage.text = "*密码不能空";
            //mLabel_ErrorMessage.gameObject.SetActive(true);
            return false;
        }

        return true;
    }
    
    // Use this for initialization
    void Start ()
    {
        Debug.Log("http setup");
        BestHTTP.HTTPManager.Setup();
    }

    void Awake()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private float m_LastEditorUpdateTime;

    protected virtual void OnEnable()
    {
//#if UNITY_EDITOR
        BestHTTP.HTTPManager.Setup();
        m_LastEditorUpdateTime = Time.realtimeSinceStartup;
        EditorApplication.update += OnEditorUpdate;
        Debug.Log("On Create!");
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
}
