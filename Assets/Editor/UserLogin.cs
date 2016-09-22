using UnityEngine;
using System.Collections;

public class UserLogin
{
    public string username;
    public string password;
    public string client;
    public string version;

    public UserLogin(string username, string password, string client, string version)
    {
        this.username = username;
        this.password = password;
        this.client = client;
        this.version = version;
    }

}
public class UserLoginResponse
{
    public Result result;
    public string accessToken;
    public string refreshToken;
    public string accessTokenExpiredTime;
    public string refreshTokenExpiredTime;
    public UserResponse user;

}

public class UserResponse
{
    public string userId;
    public string nickname;
    public string username;
    public string email;
}


public class ModelReq
{
    public string status;
    public string kewords;
    public string pagesize;
    public string pagenum;

    public ModelReq(string status, string kewords, string pagesize, string pagenum)
    {
        this.status = status;
        this.kewords = kewords;
        this.pagesize = pagesize;
        this.pagenum = pagenum;
    }
}

public class ModelReqResponse
{
    public Result result;
    public string modelId;
    public string status;
    public string reviewerId;
    public string reviewerName;
    public string makerId;
    public string makerName;
    public string createTime;
    public string startMakeTime;
    public string finishMakeTime;
    public string lastReviewTime;
    public string predictMakeTime;
    public string businessUserId;
    public string businessUsername;
    public string modelName;
    public string genAssetName;
}