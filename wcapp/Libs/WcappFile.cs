using WCAPP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using WCAPP.Models.Database;

namespace WCAPP.Libs
{
    public class WcappFile
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

        public WcappFile(Stream inputStream)
        {
            try
            {
                var code = new StreamReader(inputStream).ReadToEnd();
                var content = code.UnAesStr(Password);
                var md5 = content.Substring(0, 32);
                json = content.Substring(32);

                if (md5 != json.Md532())
                    throw new Exception("This WCAPP File is not Correct");
            }
            catch (Exception)
            {
                throw new Exception("This WCAPP File is not Correct");
            }
        }

        public WcappFile(Process[] wpses)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.RecursionLimit = 1;
            json = serializer.Serialize(wpses);
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

        public Process[] ToProcesses()
        {
            return new JavaScriptSerializer().Deserialize<Process[]>(json);
        }

        public static bool IsFileCorrect(Stream inputStream)
        {
            try
            {
                var code = new StreamReader(inputStream).ReadToEnd();
                var content = code.UnAesStr(Password);
                var json = content.Substring(32);
                var md5 = content.Substring(0, 32);

                return md5 == json.Md532();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}