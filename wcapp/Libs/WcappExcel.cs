using WCAPP.Models.Database;
using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using WCAPP.Utils;

namespace WCAPP.Libs
{
    public static class WcappExcel
    {
        private const int ParamOffset = 18;

        public static IEnumerable<Process> Import(string path)
        {
            OdbcConnection conn = null;
            var db = new Context();
            try
            {
                string strConn;
                if (GlobalData.Excel32)
                    strConn = "Driver={Microsoft Excel Driver (*.xls)};Dbq=" + path + ";";
                else
                    strConn = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};Dbq=" + path + ";";

                conn = new OdbcConnection(strConn);
                conn.Open();
                var processDict = new Dictionary<string, Process>();

                foreach (WeldMethod m in Enum.GetValues(typeof(WeldMethod)))
                {
                    var ds = new DataSet();
                    try
                    {
                        var cmd = new OdbcDataAdapter("select * from [" + m + "$]", strConn);
                        cmd.Fill(ds);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        var idx1 = col.ColumnName.IndexOf('(');
                        var idx2 = col.ColumnName.IndexOf('（');
                        var idx = idx1;

                        if (idx < 0)
                            idx = idx2;
                        else if (idx2 > 0 && idx2 < idx1)
                            idx = idx2;

                        if (idx > 0)
                            col.ColumnName = col.ColumnName.Substring(0, idx);
                    }

                    var line = 2;

                    try
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            Process process;
                            var processNo = dr["工艺规程编号"].GetDbString();
                            var userId = dr["编制者"].GetDbString();
                            var user = db.Users.SingleOrDefault(x => x.Id == userId||x.Name==userId);

                            //if (user==null)
                            //{
                            //    user=new User
                            //    {
                            //        Id = DateTime.Now.ToString(),
                            //        Name = "none person",
                            //        Position = WorkPosition.焊接工程师,
                            //        Password = "123456".Md532(),
                            //        Department = "燃烧部件制造分厂",
                            //        AuthorityRs = new[] { Authority.编制 }.ToAuthorityRs()
                            //    };
                            //}

                            if (!processDict.TryGetValue(processNo, out process))
                            {
                                process = new Process
                                {
                                    No = processNo,
                                    // PartNo = PartNo,
                                    //PartName=PartName,
                                    PartNo = dr["零件号"].GetDbString(),
                                    PartName = dr["零件名称"].GetDbString(),
                                    TestState = ProgramTestState.未完成,
                                    ApprovalState = ApprovalState.未提交审核,
                                    Version = 1,
                                    Author = user,
                                    Procedures = new List<Procedure>()
                                };
                                processDict.Add(processNo, process);
                            }

                            var procedureNo = dr["工序号"].GetDbString();
                            var procedureName = dr["工序名称"].GetDbString();
                            var procedure = process.Procedures.Find(x => x.No == procedureNo);
                           
                            if (processNo.Trim() == "" || procedureNo.Trim() == "")
                            {
                                continue;
                                //throw new Exception(m + "，第" + line + "行，" + "工艺规程编号为空!");
                            }
                            if (procedure == null)
                            {
                                procedure = new Procedure
                                {
                                    No = procedureNo,
                                    Name = procedureName,
                                    WeldMethod = m,
                                    WeldType = dr["焊接类型"].GetDbEnum<WeldType>(),
                                    Seams = new List<Seam>()
                                };
                                process.Procedures.Add(procedure);
                            }

                            if (m == WeldMethod.电阻焊)
                            {
                                string sa = dr["焊接方式"].GetDbString();
                                string s = dr["焊接方式"].GetDbEnum<ResistType>().ToString();

                                procedure.ResistType = dr["焊接方式"].GetDbEnum<ResistType>();
                            }
                            if (m == WeldMethod.氩弧焊)
                                procedure.AutoLevel = dr["自动化程度"].GetDbEnum<AutoLevel>();

                            var seam = new Seam
                            {
                                No = (procedure.Seams.Count + 1).ToString(),
                                TestState = TestState.尚未进行,
                                CheckStandard = dr["验收标准"].GetDbString(),
                                SeamLevel = dr["焊缝等级"].GetDbEnum<SeamLevel>(),
                                JointForm = dr["接头形式"].GetDbEnum<JointForm>(),
                                FillWire = dr["是否填丝"].GetDbString() == "是",
                                Material1 = dr["材料牌号1"].GetDbString(),
                                Material2 = dr["材料牌号2"].GetDbString()
                            };

                            double thick1, thick2;
                            if (!dr["材料规格1"].GetDbDouble(out thick1))
                                throw new Exception("\"材料规格1\"必须为数字");
                            if (!dr["材料规格2"].GetDbDouble(out thick2))
                                throw new Exception("\"材料规格2\"必须为数字");

                            seam.Thick1 = thick1;
                            seam.Thick2 = thick2;

                            if (m == WeldMethod.电阻焊 || m == WeldMethod.氩弧焊)
                            {
                                seam.Material3 = dr["材料牌号3"].GetDbString();
                                if (seam.Material3 != "")
                                {
                                    double thick3;
                                    if (!dr["材料规格3"].GetDbDouble(out thick3))
                                        throw new Exception("\"材料规格3\"必须为数字");
                                    seam.Thick3 = thick3;
                                }
                                seam.Material4 = dr["材料牌号4"].GetDbString();
                                if (seam.Material4 != "")
                                {
                                    double thick4;
                                    if (!dr["材料规格4"].GetDbDouble(out thick4))
                                        throw new Exception("\"材料规格4\"必须为数字");
                                    seam.Thick4 = thick4;
                                }
                                seam.ElectrodeDiameter = dr["电极直径/滚轮宽度"].GetDbString();
                                seam.WeldMachineClass = dr["焊机型别"].GetDbString();
                            }

                            seam.SpecialReportFileNo = dr["特殊过程确认报告编号"].GetDbString();

                            procedure.Seams.Add(seam);

                            switch (m)
                            {
                                case WeldMethod.氩弧焊:
                                    seam.CoverReportFileNo = dr["涵盖报告编号"].GetDbString();
                                    seam.InitialParams = FillTigParam(dr);
                                    break;
                                case WeldMethod.电阻焊:
                                    seam.InitialParams = FillResistParam(dr);
                                    break;
                                case WeldMethod.电子束焊:
                                    seam.InitialParams = FillBeamParam(dr);
                                    break;
                                case WeldMethod.高频钎焊:
                                    seam.InitialParams = FillBrazeParam(dr);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            line++;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.StackTrace);
                        throw new Exception(m + "，第" + line + "行，" + e.Message);
                    }
                }

