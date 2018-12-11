using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using WCAPP.Models.Database;

namespace WCAPP
{
    public class GlobalData
    {
        private static readonly Config Config;
        public const string Token = "YaoshiSoft^1896^XJTU";

        public static bool IsDeviceNet => Config.DeviceNet;

        public static bool AutomaticMigrationsEnabled { get => Config.AutomaticMigrationsEnabled; }
        public static bool AutomaticMigrationDataLossAllowed { get => Config.AutomaticMigrationDataLossAllowed; }

        public static bool Excel32 => Config.Excel32;

        public static Dictionary<WeldMethod, List<WeldParam>> MethodParamMap =
            new Dictionary<WeldMethod, List<WeldParam>>();

        public static Dictionary<Authority, List<Authority>> MethodParamMaps = new Dictionary<Authority, List<Authority>>();

        static GlobalData()
        {
            var serializer = new JavaScriptSerializer();
            using (var fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "config.json", FileMode.Open))
            {
                using (var sr = new StreamReader(fs))
                {
                    Config = serializer.Deserialize<Config>(sr.ReadToEnd());
                }
            }

            MethodParamMap.Add(WeldMethod.氩弧焊, new List<WeldParam>
            {
                WeldParam.焊接电流,
                WeldParam.焊接速度,
                WeldParam.氩气流量正面,
                WeldParam.氩气流量反面,
                WeldParam.电流衰减,
                WeldParam.保护气滞后,
                WeldParam.填充材料牌号,
                WeldParam.填充材料规格,
                WeldParam.送丝速度,
                WeldParam.钨极直径,
                WeldParam.喷嘴直径
            });

            MethodParamMap.Add(WeldMethod.电阻焊, new List<WeldParam>
            {
                WeldParam.焊接速度,
                //WeldParam.焊机型别,
                //WeldParam.电极直径滚轮宽度,
                WeldParam.功率级数,
                WeldParam.预压,
                WeldParam.抬起,
                WeldParam.压下,
                WeldParam.脉冲1,
                WeldParam.焊接电流1,
                WeldParam.冷却,
                WeldParam.脉冲2,
                WeldParam.焊接电流2,
                WeldParam.维持,
                WeldParam.休止,
                WeldParam.熔核直径,
                WeldParam.下气室气压,
                WeldParam.中气室气压
            });

            MethodParamMap.Add(WeldMethod.电子束焊, new List<WeldParam>
            {
                WeldParam.功率,
                WeldParam.加速电压,
                WeldParam.电子束流,
                WeldParam.焊接速度,
                WeldParam.聚焦电流,
                //WeldParam.聚焦电流fd,
                WeldParam.工作距离,
                WeldParam.上升,
                WeldParam.下降,
                WeldParam.焊接真空度,
                WeldParam.波形,
                WeldParam.幅值,
                WeldParam.频率
            });

            MethodParamMap.Add(WeldMethod.高频钎焊, new List<WeldParam>
            {
                //WeldParam.填充材料牌号,
                //WeldParam.填充材料规格,
                WeldParam.钎料牌号,
                WeldParam.钎料规格,
                WeldParam.焊接电压,
                WeldParam.氩气流量,
                WeldParam.感应圈编号,
                WeldParam.管子规格
            });
        }

        public static List<WeldParam> GetMethodParams(WeldMethod method)
        {
            List<WeldParam> params_;
            MethodParamMap.TryGetValue(method, out params_);

            return params_;
        }
        public static List<Authority> GetMethodParam(Authority method)
        {
            List<Authority> params_;
            MethodParamMaps.TryGetValue(method, out params_);

            return params_;
        }

        public static List<SeamParam1> InitialMethodParams(WeldMethod method)
        {
            return GetMethodParams(method).Select(param => new SeamParam1 { Enum = param, Value = "/" }).ToList();
        }
        
    }

    class Config
    {
        public bool AutomaticMigrationsEnabled { get; set; }
        public bool AutomaticMigrationDataLossAllowed { get; set; }
        public bool DeviceNet { get; set; }
        public bool Excel32 { get; set; }
    }
}