using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System;
using BestHTTP;
using BestHTTP.JSON;

public class LogWindow : ScriptableWizard
{
    string mInput_Account;
    string mInput_Password;


    public void OnWizardCreate()
    {
        mInput_Account = "";
        mInput_Password = "";
        Debug.Log("Create");
    }


    void OnGUI()
    {
        //Debug.Log("UPDATE");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("账号：", GUILayout.MaxWidth(35));
        mInput_Account = GUILayout.TextField(mInput_Account, GUILayout.MinWidth(80));
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("密码：", GUILayout.MaxWidth(35));
        mInput_Password = GUILayout.TextField(mInput_Password, '*', GUILayout.MinWidth(80));
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);
        if(GUILayout.Button("登录", GUILayout.MinWidth(60)))
        {
            MyWindow.window.SetInputAccount(mInput_Account);
            MyWindow.window.SetInputPassword(mInput_Password);
            MyWindow.window.OnLoginClick();
            mInput_Account = "";
            mInput_Password = "";
        }
    }
}

public class MyWindow : ScriptableWizard
{
    public enum SortFlag
    {
        noneFlag = 0,
        sortByName = 1,
        sortByCreateTime = 2,
        sortByStartTime = 3
    };

    public static MyWindow window;
    PanelQuickLogin panelQuickLogin;
    public Vector2 scrollPosition;
    public string longString = "This is a long-ish string";

    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    public bool IsSubmit = false;
    float myFloat = 1.23f;
    WWWHelp wwwh = new WWWHelp();
    public static User user;

    public int listCount = 0;

    [MenuItem("模型上传/UpLoad Window")]
    static void Init()
    {
        window = ScriptableWizard.DisplayWizard<MyWindow>("Title");
        // Get existing open window or if none, make a new one:
        window.minSize = new Vector2(500, 300);
        user = User.CreateInstance("User") as User;
        UserSaveLoad userSL = UserSaveLoad.CreateInstance("UserSaveLoad") as UserSaveLoad;
        userSL.LoadData();
        user.InitUser();
        user.SetUser(userSL.myData._iUser.userID, userSL.myData._iUser.userNickName, userSL.myData._iUser.userName, userSL.myData._iUser.accessToken, userSL.myData._iUser.refreshToken);
        //Debug.LogError(user.nickname);
    }

    bool OpenLodWin = false;
    int listPageCount = 0;

    List<Model> GetModelList(string keywords)
    {
        List<Model> models = new List<Model>();
        HTTPRequest modelsGetReq = new HTTPRequest(new Uri(GlobalConst.UploadBusinessModelPath));
        modelsGetReq.MethodType = BestHTTP.HTTPMethods.Post;
        modelsGetReq.AddHeader("Authorization", "accessToken;charset=utf-8");

        string status = "-1";
        string pagesize = "20";
        string pagenum = listPageCount.ToString();

        ModelReq modelReq = new ModelReq(status, keywords, pagesize, pagenum);
        string reqJson = JsonConvert.SerializeObject(modelReq);
        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(reqJson);
        modelsGetReq.RawData = data;
        return models;
    }

    List<Model> GetModelList()
    {
        List<Model> models = new List<Model>();
        HTTPRequest modelsGetReq = new HTTPRequest(new Uri(GlobalConst.UploadBusinessModelPath));
        modelsGetReq.MethodType = BestHTTP.HTTPMethods.Post;
        modelsGetReq.AddHeader("Authorization", "accessToken;charset=utf-8");

        string status = "-1";
        string keywords = "";
        string pagesize = "20";
        string pagenum = listPageCount.ToString();

        ModelReq modelReq = new ModelReq(status, keywords, pagesize, pagenum);
        string reqJson = JsonConvert.SerializeObject(modelReq);
        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(reqJson);
        modelsGetReq.RawData = data;
        return models;
    }

