﻿using System;

namespace fastCSharp.openApi.weixin
{
    /// <summary>
    /// 交易会话标识
    /// </summary>
    public class appPrePayId : returnSign
    {
#pragma warning disable
        /// <summary>
        /// 调用接口提交的终端设备号
        /// </summary>
        internal string device_info;
#pragma warning restore

        #region 以下字段在return_code 和result_code都为SUCCESS的时候有返回
#pragma warning disable
        /// <summary>
        /// 交易类型
        /// </summary>
        internal tradeType trade_type;
#pragma warning restore
        /// <summary>
        /// 预支付交易会话标识，微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string prepay_id;
        #endregion
        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        internal bool Verify(config config)
        {
            if (IsValue)
            {
                if (appid == config.appid && mch_id == config.mch_id && sign<appPrePayId>.Check(this, config.key, sign)) return true;
                config.PayLog.Add("签名验证错误 " + this.ToJson(), new System.Diagnostics.StackFrame(), false);
            }
            return false;
        }
    }
}
