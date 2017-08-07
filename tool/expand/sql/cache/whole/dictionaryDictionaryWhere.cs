﻿using System;
using fastCSharp.code.cSharp;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace fastCSharp.sql.cache.whole
{
    /// <summary>
    /// 分组字典缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="groupKeyType">分组关键字类型</typeparam>
    /// <typeparam name="keyType">字典关键字类型</typeparam>
    public sealed class dictionaryDictionaryWhere<valueType, modelType, groupKeyType, keyType> : dictionaryDictionary<valueType, modelType, groupKeyType, keyType>
        where valueType : class, modelType
        where modelType : class
        where groupKeyType : IEquatable<groupKeyType>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 数据匹配器
        /// </summary>
        private Func<valueType, bool> isValue;
        /// <summary>
        /// 分组字典缓存
        /// </summary>
        /// <param name="cache">整表缓存</param>
        /// <param name="getGroupKey">分组关键字获取器</param>
        /// <param name="getKey">字典关键字获取器</param>
        /// <param name="isValue">数据匹配器</param>
        public dictionaryDictionaryWhere(events.cache<valueType, modelType> cache
            , Func<valueType, groupKeyType> getGroupKey, Func<valueType, keyType> getKey, Func<valueType, bool> isValue)
            : base(cache, getGroupKey, getKey, false)
        {
            if (isValue == null) log.Error.Throw(log.exceptionType.Null);
            this.isValue = isValue;

            cache.OnReset += reset;
            cache.OnInserted += onInserted;
            cache.OnUpdated += onUpdated;
            cache.OnDeleted += onDeleted;
            resetLock();
        }
        /// <summary>
        /// 重新加载数据
        /// </summary>
        protected override void reset()
        {
            groups = dictionary<randomKey<groupKeyType>>.Create<Dictionary<randomKey<keyType>, valueType>>();
            foreach (valueType value in cache.Values) onInserted(value);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="value">数据对象</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void onInserted(valueType value)
        {
            if (isValue(value)) base.onInserted(value);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        private new void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            if (isValue(value))
            {
                if (isValue(oldValue))
                {
                    base.onUpdated(cacheValue, value, oldValue, memberMap);
                }
                else base.onInserted(cacheValue);
            }
            else if (isValue(oldValue)) base.onDeleted(oldValue);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        private new void onDeleted(valueType value)
        {
            if (isValue(value)) base.onDeleted(value);
        }
    }
}