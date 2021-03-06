﻿using System;
using fastCSharp.code.cSharp;
using System.Threading;
using System.Linq.Expressions;

namespace fastCSharp.sql.cache.part
{
    /// <summary>
    /// 先进先出优先队列 列表缓存
    /// </summary>
    /// <typeparam name="valueType">表格绑定类型</typeparam>
    /// <typeparam name="modelType">表格模型类型</typeparam>
    /// <typeparam name="counterKeyType">缓存统计关键字类型</typeparam>
    /// <typeparam name="keyType">关键字类型</typeparam>
    public class queueList<valueType, modelType, counterKeyType, keyType>
        : queueExpression<valueType, modelType, counterKeyType, keyType, list<valueType>>
        where valueType : class, modelType
        where modelType : class
        where counterKeyType : IEquatable<counterKeyType>
        where keyType : IEquatable<keyType>
    {
        /// <summary>
        /// 先进先出优先队列 列表缓存
        /// </summary>
        /// <param name="counter">缓存计数器</param>
        /// <param name="getKey">缓存关键字获取器</param>
        /// <param name="getWhere">条件表达式获取器</param>
        /// <param name="maxCount">缓存默认最大容器大小</param>
        public queueList(events.counter<valueType, modelType, counterKeyType> counter
            , Expression<Func<modelType, keyType>> getKey, Func<keyType, Expression<Func<modelType, bool>>> getWhere, int maxCount = 0)
            : base(counter, getKey, maxCount, getWhere)
        {
            counter.OnReset += reset;
            counter.SqlTool.OnInsertedLock += onInserted;
            counter.OnUpdated += onUpdated;
            counter.OnDeleted += onDeleted;
        }
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="value">新增的数据</param>
        private void onInserted(valueType value)
        {
            keyType key = getKey(value);
            list<valueType> values = queueCache.Get(key, null);
            if (values != null) values.Add(counter.Add(value));
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="cacheValue">缓存数据</param>
        /// <param name="value">更新后的数据</param>
        /// <param name="oldValue">更新前的数据</param>
        private void onUpdated(valueType cacheValue, valueType value, valueType oldValue, fastCSharp.code.memberMap<modelType> memberMap)
        {
            keyType key = getKey(value);
            if (cacheValue == null)
            {
                list<valueType> values;
                if (queueCache.Remove(key, out values))
                {
                    foreach (valueType removeValue in values) counter.Remove(removeValue);
                }
            }
            else
            {
                keyType oldKey = getKey(oldValue);
                if (!key.Equals(oldKey))
                {
                    list<valueType> values = queueCache.Get(key, null), oldValues = queueCache.Get(oldKey, null);
                    if (values != null)
                    {
                        if (oldValues != null)
                        {
                            values.Add(cacheValue);
                            if (!oldValues.Remove(cacheValue)) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                        }
                        else values.Add(counter.Add(cacheValue));
                    }
                    else if (oldValues != null)
                    {
                        if (oldValues.Remove(cacheValue)) counter.Remove(cacheValue);
                        else log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
                    }
                }
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="value">被删除的数据</param>
        private void onDeleted(valueType value)
        {
            list<valueType> values = queueCache.Get(getKey(value), null);
            if (values != null && !values.Remove(value)) log.Error.Add(typeof(valueType).FullName + " 缓存同步错误", null, true);
        }
        /// <summary>
        /// 读取数据库数据列表
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据列表</returns>
        private list<valueType> getList(keyType key)
        {
            list<valueType> values = counter.SqlTool.Where(getWhere(key), counter.MemberMap).getList();
            if (values != null)
            {
                if (values.Count != 0)
                {
                    int index = 0, count = values.Count;
                    valueType[] array = values.UnsafeArray;
                    foreach (valueType value in array)
                    {
                        array[index] = counter.Add(value);
                        if (++index == count) break;
                    }
                }
            }
            else values = new list<valueType>();
            queueCache[key] = values;
            if (queueCache.Count > maxCount)
            {
                foreach (valueType value in queueCache.UnsafePopValue()) counter.Remove(value);
            }
            return values;
        }
        /// <summary>
        /// 删除缓存数据
        /// </summary>
        /// <param name="key"></param>
        public void Remove(keyType key)
        {
            list<valueType> values;
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                queueCache.Remove(key, out values);
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            if (values != null)
            {
                foreach (valueType value in values) counter.Remove(value);
            }
        }
        /// <summary>
        /// 获取一个匹配数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>匹配数据,失败返回null</returns>
        public valueType FirstOrDefaultLock(keyType key, Func<valueType, bool> isValue)
        {
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                list<valueType> values = queueCache.Get(key, null);
                if (values != null) return values.FirstOrDefault(isValue);
                values = getList(key);
                if (values != null) return values.FirstOrDefault(isValue);
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            return null;
        }
        /// <summary>
        /// 获取一个匹配数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <param name="isKey">是否存在关键字</param>
        /// <returns>匹配数据,失败返回null</returns>
        public valueType TryFirstOrDefaultLock(keyType key, Func<valueType, bool> isValue, out bool isKey)
        {
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                list<valueType> values = queueCache.Get(key, null);
                if (values != null)
                {
                    isKey = true;
                    return values.FirstOrDefault(isValue);
                }
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            isKey = false;
            return null;
        }
        /// <summary>
        /// 获取缓存数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>数据集合</returns>
        public valueType[] GetArray(keyType key)
        {
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                list<valueType> values = queueCache.Get(key, null);
                if (values != null) return values.GetArray();
                values = getList(key);
                if (values != null) return values.GetArray();
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取缓存数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>数据集合</returns>
        public valueType[] GetFindArray(keyType key, Func<valueType, bool> isValue)
        {
            list<valueType> list;
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                list = queueCache.Get(key, null);
                if (list == null) list = getList(key);
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            return list.ToSubArray().GetFindArray(isValue);
        }
        /// <summary>
        /// 获取范围排序数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="comparer">排序比较器,禁止锁操作</param>
        /// <param name="skipCount">跳过记录数</param>
        /// <param name="getCount">获取记录数</param>
        /// <returns>范围排序数据集合</returns>
        public valueType[] GetRangeSortLock(keyType key, Func<valueType, valueType, int> comparer, int skipCount, int getCount)
        {
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                list<valueType> values = queueCache.Get(key, null);
                if (values != null) return values.rangeSort(comparer, skipCount, getCount).GetArray();
                values = getList(key);
                if (values != null) return values.rangeSort(comparer, skipCount, getCount).GetArray();
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            return nullValue<valueType>.Array;
        }
        /// <summary>
        /// 获取缓存数据集合
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="getValue">数据获取器,禁止锁操作</param>
        /// <returns>数据集合</returns>
        public arrayType[] GetArrayLock<arrayType>(keyType key, Func<valueType, arrayType> getValue)
        {
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                list<valueType> values = queueCache.Get(key, null);
                if (values != null) return values.GetArray(getValue);
                values = getList(key);
                if (values != null) return values.GetArray(getValue);
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            return nullValue<arrayType>.Array;
        }
        /// <summary>
        /// 获取缓存数据数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>缓存数据数量</returns>
        public int GetCount(keyType key)
        {
            list<valueType> values = queueCache.Get(key, null);
            if (values != null) return values.Count;
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                values = getList(key);
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
            return values.count();
        }

        /// <summary>
        /// 获取缓存数据数量
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="isValue">数据匹配器,禁止锁操作</param>
        /// <returns>缓存数据数量</returns>
        public int GetCountLock(keyType key, Func<valueType, bool> isValue)
        {
            Monitor.Enter(counter.SqlTool.Lock);
            try
            {
                return (queueCache.Get(key, null) ?? getList(key)).GetCount(isValue);
            }
            finally { Monitor.Exit(counter.SqlTool.Lock); }
        }
    }
}
