﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;

namespace fastCSharp
{
    /// <summary>
    /// 内存或字节数组
    /// </summary>
    public static class memory
    {
        /// <summary>
        /// 填充字节
        /// </summary>
        /// <param name="src">串起始地址</param>
        /// <param name="value">字节值</param>
        /// <param name="length">字节数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void Fill(void* src, byte value, int length)
        {
            if (src != null && length > 0) unsafer.memory.Fill(src, value, length);
        }
        /// <summary>
        /// 复制字符数组
        /// </summary>
        /// <param name="source">原串起始地址</param>
        /// <param name="destination">目标串起始地址</param>
        /// <param name="count">字符数量</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void Copy(char* source, char* destination, int count)
        {
            if (source != null && destination != null && count > 0) unsafer.memory.Copy((void*)source, (void*)destination, count << 1);
        }
        /// <summary>
        /// 复制字节数组
        /// </summary>
        /// <param name="source">原串起始地址</param>
        /// <param name="destination">目标串起始地址</param>
        /// <param name="length">字节长度</param>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static void Copy(void* source, void* destination, int length)
        {
            if (source != null && destination != null && length > 0) unsafer.memory.Copy(source, destination, length);
        }
        /// <summary>
        /// 字节数组比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>是否相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool equal(this byte[] left, byte[] right)
        {
            if (left == null) return right == null;
            return right != null && (left.Equals(right) || unsafer.memory.Equal(left, right));
        }
        /// <summary>
        /// 字节数组比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>是否相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static bool equal(this subArray<byte> left, subArray<byte> right)
        {
            return equal(left, ref right);
        }
        /// <summary>
        /// 字节数组比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>是否相等</returns>
        public unsafe static bool equal(this subArray<byte> left, ref subArray<byte> right)
        {
            if (left.array == null) return right.array == null;
            if (left.length == right.length)
            {
                fixed (byte* leftFixed = left.array, rightFixed = right.array)
                {
                    return unsafer.memory.Equal(leftFixed + left.startIndex, rightFixed + right.startIndex, left.length);
                }
            }
            return false;
        }
        /// <summary>
        /// 字节数组比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>是否相等</returns>
        public unsafe static bool simpleEqual(this subArray<byte> left, ref subArray<byte> right)
        {
            if (left.array == null) return right.array == null;
            if (left.length == right.length)
            {
                fixed (byte* leftFixed = left.array, rightFixed = right.array)
                {
                    return unsafer.memory.SimpleEqual(leftFixed + left.startIndex, rightFixed + right.startIndex, left.length);
                }
            }
            return false;
        }
        /// <summary>
        /// 字节数组比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="count">比较字节数</param>
        /// <returns>是否相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static bool equal(this byte[] left, byte[] right, int count)
        {
            if (left == null) return right == null;
            return right != null && left.Length >= count && right.Length >= count && count >= 0 && unsafer.memory.Equal(left, right, count);
        }
        /// <summary>
        /// 字节数组比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="count">比较字节数</param>
        /// <returns>是否相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe bool equal(this byte[] left, void* right, int count)
        {
            if (left == null) return right == null;
            return right != null && left.Length >= count && count >= 0 && unsafer.memory.Equal(left, right, count);
        }
        /// <summary>
        /// 字节数组比较
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="count">比较字节数</param>
        /// <returns>是否相等</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static unsafe bool Equal(void* left, void* right, int count)
        {
            if (left != null && right != null)
            {
                return count > 0 ? unsafer.memory.Equal(left, right, count) : count == 0;
            }
            return left == null && right == null;
        }
        /// <summary>
        /// 查找字节
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <param name="value">字节值</param>
        /// <returns>字节位置,失败为-1</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static int indexOf(this byte[] data, byte value)
        {
            return data != null && data.Length > 0 ? unsafer.memory.IndexOf(data, value) : -1;
        }
        /// <summary>
        /// 查找字节
        /// </summary>
        /// <param name="start">起始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="value">字节值</param>
        /// <returns>字节位置</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static byte* Find(void* start, void* end, byte value)
        {
            return start != null && end > start ? unsafer.memory.Find((byte*)start, (byte*)end, value) : null;
        }
        /// <summary>
        /// 字节替换
        /// </summary>
        /// <param name="value">字节数组</param>
        /// <param name="oldData">原字节</param>
        /// <param name="newData">目标字节</param>
        /// <returns>字节数组</returns>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public static void replace(this byte[] value, byte oldData, byte newData)
        {
            if (value != null && value.Length != 0) unsafer.memory.Replace(value, oldData, newData);
        }
        /// <summary>
        /// 大写转小写
        /// </summary>
        [MethodImpl((MethodImplOptions)fastCSharp.pub.MethodImplOptionsAggressiveInlining)]
        public unsafe static byte[] toLower(this byte[] value)
        {
            if (value != null)
            {
                fixed (byte* valueFixed = value) unsafer.memory.ToLower(valueFixed, valueFixed + value.Length);
            }
            return value;
        }
        /// <summary>
        /// 大写转小写
        /// </summary>
        public unsafe static byte[] getToLower(this byte[] value)
        {
            if (value.length() != 0)
            {
                byte[] newValue = new byte[value.Length];
                fixed (byte* valueFixed = value, newValueFixed = newValue) unsafer.memory.ToLower(valueFixed, valueFixed + value.Length, newValueFixed);
                return newValue;
            }
            return value;
        }
        /// <summary>
        /// 转16进制字符串(小写字母)
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>16进制字符串</returns>
        public unsafe static string toLowerHex(this byte[] data)
        {
            if (data.length() != 0)
            {
                string hex = fastCSharp.String.FastAllocateString(data.Length << 1);
                fixed (byte* dataFixed = data)
                fixed (char* hexFixed = hex)
                {
                    char* write = hexFixed;
                    for (byte* start = dataFixed, end = dataFixed + data.Length; start != end; ++start)
                    {
                        int code = *start >> 4;
                        *write++ = (char)(code < 10 ? code + '0' : (code + ('0' + 'a' - '9' - 1)));
                        code = *start & 0xf;
                        *write++ = (char)(code < 10 ? code + '0' : (code + ('0' + 'a' - '9' - 1)));
                    }
                }
                return hex;
            }
            return data != null ? string.Empty : null;
        }
        /// <summary>
        /// 转16进制字符串(小写字母)
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>16进制字符串</returns>
        public unsafe static string toUpperHex(this byte[] data)
        {
            if (data == null) return null;
            if (data.Length == 0) return string.Empty;
            string hex = fastCSharp.String.FastAllocateString(data.Length << 1);
            fixed (byte* dataFixed = data)
            fixed (char* hexFixed = hex)
            {
                char* write = hexFixed;
                for (byte* start = dataFixed, end = dataFixed + data.Length; start != end; ++start)
                {
                    *write++ = (char)number.ToHex((uint)*start >> 4);
                    *write++ = (char)number.ToHex((uint)*start & 0xf);
                }
            }
            return hex;
        }
        ///// <summary>
        ///// 对象序列化
        ///// </summary>
        ///// <typeparam name="valueType">对象类型</typeparam>
        ///// <param name="value">对象</param>
        ///// <returns>字节数组</returns>
        //public static subArray<byte> serialize<valueType>(this valueType value)
        //{
        //    if (value != null)
        //    {
        //        using (MemoryStream memoryStream = new memoryStream.Get())
        //        {
        //            (new BinaryFormatter()).Serialize(memoryStream, value);
        //            return subArray<byte>.Unsafe(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
        //        }
        //    }
        //    return default(subArray<byte>);
        //}
        ///// <summary>
        ///// 对象反序列化
        ///// </summary>
        ///// <typeparam name="valueType">对象类型</typeparam>
        ///// <param name="value">字节数组</param>
        ///// <param name="startIndex">起始位置</param>
        ///// <param name="length">数据长度</param>
        ///// <returns>对象</returns>
        //public static valueType deSerialize<valueType>(this byte[] data, int startIndex, int length)
        //{
        //    valueType value = default(valueType);
        //    if (data != null && data.Length != 0 && startIndex >= 0 && startIndex + length <= data.Length)
        //    {
        //        using (MemoryStream memoryStream = new MemoryStream(data, startIndex, length))
        //        {
        //            value = (valueType)(new BinaryFormatter()).Deserialize(memoryStream);
        //        }
        //    }
        //    return value;
        //}
        ///// <summary>
        ///// 对象反序列化
        ///// </summary>
        ///// <typeparam name="valueType">对象类型</typeparam>
        ///// <param name="value">字节数组</param>
        ///// <param name="defaultValue">默认空值</param>
        ///// <returns>对象</returns>
        //public static valueType deSerialize<valueType>(this byte[] data, valueType defaultValue)
        //{
        //    valueType value = defaultValue;
        //    if (data != null && data.Length != 0)
        //    {
        //        using (MemoryStream memoryStream = new MemoryStream(data))
        //        {
        //            value = (valueType)(new BinaryFormatter()).Deserialize(memoryStream);
        //        }
        //    }
        //    return value;
        //}
        ///// <summary>
        ///// 对象反序列化
        ///// </summary>
        ///// <typeparam name="valueType">对象类型</typeparam>
        ///// <param name="value">字节数组</param>
        ///// <param name="startIndex">起始位置</param>
        ///// <param name="length">数据长度</param>
        ///// <param name="defaultValue">默认空值</param>
        ///// <returns>对象</returns>
        //public static valueType deSerialize<valueType>(this byte[] data, int startIndex, int length, valueType defaultValue)
        //{
        //    valueType value = defaultValue;
        //    if (data != null && data.Length != 0 && startIndex >= 0 && startIndex + length <= data.Length)
        //    {
        //        using (MemoryStream memoryStream = new MemoryStream(data, startIndex, length))
        //        {
        //            value = (valueType)(new BinaryFormatter()).Deserialize(memoryStream);
        //        }
        //    }
        //    return value;
        //}
    }
}
