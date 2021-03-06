﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace fastCSharp.threading
{
    /// <summary>
    /// 死锁检测
    /// </summary>
    public static class lockCheck
    {
        /// <summary>
        /// 未释放锁信息
        /// </summary>
        private struct lockInfo
        {
            /// <summary>
            /// 第一次申请时间
            /// </summary>
            public DateTime LockTime;
            /// <summary>
            /// 最后一次申请堆栈
            /// </summary>
            public StackTrace StackTrace;
            /// <summary>
            /// 申请次数
            /// </summary>
            public int Count;
            /// <summary>
            /// 未释放锁信息
            /// </summary>
            /// <returns>未释放锁信息</returns>
            public override string ToString()
            {
                return LockTime.toString() + "[" + Count.toString() + @"]
" + StackTrace.ToString();
            }
        }
        /// <summary>
        /// 当前未释放的锁
        /// </summary>
        private static readonly Dictionary<objectReference, lockInfo> lockInfos;
        /// <summary>
        /// 访问锁
        /// </summary>
        private static readonly object interlock = new object();
        /// <summary>
        /// 是否输出日志
        /// </summary>
        private static volatile bool isOutput;
        /// <summary>
        /// 输出日志休眠时间
        /// </summary>
        private static TimeSpan sleepTime;
        /// <summary>
        /// 申请锁
        /// </summary>
        /// <param name="value">锁对象, 必须是引用类型且不能为null</param>
        public static void Enter(object value)
        {
            Monitor.Enter(value);
            if (fastCSharp.config.pub.Default.LockCheckMinutes != 0)
            {
                lockInfo lockInfo;
                Monitor.Enter(interlock);
                try
                {
                    if (lockInfos.TryGetValue(new objectReference { Value = value }, out lockInfo))
                    {
                        ++lockInfo.Count;
                        lockInfo.StackTrace = new StackTrace(true);
                        lockInfos[new objectReference { Value = value }] = lockInfo;
                    }
                    else lockInfos.Add(new objectReference { Value = value }, new lockInfo { Count = 1, LockTime = fastCSharp.date.NowSecond, StackTrace = new StackTrace(true) });
                }
                finally { Monitor.Exit(interlock); }
            }
        }
        /// <summary>
        /// 释放锁
        /// </summary>
        /// <param name="value">必须是当前线程已经申请的锁对象, 必须是引用类型且不能为null</param>
        public static void Exit(object value)
        {
            if (fastCSharp.config.pub.Default.LockCheckMinutes != 0)
            {
                lockInfo lockInfo;
                Monitor.Enter(interlock);
                try
                {
                    if (lockInfos.TryGetValue(new objectReference { Value = value }, out lockInfo))
                    {
                        if (--lockInfo.Count == 0) lockInfos.Remove(new objectReference { Value = value });
                        else
                        {
                            lockInfo.StackTrace = new StackTrace(true);
                            lockInfos[new objectReference { Value = value }] = lockInfo;
                        }
                    }
                }
                finally { Monitor.Exit(interlock); }
                if (lockInfo.StackTrace == null) log.Error.Add("没有找到需要释放的锁", null, true);
            }
            Monitor.Exit(value);
        }
        /// <summary>
        /// 释放锁并等待唤醒
        /// </summary>
        /// <param name="value">必须是当前线程已经申请的锁对象, 是引用类型且不能为null</param>
        public static void Wait(object value)
        {
            object newValue = null;
            if (fastCSharp.config.pub.Default.LockCheckMinutes != 0)
            {
                lockInfo lockInfo;
                Monitor.Enter(interlock);
                try
                {
                    if (lockInfos.TryGetValue(new objectReference { Value = value }, out lockInfo))
                    {
                        lockInfo.StackTrace = new StackTrace(true);
                        lockInfos.Add(new objectReference { Value = newValue = new object() }, lockInfo);
                        lockInfos.Remove(new objectReference { Value = value });
                    }
                }
                finally { Monitor.Exit(interlock); }
                if (lockInfo.StackTrace == null) log.Error.Add("没有找到需要释放的锁", null, true);
            }
            Monitor.Wait(value);
            if (newValue != null)
            {
                Monitor.Enter(interlock);
                try
                {
                    lockInfo lockInfo = lockInfos[new objectReference { Value = newValue }];
                    lockInfos.Add(new objectReference { Value = value }, lockInfo);
                    lockInfos.Remove(new objectReference { Value = newValue });
                }
                finally { Monitor.Exit(interlock); }
            }
        }
        /// <summary>
        /// 输出未释放的锁信息
        /// </summary>
        private static void output()
        {
            while (isOutput)
            {
                try
                {
                    DateTime time = fastCSharp.date.NowSecond.AddMinutes(-fastCSharp.config.pub.Default.LockCheckMinutes);
                    lockInfo[] values;
                    Monitor.Enter(interlock);
                    try
                    {
                        values = lockInfos.Values.getArray();
                    }
                    finally { Monitor.Exit(interlock); }
                    list<lockInfo> list = values.toList().remove(value => value.LockTime > time);
                    if (list.count() != 0)
                    {
                        log.Default.Add("未释放锁数量 " + list.Count.toString() + @"
" + list.JoinString(@"
", value => value.ToString()), null, false);
                    }
                }
                catch (Exception error)
                {
                    log.Error.Add(error, null, false);
                }
                Thread.Sleep(sleepTime);
            }
        }
        /// <summary>
        /// 停止输出日志
        /// </summary>
        private static void dispose()
        {
            isOutput = false;
        }
        static lockCheck()
        {
            if (fastCSharp.config.pub.Default.LockCheckMinutes != 0)
            {
                lockInfos = dictionary<objectReference>.Create<lockInfo>();
                isOutput = true;
                sleepTime = new TimeSpan(0, fastCSharp.config.pub.Default.LockCheckMinutes, 0);
                threading.threadPool.TinyPool.Start(output, dispose);
            }
            if (fastCSharp.config.appSetting.IsCheckMemory) checkMemory.Add(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}
