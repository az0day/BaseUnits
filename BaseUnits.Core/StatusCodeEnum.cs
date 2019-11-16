using System;

namespace BaseUnits.Core
{
    /// <summary>
    /// 操作结果
    /// </summary>
    [Serializable]
    public enum StatusCodeEnum
    {
        /// <summary>
        /// 未定义
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// 执行成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 已接受
        /// </summary>
        Accepted = 2,

        /// <summary>
        /// 已支付
        /// </summary>
        Paid = 3,

        /// <summary>
        /// 已发货
        /// </summary>
        Shipped = 4,
        
        /// <summary>
        /// 密码错误
        /// </summary>
        WrongPassword = 21,

        /// <summary>
        /// 该用户不存在
        /// </summary>
        UserNotExist = 22,

        /// <summary>
        /// 该用户名已存在
        /// </summary>
        UserExist = 23,
        
        /// <summary>
        /// 用户被锁
        /// </summary>
        UserLocked = 24,

        /// <summary>
        /// 别名已经存在
        /// </summary>
        NickNameExist = 25,

        /// <summary>
        /// 不是admin用户
        /// </summary>
        NotAdmin = 26,

        /// <summary>
        /// Member
        /// </summary>
        NotMember = 27,

        /// <summary>
        /// 未激活
        /// </summary>
        NoneActivate = 28,


        /// <summary>
        /// 黑名单用户列表
        /// </summary>
        BlackListUser = 41,

        /// <summary>
        /// 密码引起的黑名单
        /// </summary>
        BlackListWrongPassword = 42,

        /// <summary>
        /// 连续密码错误达到锁定状态
        /// </summary>
        LockWrongPassword = 43,

        /// <summary>
        /// 国家限制
        /// </summary>
        LimitCountry = 61,

        /// <summary>
        /// 用户限定只能登录固定域名
        /// </summary>
        LimitDomain = 62,

        /// <summary>
        /// 特定的国家只能登录特定的品牌
        /// </summary>
        LimitCountryBrand = 63,

        /// <summary>
        /// 用户限定只能登录固定品牌
        /// </summary>
        LimitBrand = 64,

        /// <summary>
        /// 地区限制
        /// </summary>
        LimitLocation = 65,

        // 200 - 500 http status codes

        OK = 200,
        Created = 201,
        NonAuthoritative = 203,
        NoContent = 204,
        MovedPermanently = 301,
        Found = 302,
        NotModified = 304,
        TemporaryRedirect = 307,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        RequestTimeout = 408,
        InternalServerError = 500,

        // 200 - 500 http status codes
        
        /// <summary>
        /// 服务器错误
        /// </summary>
        ServerError = 991,

        /// <summary>
        /// 参数错误
        /// </summary>
        InvalidArgs = 992,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 999,
    }
}