                return processDict.Values;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private static List<SeamParam1> FillTigParam(DataRow dr)
        {
            return new List<SeamParam1>
            {
                new SeamParam1
                {
                    Enum = WeldParam.焊接电流,
                    Value = dr["焊接电流"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.焊接速度,
                    Value = dr["焊接速度"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.氩气流量正面,
                    Value = dr["氩气流量正面"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.氩气流量反面,
                    Value = dr["氩气流量反面"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.电流衰减,
                    Value = dr["电流衰减"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.保护气滞后,
                    Value = dr["保护气滞后"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.填充材料牌号,
                    Value = dr["填充材料牌号"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.填充材料规格,
                    Value = dr["填充材料规格"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.送丝速度,
                    Value = dr["送丝速度"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.钨极直径,
                    Value = dr["钨极直径"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.喷嘴直径,
                    Value = dr["喷嘴直径"].GetDbString()
                }
            };
        }

        private static List<SeamParam1> FillBeamParam(DataRow dr)
        {
            return new List<SeamParam1>
            {
                new SeamParam1
                {
                    Enum = WeldParam.功率,
                    Value = dr["功率"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.加速电压,
                    Value = dr["加速电压"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.电子束流,
                    Value = dr["电子束流"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.焊接速度,
                    Value = dr["焊接速度"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.聚焦电流,
                    Value = dr["聚焦电流"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.工作距离,
                    Value = dr["工作距离"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.上升,
                    Value = dr["束流上升斜率控制"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.下降,
                    Value = dr["束流下降斜率控制"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.焊接真空度,
                    Value = dr["焊接真空度"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.波形,
                    Value = dr["电子束扫描偏转波形"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.幅值,
                    Value = dr["电子束扫描偏转幅值"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.频率,
                    Value = dr["电子束扫描偏转频率"].GetDbString()
                }
            };
        }

        private static List<SeamParam1> FillBrazeParam(DataRow dr)
        {
            return new List<SeamParam1>
            {
                new SeamParam1
                {
                    Enum = WeldParam.钎料牌号,
                    Value = dr["钎料牌号"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.钎料规格,
                    Value = dr["填料规格"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.焊接电压,
                    Value = dr["焊接电压"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.氩气流量,
                    Value = dr["氩气流量"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.感应圈编号,
                    Value = dr["感应圈编号"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.管子规格,
                    Value = dr["管子规格"].GetDbString()
                }
            };
        }

        private static List<SeamParam1> FillResistParam(DataRow dr)
        {
            return new List<SeamParam1>
            {
                //new SeamParam1
                //{
                //    Enum = WeldParam.焊机型别,
                //    Value = dr["焊机型别"].GetDbString()
                //},

                //new SeamParam1
                //{
                //    Enum = WeldParam.电极直径滚轮宽度,
                //    Value = dr["电极直径/滚轮宽度"].GetDbString()
                //},

                new SeamParam1
                {
                    Enum = WeldParam.功率级数,
                    Value = dr["功率级数"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.预压,
                    Value = dr["预压"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.抬起,
                    Value = dr["电极压力抬起"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.压下,
                    Value = dr["电极压力压下"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.脉冲1,
                    Value = dr["脉冲1"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.焊接电流1,
                    Value = dr["焊接电流1"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.冷却,
                    Value = dr["冷却"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.脉冲2,
                    Value = dr["脉冲2"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.焊接电流2,
                    Value = dr["焊接电流2"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.维持,
                    Value = dr["维持"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.休止,
                    Value = dr["休止"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.下气室气压,
                    Value = dr["下气室气压"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.中气室气压,
                    Value = dr["中气室气压"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.焊接速度,
                    Value = dr["焊接速度"].GetDbString()
                },

                new SeamParam1
                {
                    Enum = WeldParam.熔核直径,
                    Value = dr["熔核直径"].GetDbString()
                }
            };
        }
    }
}