using System;
//using Mono.Data.Sqlite;
using System.Collections;
using Newtonsoft.Json;
using BestHTTP;
using UnityEngine;
using System.Collections;

public class Result
{

    public int code = -1;
    public string description;
}

public class UserRefreshToken
{
    public string accessToken;
    public string refreshToken;
    public string client;
}

public class UserRefreshTokenResponse
{
    public Result result;
    public string accessToken;
    public string refreshToken;
    public string accessTokenExpiredTime;
    public string refreshTokenExpiredTime;

}

public class UserInfo
{
    
}

public class UserInfoResponse
{
    public Result result;
    public User user;
}

public class UpdateUserInfoRequest
{
    public string nickname;
}

public class UpdateUserInfoResponse
{
    public Result result;
}

public class User : Singleton<User>
{
    //userid
    public string id;
    //用户名
    public string username;
    //登陆令牌
    public string accessToken;
    //刷新令牌
    public string refreshToken;
    //设备标识
    public string deviceFlag;
    ///
    public string accountType;
    ///昵称
    public string nickname;

    ///<summary>设置user信息</summary>
    public void SetUser(string id, string nickname, string name, string accessToken, string refreshToken, string deviceFlag = null)
    {
        this.id = id;
        this.nickname = nickname;
        this.username = name;
        this.accessToken = accessToken;
        this.refreshToken = refreshToken;
        if (deviceFlag != null)
            this.deviceFlag = deviceFlag;
    }

    public void InitUser()
    {
        if (IsLoggedin())
        {
            UserSaveLoad userSL = new UserSaveLoad();
            userSL.LoadData();
            HTTPRequest request = new HTTPRequest(new Uri(GlobalConst.UserLoginPath), OnRefreshRequestFinished);
            request.MethodType = BestHTTP.HTTPMethods.Post;
            request.AddHeader("Content-Type", "application/json;charset=utf-8");
            UserRefreshToken urt = new UserRefreshToken();
            urt.accessToken = userSL.myData._iUser.accessToken;
            urt.refreshToken = userSL.myData._iUser.refreshToken;
            urt.client = GlobalConst.ClientFlag + "-" + deviceFlag;
            string parm = JsonConvert.SerializeObject(urt);
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(parm);
            request.RawData = data;
        }
    }

