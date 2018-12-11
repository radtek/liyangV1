using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Microsoft.Ajax.Utilities;
using WCAPP.Models.Home;
using WCAPP.Types;

namespace WCAPP.Utils
{
    public static class Common
    {
        private static readonly string[] HexDict = new string[256];
        private static readonly JavaScriptSerializer Jss = new JavaScriptSerializer();

        static Common()
        {
            for (var i = 0; i < 256; i++)
            {
                var s = Convert.ToString(i, 16);
                if (s.Length < 2)
                    HexDict[i] = "0" + s;
                else
                    HexDict[i] = s;
            }
        }

        public static int FixPageNo(int pageNo, int pageSize, int totalSize)
        {
            var skip = (pageNo - 1) * pageSize;
            if (skip >= totalSize)
            {
                pageNo = (totalSize - 1) / pageSize + 1;
                if (pageNo < 1)
                    pageNo = 1;
            }

            return pageNo;
        }

        public static bool DoubleEquals(this double x1, double x2)
        {
            return Math.Abs(x1 - x2) < 0.01;
        }

        public static bool DoubleEquals(this double? x1, double? x2)
        {
            if (x1 == x2)
                return true;
            if (!x1.HasValue)
                return false;
            return Math.Abs(x1.Value - x2.Value) < 0.01;
        }

        public static bool DoubleEquals(this double x1, double? x2)
        {
            if (!x2.HasValue)
                return false;
            return Math.Abs(x1 - x2.Value) < 0.01;
        }

        public static bool DoubleEquals(this double? x1, double x2)
        {
            if (!x1.HasValue)
                return false;
            return Math.Abs(x1.Value - x2) < 0.01;
        }

        public static string ToColonString(this string[] strs)
        {
            var str = strs.Aggregate("", (current, s) => current + s + ":");
            return str.TrimEnd(':');
        }

        public static string LowerFirstChar(this string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public static string[] SplitByColon(this string str)
        {
            return str.Split(':');
        }

        public static byte[] ToUtf8Bytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ToUtf8String(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ToJson(this object o)
        {
            return Jss.Serialize(o);
        }

        public static object ToOjectAsJson(this string json)
        {
            return json == null ? null : Jss.DeserializeObject(json);
        }

        public static object ToOjectAsJson(this string json, Type type)
        {
            return json == null ? null : Jss.Deserialize(json, type);
        }

        public static T ToOjectAsJson<T>(this string json) where T : class
        {
            return json == null ? null : (T)Jss.Deserialize(json, typeof(T));
        }

        public static void WriteJson(this HttpResponse rsp, object o)
        {
            rsp.ContentType = "application/json";
            rsp.Write(o.ToJson());
        }

        public static string Joint(this string[] ss, string seperator)
        {
            if (ss == null || ss.Length < 1)
                return "";

            var sb = new StringBuilder();
            for (var i = 0; i < ss.Length;)
            {
                sb.Append(ss[i]);
                if (++i < ss.Length)
                    sb.Append(seperator);
            }

            return sb.ToString();
        }

        public static string Joint(this List<string> ss, string seperator)
        {
            return Joint(ss.ToArray(), seperator);
        }

        public static string Joint<T>(this T[] os, string seperator, Func<T, string> func)
        {
            if (os == null || os.Length < 1)
                return "";

            var sb = new StringBuilder();
            for (var i = 0; i < os.Length;)
            {
                sb.Append(func(os[i]));
                if (++i < os.Length)
                    sb.Append(seperator);
            }

            return sb.ToString();
        }

        public static string Joint<T>(this List<T> os, string seperator, Func<T, string> func)
        {
            return Joint(os.ToArray(), seperator, func);
        }

        public static bool Exists<T>(this T[] array, Predicate<T> match)
        {
            return Array.Exists(array, match);
        }

        public static SessionUser GetSessionUser(this HttpSessionStateBase seesion)
        {
            return (SessionUser)seesion["USER"];
        }

        public static void SetSessionUser(this HttpSessionStateBase seesion, SessionUser user)
        {
            seesion["USER"] = user;
        }

        public static SessionUser GetSessionUser(this HttpSessionState seesion)
        {
            return (SessionUser)seesion["USER"];
        }

        public static void SetSessionUser(this HttpSessionState seesion, SessionUser user)
        {
            seesion["USER"] = user;
        }

        public static T ToEnum<T>(this string s) where T : struct
        {
            return (T)Enum.Parse(typeof(T), s);
        }

        public static string GetParamValue(this Dictionary<WeldParam, string> dict, WeldParam p)
        {
            if (dict.TryGetValue(p, out var v))
                return string.IsNullOrEmpty(v) ? "/" : v;
            return "/";
        }

        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static string GetValue<T>(this Dictionary<T, string> dict, T key)
        {
            return dict.TryGetValue(key, out string v) ? v : "";
        }

        public static string GetDbString(this object cell)
        {
            return cell == null || cell is DBNull ? "" : cell.ToString().Trim();
        }

        public static bool GetDbDouble(this object cell, out double ret)
        {
            var s = GetDbString(cell);
            return double.TryParse(s, out ret);
        }

        public static T GetDbEnum<T>(this object cell) where T : struct
        {
            T ret;
            var s = GetDbString(cell);
            if (Enum.TryParse(s, true, out ret))
                return ret;
            throw new Exception("\"" + s + "\"为非法枚举值, 该列枚举值应该为: " + Enum.GetNames(typeof(T)).Joint(", "));
        }
    }
}