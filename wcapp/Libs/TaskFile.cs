using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using WCAPP.Models.Database;
using WCAPP.Utils;

namespace WCAPP.Libs
{
    public class TaskFile
    {
        const string s1 = "This";
        const string s2 = "is";
        const string s3 = "a";
        const string s4 = "very";
        const string s5 = "complex";
        const string s6 = "password";
        const string s7 = "you";
        const string s8 = "are";
        const string s9 = "a";
        const string s0 = "SB";

        static string Password = string.Format("^{0}~{1}!{2}@{3}#{4}${5}%{6}*{7}#{8}^{9}`", s1, s2, s4, s5, s7, s8, s9,
            s6, s3, s0);

        string json;

        public TaskFile(Stream inputStream)
        {
            try
            {
                var code = new StreamReader(inputStream).ReadToEnd();
                
                var content = code.UnAesStr(Password);
                var md5 = content.Substring(0, 32);
                json = content.Substring(32);

                if (md5 != json.Md532())
                    throw new Exception("所选的Task文件格式不正确！");
            }
            catch (Exception e)
            {
                throw new Exception("所选的Task文件格式不正确！" + e.Message);
            }
        }
        public TaskFile(DispatchMessage[] task)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.RecursionLimit = 1;
            json = serializer.Serialize(task);
        }
        public void SaveAs(string path)
        {
            var fs = new FileStream(path, FileMode.Create);
            var sw = new StreamWriter(fs);
            var md5 = json.Md532();
            var content = md5 + json;
            sw.Write(content.AesStr(Password));
            sw.Close();
            fs.Close();
        }
        public DispatchMessage[] ToTask()
        {
            return new JavaScriptSerializer().Deserialize<DispatchMessage[]>(json);
        }        
    }
}