    void OnRefreshRequestFinished(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    Debug.Log(resp.DataAsText);

                    Debug.Log("Success");
                    UserRefreshTokenResponse urtr = JsonConvert.DeserializeObject<UserRefreshTokenResponse>(resp.DataAsText);
                    if (urtr.result.code == 0)
                    {
                        User user = User.CreateInstance("User") as User;
                        user.accessToken = urtr.accessToken;
                        user.refreshToken = urtr.refreshToken;
                        user.SaveUser();
                        //yield return StartCoroutine(GetUserInfo
                        GetUserInfo();
                        if (string.IsNullOrEmpty(nickname))
                        {
                            //UIManager.Instance.SetVisible(UIName.PanelSetNickname, true);
                        }
                    }
                    else
                    {
                        Debug.LogError("resultCode:" + urtr.result.code + "  " + urtr.result.description);
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

    ///初始化User
    public IEnumerator sInitUser()
    {
        //OperatingDB.Instance.GetUser();
        if (IsLoggedin())
        {
            ///刷新accessToken
            WWWHelp www = new WWWHelp();
            UserRefreshToken urt = new UserRefreshToken();
            urt.accessToken = User.Instance.accessToken;
            urt.refreshToken = User.Instance.refreshToken;
            urt.client = GlobalConst.ClientFlag + "-" + User.Instance.deviceFlag;
            string parm = JsonConvert.SerializeObject(urt);
            //yield return StartCoroutine(www.WWWPostLoad(GlobalConst.UserRefreshTokenPath, parm));
            if (www.CheckWWW())
            {
                UserRefreshTokenResponse urtr = JsonConvert.DeserializeObject<UserRefreshTokenResponse>(www.text);
                if (urtr.result.code == 0)
                {
                    User.Instance.accessToken = urtr.accessToken;
                    User.Instance.refreshToken = urtr.refreshToken;
                    SaveUser();
                    yield return 0;
                    //yield return StartCoroutine(GetUserInfo());
                    if (string.IsNullOrEmpty(nickname))
                    {
                        //UIManager.Instance.SetVisible(UIName.PanelSetNickname, true);
                    }
                }
                else
                {
                    Debug.Log("resultCode:" + urtr.result.code + "  " + urtr.result.description);
                }
            }
            www.Dispose();
        }
    }

    ///<summary>保存用户信息到数据库</summary>
    public void SaveUser()
    {
        //OperatingDB.Instance.SaveUser();
        UserSaveLoad save = new UserSaveLoad();
        
        save.SaveData(this);
    }

    ///检测是否已登录
    public bool IsLoggedin()
    {
        UserSaveLoad userSL = new UserSaveLoad();
        userSL.LoadData();
        if (string.IsNullOrEmpty(userSL.myData._iUser.userID))
            return false;
        else
            return true;
    }

    public void GetUserInfo()
    {
        if (!IsLoggedin())
            return;
        HTTPRequest getInfoReq = new HTTPRequest(new Uri(GlobalConst.USERPATH), OnGetInfoFinished);
        getInfoReq.AddHeader("Content-Type", "application/json;charset=utf-8");
    }

    

    void OnGetInfoFinished(HTTPRequest req, HTTPResponse resp)
    {
        switch (req.State)
        {
            case HTTPRequestStates.Finished:
                if (resp.IsSuccess)
                {
                    UserInfoResponse uir = JsonConvert.DeserializeObject<UserInfoResponse>(resp.DataAsText);
                    if (uir.result.code == 0)
                    {
                        Debug.Log("获取用户信息成功");
                        if (!uir.user.nickname.Equals(nickname))
                        {
                            nickname = uir.user.nickname;
                            
                        }
                        SaveUser();
                    }
                    else if(uir.result.code == 1000)
                    {
                        Debug.Log("resultCode:" + uir.result.code + "  " + uir.result.description);
                    }
                    else if(uir.result.code == 1004)
                    {
                        Debug.Log("resultCode:" + uir.result.code + "  " + uir.result.description);
                    }
                }
                else
                {
                    Debug.Log("Get user info failed");
                }
                break;
        }
    }

    public IEnumerator sGetUserInfo()
    {
        if (!IsLoggedin())
            yield break;
        WWWHelp www = new WWWHelp();
        //yield return StartCoroutine(www.WWWGetLoad(GlobalConst.USERPATH + User.Instance.id + GlobalConst.GETUSERINFO, new string[] { "Authorization" }, new string[] { User.Instance.accessToken }));
        if (www.CheckWWW())
        {
            UserInfoResponse uir = JsonConvert.DeserializeObject<UserInfoResponse>(www.text);
            if (uir.result.code == 0)
            {
                Debug.Log("获取用户信息成功");
                if (!uir.user.nickname.Equals(nickname))
                {
                    nickname = uir.user.nickname;
                    SaveUser();
                }
            }
            else if (uir.result.code == 1000)
            {
                Debug.Log("resultCode:" + uir.result.code + "  " + uir.result.description);
            }
            else if (uir.result.code == 1004)
            {
                Debug.Log("resultCode:" + uir.result.code + "  " + uir.result.description);
            }
            else
            {
                Debug.Log("resultCode:" + uir.result.code + "  " + uir.result.description);
            }
        }
        www.Dispose();
    }

    void CleanUserData()
    {
        //for (int i = 0; i < UIDataPool.Instance.myPlansList.Count; i++)
        //{
        //    UIDataPool.Instance.myPlansList[i].DeleteLocalData();
        //}
        //UIDataPool.Instance.myPlansList.Clear();
        //OperatingDB.Instance.RemoveMyPlans();
        UserSaveLoad save = new UserSaveLoad();

        save.ClearData();
    }

    ///用户登出处理
    public void LogoutUser()
    {
        //OperatingDB.Instance.RemoveUser();
        //OperatingDB.Instance.GetUser();
        CleanUserData();
        //UICacheManager.Instance.ClearCache(TextureType.myPlans);
    }

    ///用户登录过期处理方法
    public void TokenExpired()
    {
        LogoutUser();
        Debug.Log("登录过期");
        //MyWindow.mPanelQuickLogin.SetPopupType(PopupType.TopToCenter);
        //MyWindow.mPanelQuickLogin.SetLoginTips("登录过期，请重新登录帐号");
        //UIManager.Instance.SetVisible(UIName.PanelQuickLogin, true);
    }
}
