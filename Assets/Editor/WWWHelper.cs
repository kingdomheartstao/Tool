using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WWWHelp
{

    public WWW www;
    public string text { get { return www.text; } }
    public Dictionary<string, string> responseHeader { private set; get; }
    public string error { private set; get; }
    // public Texture2D texture{get{return www.texture;}}
    public int timeOut = 10;
    private bool isDispose = false;
    public string currentPath = "";

    public void Dispose()
    {
        if (!isDispose)
        {
            www.Dispose();
            isDispose = true;
        }
    }

    ///显示Loading界面
    // private void ShowLoading(string loadingText = "loading"){
    //     UIManager.Instance.ShowLoading(delegate(ref float progress,ref bool loading,ref bool fail){
    //         progress += www.progress;
    //         Debug.Log("progress:" + loading +"  " + www.isDone + "   " + www.progress + "   " + www.url);
    //         loading = !(loading & www.isDone);
    //         fail = error !=null;
    //     },true,loadingText);
    // }
    // 
    // private void ShowUploadLoading(string loadingText = "loading"){
    //     UIManager.Instance.ShowLoading(delegate(ref float progress,ref bool loading ,ref bool fail){
    //         progress += www.uploadProgress;
    //         Debug.Log("progress:" + loading +"  " + www.isDone + "   www.progress:" + www.progress + " www.uploadProgress:" + www.uploadProgress + "   " + www.url);
    //         loading = !(loading & www.isDone);
    //         fail = error !=null;
    //         
    //     },true,loadingText);
    // }

    ///计算time out 时间方法
    public IEnumerator CountTimeOut(int StartTime = 0)
    {
        int time = StartTime;
        while (time < timeOut)
        {
            yield return new WaitForSeconds(1);
            time++;
        }
        if (!isDispose && !www.isDone)
        {
            Dispose();
            error = "time out!";
        }
        // else{
        //     Debug.Log("no time out");
        // }
    }

    ///get请求方法
    public IEnumerator WWWGetLoad(string path, string[] parmName, string[] parmValue, bool showLoading = false, string loadingText = "loading")
    {
        Dictionary<string, string> header = new Dictionary<string, string>();
        if (parmName.Length == parmValue.Length)
        {
            for (int i = 0; i < parmName.Length; i++)
            {
                header.Add(parmName[i], parmValue[i]);//设置http请求头
            }
        }
        else
        {
            Debug.LogError("参数数量不一致");
        }
        Debug.Log(path);
        currentPath = path;
        www = new WWW(path, null, header);
        // if(showLoading)
        //     ShowLoading(loadingText);
        yield return www;
        if (!isDispose && www.isDone)
        {
            error = www.error != null ? www.error : null;
            responseHeader = www.responseHeaders;
        }
    }

    ///get请求方法
    public IEnumerator WWWGetLoad(string path, bool showLoading = false, string loadingText = "loading")
    {
        Debug.Log(path);
        www = new WWW(path);
        // if(showLoading)
        //     ShowLoading(loadingText);
        currentPath = path;
        yield return www;
        if (!isDispose && www.isDone)
        {
            error = www.error != null ? www.error : null;
            responseHeader = www.responseHeaders;
        }
    }

    ///post请求方法
    public IEnumerator WWWPostLoad(string path, string parmJson, bool showLoading = false, string loadingText = "loading")
    {
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json;charset=utf-8");//设置http请求头
        Debug.Log(parmJson);
        Debug.Log(path);
        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(parmJson);
        www = new WWW(path, data, header);
        //  if(showLoading)
        //     ShowLoading(loadingText);
        currentPath = path;
        yield return www;
        if (!isDispose && www.isDone)
        {
            error = www.error != null ? www.error : null;
            responseHeader = www.responseHeaders;
        }
    }

    ///post请求方法,有accessToken
    public IEnumerator WWWPostLoad(string path, string parmJson, string accessToken, bool showLoading = false, string loadingText = "loading")
    {
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json;charset=utf-8");//设置http请求头
        header.Add("Authorization", accessToken);//设置http请求头
        Debug.Log("Authorization:" + accessToken);
        Debug.Log(parmJson);
        Debug.Log(path);
        // byte[] data = System.Text.Encoding.Default.GetBytes(parmJson);
        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(parmJson);
        currentPath = path;
        www = new WWW(path, data, header);
        // if(showLoading)
        //     ShowLoading(loadingText);
        yield return www;
        if (!isDispose && www.isDone)
        {
            error = www.error != null ? www.error : null;
            responseHeader = www.responseHeaders;
        }
    }

    ///传文件post请求方法
    public IEnumerator WWWFormPostLoad(string path, WWWForm wwwform, string[] parmName, string[] parmValue, bool showLoading = false, string loadingText = "loading")
    {
        Dictionary<string, string> header = new Dictionary<string, string>();
        if (parmName.Length == parmValue.Length)
        {
            for (int i = 0; i < parmName.Length; i++)
            {
                header.Add(parmName[i], parmValue[i]);//设置http请求头
            }
        }
        else
        {
            Debug.LogError("参数数量不一致");
        }
        header.Add("Content-Type", wwwform.headers["Content-Type"]);
        //  Debug.Log(header["Content-Type"]+" "+header["Authorization"]);
        byte[] byteData = wwwform.data;
        Debug.Log(path + "   data: " + System.Text.Encoding.UTF8.GetString(byteData));
        www = new WWW(path, byteData, header);
        // if(showLoading)
        //     ShowUploadLoading(loadingText);
        currentPath = path;
        yield return www;
        if (!isDispose && www.isDone)
        {
            error = www.error != null ? www.error : null;
            responseHeader = www.responseHeaders;
        }
    }

    ///检查www是否访问成功
    public bool CheckWWW()
    {
        if (error != null && error.Contains("time out"))
        {
            //UIManager.Instance.ShowMessageBox("网络异常", 4f);
            // UIManager.Instance.SetVisible(UIName.PanelNetWorkError,true);
            return false;
        }
        else
        {
            if (www.isDone)
            {
                //检查网络访问是否有错唔信息返回
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.Log("error:" + error);
                    //error.Substring(0,3) == "401" ||
                    if (error.Contains("unauthorized") || error.Contains("Unauthorized"))
                    {
                        Debug.Log("accessToken过期");
                        User.Instance.TokenExpired();
                    }
                    else if (error.Contains("404"))
                    {
                        Debug.Log("error:" + error);
                    }
                    else if (error.Contains("Couldn't resolve host") || error.Contains("resolve host"))
                    {
                        //UIManager.Instance.ShowMessageBox("网络异常", 4f);
                        // UIManager.Instance.SetVisible(UIName.PanelNetWorkError,true);
                    }
                    else if (error.Contains("time out") || error.Contains("Time Out"))
                    {
                        //UIManager.Instance.ShowMessageBox("网络异常", 4f);
                        // UIManager.Instance.SetVisible(UIName.PanelNetWorkError,true);
                    }
                    return false;
                }
                else
                {
                    if (responseHeader.ContainsKey("STATUS"))
                    {
                        //检查status code
                        string[] status = responseHeader["STATUS"].Split(' ');
                        if (status[1] == "200")
                        {
                            //访问成功
                            return true;
                        }
                        else if (status[1] == "401")
                        {
                            Debug.Log("accessToken过期");

                            return false;
                        }
                        else
                        {
                            //status code 服务器错误
                            Debug.Log("status code错误");
                            return false;
                        }
                    }
                    else
                    {
                        // Debug.Log("本地读取资源");
                        return true;
                    }
                }
            }
            else
            {
                //www访问未完成
                Debug.Log("www访问未完成");
                return false;
            }
        }
    }
}