    void OnRequestGetListFinished(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    ModelReqResponse modelReqResponse = JsonConvert.DeserializeObject<ModelReqResponse>(resp.DataAsText);
                    if (modelReqResponse.result.code == 0)
                    {
                        if()
                    }
                    else
                    {
                        Debug.LogError("参数错误");
                    }
                }
                else if (resp.StatusCode == 401)
                {
                    Debug.LogError("未认证");
                }
                else if (resp.StatusCode == 402)
                {
                    Debug.LogError("无权限");
                }
                break;
        }
    }

    void SortByFlag(ref List<Model> list, SortFlag sortFlag)
    {
        switch (sortFlag)
        {
            case SortFlag.noneFlag: return;

            case SortFlag.sortByStartTime:
                list.Sort(delegate (Model x, Model y)
                {
                    if (x.startMakeTime == null && y.startMakeTime == null) return 0;
                    else if (x.startMakeTime == null) return -1;
                    else if (y.startMakeTime == null) return 1;
                    else return x.startMakeTime.CompareTo(y.startMakeTime);
                });
                break;

            case SortFlag.sortByName:
                list.Sort(delegate (Model x, Model y)
                {
                    if (x.modelName == null && y.modelName == null) return 0;
                    else if (x.modelName == null) return -1;
                    else if (y.modelName == null) return 1;
                    else return x.modelName.CompareTo(y.modelName);
                });
                break;
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("刷新", GUILayout.Width(100));
        OpenLodWin = GUILayout.Button("登录帐号", GUILayout.Width(100));
        GUILayout.Space(70);

        GUILayout.Label("账号：", GUILayout.MaxWidth(35));
        GUILayout.TextField(user == null ? "Null" : user.nickname, GUILayout.MinWidth(80));
        //Debug.LogError(user.nickname);
        GUILayout.Label("角色：", GUILayout.MaxWidth(35));
        GUILayout.TextField("模型制作", GUILayout.MinWidth(80));
        EditorGUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MinWidth(500), GUILayout.MinHeight(200));

        EditorGUILayout.BeginHorizontal();
        float nameW = 50;
        float dayuseW = 20;
        float timeW = 30;
        float statusW = 20;
        float pageW = 20;
        float revieW = 30;
        float lastimeW = 30;
        bool modelNameFlag = GUILayout.Button("模型名称", GUILayout.MinWidth(nameW));
        GUILayout.Button("制作用时", GUILayout.MinWidth(dayuseW));
        GUILayout.Button("状态", GUILayout.MinWidth(statusW));
        GUILayout.Button("详细页面", GUILayout.MinWidth(pageW));
        GUILayout.Button("审核人", GUILayout.MinWidth(revieW));
        bool makeTimeFlag = GUILayout.Button("开始制作时间", GUILayout.MinWidth(timeW));
        GUILayout.Button("最后修改日期", GUILayout.MinWidth(lastimeW));
        SortFlag sortflag = SortFlag.noneFlag;
        if (modelNameFlag)
            sortflag = SortFlag.sortByName;
        if (makeTimeFlag)
            sortflag = SortFlag.sortByStartTime;
        
        modelNameFlag = false;
        makeTimeFlag = false;

        EditorGUILayout.EndHorizontal();
        List<Model> moudels = GetModelList();
        SortByFlag(ref moudels, sortflag);

        for (int i = 0; i < moudels.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(moudels[i].modelName, GUILayout.MinWidth(nameW));
            GUILayout.Label(moudels[i].predictMakeTime, GUILayout.MinWidth(dayuseW));
            GUILayout.Label(moudels[i].status, GUILayout.MinWidth(statusW));
            GUILayout.Button("查看", GUILayout.MinWidth(pageW));
            GUILayout.Button(moudels[i].reviewerName, GUILayout.MinWidth(revieW));
            GUILayout.Button(moudels[i].startMakeTime, GUILayout.MinWidth(timeW));
            GUILayout.Button(moudels[i].lastReviewTime, GUILayout.MinWidth(lastimeW));
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("审核通过", GUILayout.Width(100));
        GUILayout.Button("审核打回", GUILayout.Width(100));
        GUILayout.Space(200);
        GUILayout.Button("上传模型", GUILayout.MinWidth(60));
        EditorGUILayout.EndHorizontal();

        if (OpenLodWin)
        {
            LogWindow LogWin = ScriptableWizard.DisplayWizard<LogWindow>("用户登录");

            LogWin.minSize = new Vector2(300, 200);

            LogWin.OnWizardCreate();

            OpenLodWin = false;
        }
    }

    string mInput_Account;
    string mInput_Password;

    void Update()
    {
        //Debug.Log("Account: " + mInput_Account);
        //Debug.Log("Password: " + mInput_Password);
    }

    public void OnLoginClick()
    {
        panelQuickLogin = PanelQuickLogin.CreateInstance("PanelQuickLogin") as PanelQuickLogin;
        panelQuickLogin.LoginOnClick();
    }

    public string GetInputAccount()
    {
        return mInput_Account;
    }

    public string GetInputPassword()
    {
        return mInput_Password;
    }

    public void SetInputAccount(string mInput_Account)
    {
        this.mInput_Account = mInput_Account;
    }

    public void SetInputPassword(string mInput_Password)
    {
        this.mInput_Password = mInput_Password;
    }

    public void ClearInputAccountValue()
    {
        mInput_Account = "";
    }

    public void ClearInputPasswordValue()
    {
        mInput_Password = "";
    }
}
