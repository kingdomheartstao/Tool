using System.Text;
using UnityEngine;
///全局常量
public class GlobalConst
{
    ///客户端标识
    public const string ClientFlag = "Windows-editor";
    ///客户端版本号
    public const string ClientVersion = "0.1";
    ///数据库版本号
    public const string DBVERSION = "4";
    ///服务器地址
    public const string BusinessModelServerPath = "http://192.168.1.220:8081/daedaluse-business-api/";
    public const string ServerPath = "http://testapi.dodojia.cn:8080/daedaluse-restful-api/v1";
    ///加密盐字符串
    public const string salt = "LSda2015";
    ///上传模型
    public const string UploadBusinessModelPath = BusinessModelServerPath + "model/upload";
    ///获取模型列表
    public const string GetBusinessModelListPath = BusinessModelServerPath + "model/list";
    ///获取资源库及户型版本号
    public const string GETVERSION = ServerPath + "/version/get_versions";
    ///用户登录访问地址
    public const string UserLoginPath = ServerPath + "/users/login";
    public const string UserLogoutPath = ServerPath + "/users/logout";
    ///确认验证码是否正确
    public const string VERIFY_MOBIILE_CODE_PATH = ServerPath + "/users/verify_mobile_code";
    ///发送验证码到手机
    public const string GET_VERIFY_MOBIILE_CODE_PATH = ServerPath + "/users/get_verify_code_by_mobile";
    ///找回密码(手机)-重设密码
    public const string RESET_PASSWORD_BY_MOBILE_PATH = ServerPath + "/users/reset_password_by_mobile";
    ///刷新帐号验证令牌
    public const string UserRefreshTokenPath = ServerPath + "/users/refresh_token";
    ///用户注册访问地址
    public const string UserRegisterPath = ServerPath + "/users/register";
    ///用户重设密码访问地址
    public const string USERRESTPASSWORD = ServerPath + "/users/reset_password_client";
    ///用户忘记密码访问地址
    public const string UserRequestPasswordPath = ServerPath + "/users/forget_password";
    ///获取预设方案列表地址
    public const string PRESETPLANSLIST = ServerPath + "/plans/preset/list";
    ///获取我的方案列表地址
    //public const string MYPLANSLIST = ServerPath + "/plans/users/";
    ///我的方案地址
    public const string MYPLANS = ServerPath + "/plans/users/";
    ///用户访问前缀地址
    public const string USERPATH = ServerPath + "/users/";
    ///用户更新元信息地址
    public const string UPDATEUSERINFO = "/update_info";
    ///用户获取元信息地址
    public const string GETUSERINFO = "/get_info";
    ///我的方案保存
    public const string MYPLANSSAVE = "/upload";
    ///我的方案更新
    public const string MYPLANSUPDATE = "/update";
    public const string MYPLANSUPDATEMETA = "/update_meta";
    ///我的方案删除
    public const string MYPLANSREMOVE = "/remove";
    ///我的方案列表
    public const string MYPLANSList = "/get";
    ///获取商品种类列表地址
#if UNITY_EDITOR
    public const string ProductListType = ServerPath + "/products/list_type?showAll=1";
#elif UNITY_ANDROID || UNITY_IPHONE
    public const string ProductListType = ServerPath + "/products/list_type";
#else 
    public const string ProductListType = ServerPath + "/products/list_type";
#endif
    ///获取商品风格列表地址
    public const string ProductListStyle = ServerPath + "/products/list_style";
    ///获取默认默认门窗
    public const string DEFAULTPRODUCTLIST = ServerPath + "/products/list_default_products";
    ///获取商品列表地址
    public const string ProductList = ServerPath + "/products/list_product";
    ///获取商品详细信息地址
    public const string ProductMetaInfo = ServerPath + "/products/product_meta_info";
    public const string PRODUCTTAGLIST = ServerPath + "/products/list_tag";
    ///意见反馈提交地址
    public const string SENDSUGGESTIONPATH = ServerPath + "/suggestion/send_suggestion";
    ///系统关于地址
    public const string SystemAboutPath = "";
    ///本地数据库名
    public const string DBName = "DaedalusDB";
    ///图片保存格式
    public const string IMAGESUFFIX = ".jpg";
    ///我的方案户型数据本地保存路径
    public const string MYPLANFLOORPATH = "/U3dFloorData/MyPlan/";
    ///我的方案本地保存路径
    public const string MYPLANDATASPATH = "/U3dData/MyPlan/";
    ///我的方案图片本地保存路径
    public const string MYPLANIMAGESPATH = "/U3dImage/MyPlan/";
    ///预设方案户型数据本地保存路径
    public const string PRESETPLANFLOORPATH = "/U3dFloorData/PresetPlan/";
    ///预设方案本地保存路径
    public const string PRESETPLANDATASPATH = "/U3dData/PresetPlan/";
    ///预设方案图片本地保存路径
    public const string PRESETPLANIMAGESPATH = "/U3dImage/PresetPlan/";

    ///文件数据访问地址前缀
    public const string FILEDATAPATH = "http://daedalus-test.oss-cn-shenzhen.aliyuncs.com/";
    ///图片数据访问地址前缀
    public const string IMGPATH = "http://cdn-test.dodojia.cn/";
    public const string ASSETBUNBLEPATH = "http://cdn-assets-test.dodojia.cn/";
    ///商品普通图后缀
    public const string PRODUCTIMGNORMAL = "@!product-normal";
    ///商品缩略图后缀
    public const string PRODUCTIMGTHUMBNAIL = "@!product-thumbnail";
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    public static string savePath = Application.streamingAssetsPath;
#elif UNITY_EDITOR
    public static string savePath = Application.persistentDataPath.Replace("_________", "Daedalus_unity");
#elif UNITY_ANDROID || UNITY_IPHONE
	public static string savePath = Application.persistentDataPath;
#else 
    public static string savePath = Application.persistentDataPath;
#endif
}