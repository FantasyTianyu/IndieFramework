using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IndieFramework {
    public class ExcelData {
        //public int Id;
        public virtual void ReadData(byte[] datas, ref int index) {

        }
        public static T[] GetDatas<T>(byte[] datas) where T : ExcelData, new() {
            int index = 0;
            int count = ExcelData.ReadInt(datas, ref index);
            T[] results = new T[count];
            for (int i = 0; i < count; i++) {
                results[i] = new T();
                results[i].ReadData(datas, ref index);
            }
            return results;
        }

        //public static Dictionary<int, T> GetDataDictionary<T>(byte[] datas) where T : ExcelData, new() {
        //    Dictionary<int, T> dict = new Dictionary<int, T>();
        //    int index = 0;
        //    int count = ExcelData.ReadInt(datas, ref index);
        //    T[] results = new T[count];
        //    for (int i = 0; i < count; i++) {
        //        results[i] = new T();
        //        results[i].ReadData(datas, ref index);
        //        dict.Add(results[i].Id, (T)results[i]);
        //    }
        //    return dict;
        //}

        public static int ReadInt(byte[] data, ref int index) {
            byte[] read = new byte[sizeof(int)];
            Array.Copy(data, index, read, 0, read.Length);
            index += read.Length;
            return BitConverter.ToInt32(read);
        }

        public static float ReadFloat(byte[] data, ref int index) {
            byte[] read = new byte[sizeof(float)];
            Array.Copy(data, index, read, 0, read.Length);
            index += read.Length;
            return BitConverter.ToSingle(read);
        }

        public static string ReadString(byte[] data, ref int index) {
            int length = ReadInt(data, ref index);
            if (length <= 0) {
                return "";
            }
            byte[] read = new byte[length];
            Array.Copy(data, index, read, 0, length);
            index += length;
            return Encoding.UTF8.GetString(read);
        }

        public static string[] ReadStringArray(byte[] data, ref int index) {
            int count = ReadInt(data, ref index);
            string[] array = new string[count];
            for (int i = 0; i < count; i++) {
                array[i] = ReadString(data, ref index);
            }
            return array;
        }

        public static byte[] WriteInt(int value) {
            byte[] data = BitConverter.GetBytes(value);
            return data;
        }

        public static byte[] WriteFloat(float value) {
            byte[] data = BitConverter.GetBytes(value);
            return data;
        }

        public static byte[] WriteString(string value) {
            byte[] data = Encoding.UTF8.GetBytes(value);
            int length = data.Length;
            List<byte> byteList = new List<byte>();
            byteList.AddRange(WriteInt(length));
            byteList.AddRange(data);
            return byteList.ToArray();
        }

        public static byte[] WriteStringArray(string[] values) {
            List<byte> byteList = new List<byte>();
            int count = values.Length;
            byteList.AddRange(WriteInt(count));
            for (int i = 0; i < count; i++) {
                byteList.AddRange(WriteString(values[i]));
            }
            return byteList.ToArray();
        }
    }
}