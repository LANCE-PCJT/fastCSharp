﻿using System;

namespace fastCSharp.openApi.renren
{
    /// <summary>
    /// API调用http://wiki.dev.renren.com/wiki/Authorization#.E6.9C.8D.E5.8A.A1.E7.AB.AF.E6.B5.81.E7.A8.8B
    /// </summary>
    public class api
    {
        /// <summary>
        /// 应用配置
        /// </summary>
        private config config;
        /// <summary>
        /// 访问令牌
        /// </summary>
        private token token;
        /// <summary>
        /// 访问令牌用户
        /// </summary>
        public tokenUser User
        {
            get { return token.user; }
        }
        /// <summary>
        /// 访问令牌+用户身份的标识
        /// </summary>
        public refreshToken RefreshToken
        {
            get
            {
                return new refreshToken { access_token = token.access_token, refresh_token = token.refresh_token, id = token.user.id };
            }
        }
        /// <summary>
        /// API调用
        /// </summary>
        /// <param name="config">应用配置</param>
        /// <param name="token">访问令牌</param>
        internal api(config config, token token)
        {
            this.config = config;
            this.token = token;
        }
        /// <summary>
        /// 表单提交
        /// </summary>
        /// <typeparam name="jsonType">json数据数据类型</typeparam>
        /// <typeparam name="formType">表单数据类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="form">POST表单</param>
        /// <returns>数据对象,失败放回null</returns>
        private jsonType form<jsonType, formType>(string url, formType form)
            where jsonType : class, IValue
            where formType : apiForm
        {
            form.access_token = token.access_token;
            return config.Request.RequestForm<jsonType, formType>(url, form);
        }
        /// <summary>
        /// 发表一个网页分享，分享应用中的内容给好友
        /// </summary>
        /// <param name="value">网页分享</param>
        /// <returns>是否成功,失败返回null</returns>
        public share AddShare(shareQuery value)
        {
            value.method = "share.share";
            return form<share, shareQuery>(@"https://api.renren.com/restserver.do", value);
        }
        //string query = "access_token=" + accessToken + "&method=users.getInfo&fields=uid,name,sex,mainurl&call_id=" + DateTime.Now.Ticks.ToString() + @"&v=1.0&format=JSON";
        //using (MD5 md5 = new MD5CryptoServiceProvider())
        //{
        //    url = new Uri("http://api.renren.com/restserver.do?" + query + "&sig" + md5.ComputeHash((query + fastCSharp.config.web.login.renren.secretKey).bytes()).toLowerHex());
        //}
        //data = null;
        //Monitor.Enter(WebClientLock);
        //try
        //{
        //    data = WebClient.DownloadData(url);
        //}
        //catch (Exception error)
        //{
        //    log.Default.Add(error, null, true);
        //}
    }
}
