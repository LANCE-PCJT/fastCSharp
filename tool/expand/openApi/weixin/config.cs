﻿using System;
using System.Text;
using System.Security.Cryptography;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 应用配置
    /// </summary>
    public sealed class config
    {
        /// <summary>
        /// 编码绑定请求
        /// </summary>
        internal static readonly encodingRequest Request = new encodingRequest(openApi.request.Default, Encoding.UTF8);
#pragma warning disable
        /// <summary>
        /// appid是微信公众账号或开放平台APP的唯一标识，在公众平台申请公众账号或者在开放平台申请APP账号后，微信会自动分配对应的appid，用于标识该应用。可在微信公众平台-->开发者中心查看，商户的微信支付审核通过邮件中也会包含该字段值。
        /// </summary>
        internal string appid;
        /// <summary>
        /// mch_id
        /// </summary>
        internal string partnerid
        {
            get { return mch_id; }
        }
        /// <summary>
        /// 应用密钥 是APPID对应的接口密码，用于获取接口调用凭证access_token时使用。在微信支付中，先通过OAuth2.0接口获取用户openid，此openid用于微信内网页支付模式下单接口使用。在开发模式中获取AppSecret（成为开发者且帐号没有异常状态）。
        /// </summary>
        private string secret;
        /// <summary>
        /// 自定义令牌
        /// </summary>
        private string token;
        /// <summary>
        /// 微信支付商户号，商户申请微信支付后，由微信支付分配的商户收款账号。new X509Certificate("apiclient.p12", mch_id)
        /// </summary>
        internal string mch_id;
        /// <summary>
        /// API密钥，交易过程生成签名的密钥，仅保留在商户系统和微信支付后台，不会在网络中传播。商户妥善保管该Key，切勿在网络中传输，不能在其他客户端中存储，保证key不会被泄漏。商户可根据邮件提示登录微信商户平台进行设置。也可按一下路径设置：微信商户平台(pay.weixin.qq.com)-->账户设置-->API安全-->密钥设置
        /// 商户如果使用.NET环境开发，请确认Framework版本大于2.0，必须在操作系统上双击安装证书apiclient_cert.p12后才能被正常调用。
        /// </summary>
        internal string key;
        /// <summary>
        /// 是否独立支付日志
        /// </summary>
        internal bool IsPayLog = true;
#pragma warning restore
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <returns>访问令牌,失败返回null</returns>
        internal token GetToken()
        {
            return Request.Request<token>("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + secret);
        }
        /// <summary>
        /// 微信服务器验证
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <returns></returns>
        public bool Verify(string signature, string timestamp, string nonce)
        {
            if (signature.length() == 40)
            {
                string value;
                if (string.Compare(nonce, timestamp, StringComparison.Ordinal) >= 0)
                {
                    string other = nonce;
                    nonce = timestamp;
                    timestamp = other;
                }
                if (string.Compare(timestamp, token, StringComparison.Ordinal) >= 0)
                {
                    value = timestamp;
                    if (string.Compare(nonce, token, StringComparison.Ordinal) >= 0)
                    {
                        timestamp = nonce;
                        nonce = token;
                    }
                    else timestamp = token;
                }
                else value = token;
                if (fastCSharp.unsafer.memory.CheckLowerHex(fastCSharp.pub.Sha1(fastCSharp.unsafer.String.ConcatBytes(nonce, timestamp, value)), signature)) return true;
            }
            log.Default.Add("微信服务器验证错误 signature[" + signature + @"] timestamp[" + timestamp + "] nonce[" + nonce + "] token[" + token + "]", new System.Diagnostics.StackFrame(), false);
            return false;
        }
        /// <summary>
        /// 时间戳
        /// </summary>
        internal static readonly DateTime MinTime = new DateTime(1970, 1, 1);
        /// <summary>
        /// 获取商品二维码URL
        /// </summary>
        private sealed class productUrlSign : signQuery
        {
            /// <summary>
            /// 商品ID
            /// </summary>
            public string product_id;
            /// <summary>
            /// 时间戳,1970/1/1经过的秒数
            /// </summary>
            public long time_stamp = (long)(date.UtcNow - MinTime).TotalSeconds;
        }
        /// <summary>
        /// 获取商品二维码URL
        /// </summary>
        /// <param name="product_id"></param>
        /// <returns></returns>
        public unsafe string GetProductUrl(string product_id)
        {
            productUrlSign sign = new productUrlSign { product_id = product_id };
            sign.setConfig(this);
            memoryPool.pushSubArray data = sign<productUrlSign>.GetData(sign, key);
            try
            {
                fixed (byte* bufferFixed = data.UnsafeArray)
                {
                    using (MD5 md5 = new MD5CryptoServiceProvider())
                    {
                        return "weixin://wxpay/bizpayurl?" + fastCSharp.String.UnsafeDeSerialize(bufferFixed, -data.SubArray.Count) + "&sign=" + sign.sign;
                    }
                }
            }
            finally { data.Push(); }
        }

        /// <summary>
        /// 默认配置
        /// </summary>
        public static readonly config Default = fastCSharp.config.pub.LoadConfig<config>(new config());
        /// <summary>
        /// 微信支付日志
        /// </summary>
        public static readonly log PayLog = Default.IsPayLog ? new log(fastCSharp.config.appSetting.IsLogConsole ? null : fastCSharp.config.appSetting.LogPath + log.DefaultFilePrefix + "weixin.txt", fastCSharp.config.appSetting.MaxLogCacheCount) : log.Error;
    }
}
