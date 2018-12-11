using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml;
using WCAPP.Libs;
using WCAPP.Models.Database;
using WCAPP.Models.ProcessModels;
using WCAPP.Utils;

namespace WCAPP
{
    /// <summary>
    /// WebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://10.15.7.223/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        /// <summary>
        /// 接受xml中的CData数据
        /// </summary>
        /// <param name="XMLParam">xml</param>
        /// <returns></returns>
        private Hashtable GetParametersFromXML(string XMLParam,string task)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(XMLParam);
            XmlNode dataNode = xml.SelectSingleNode("ReqMsg/document");

            var d = dataNode.InnerText.ToJson();
            string DataNode = dataNode.InnerText;

            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append(DataNode);
            XmlDocument xmls = new XmlDocument();
            xmls.LoadXml(sb.ToString());
            XmlNode DataNodes = xmls.SelectSingleNode(task);

            Hashtable Parameters = new Hashtable();
            foreach (XmlNode node in DataNodes.ChildNodes)
            {
                Parameters.Add(node.Name, node.InnerText);
            }
            return Parameters;
            
        }

        /// <summary>
        /// 检查是否缺少参数必填参数
        /// </summary>
        /// <param name="Parastr">参数名称，多个参数用逗号隔开</param>
        /// <param name="Paras">获取到参数Hastable</param>
        /// <returns></returns>
        public string CheckParas(string Parastr, Hashtable Paras)
        {
            string[] listParas = Parastr.Split(',');
            string Error = "";
            for (int i = 0; i < listParas.Length; i++)
            {
                if (listParas[i] != "")
                {
                    if (!Paras.ContainsKey(listParas[i]))
                    {
                        Error += listParas[i] + ",";
                    }
                }
            }

            if (Error != "")
            {
                return "缺少参数：" + Error.TrimEnd(',');
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 返回xml信息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetResult(bool status, string description, string code)
        {       
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<RespMsg>");
            sb.Append("<header>");
            sb.Append("<sysName>APP_MES</sysName>");
            sb.Append("<isSuccess>" + status.ToString() + "</isSuccess>");
            sb.Append("<message>" + description + "</message>");
            sb.Append("</header>");
            sb.Append("<document>");
            sb.Append("<![CDATA[<HTINFO>");
            sb.Append("<result>"+status.ToString()+ "</result>");
            sb.Append("<code>" + code + "</code>");
            sb.Append("<msg>" + description + "</msg>");
            sb.Append("</HTINFO>]]>");
            sb.Append("</document>");
            sb.Append("</RespMsg>");

            return sb.ToString();
        }
       
        /// <summary>
        /// 添加任务单
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod]
        public string AddTask(string xml)
        {
            try
            {
                // TODO 此处解析XML数据并处理
                string AddTask = "AddTask";
                Hashtable paras = GetParametersFromXML(xml,AddTask);
                //检查参数是否存在
                string msg = this.CheckParas("TaskNo,PartNo,BatchNo,SeqNo,Count,FacCode,WeldNo,WelderNo,StartTime,EndTime",paras);

                int code = 00000;//00000代表数据出错

                if (msg != "")
                {
                    return GetResult(false, msg,code.ToString());
                }

                string TaskNo = paras["TaskNo"].ToString();
                string PartNo = paras["PartNo"].ToString();
                string BatchNo = paras["BatchNo"].ToString();
                string SeqNo = paras["SeqNo"].ToString();
                string Count = paras["Count"].ToString();
                string FacCode = paras["FacCode"].ToString();
                string WeldNo = paras["WeldNo"].ToString();
                string WelderNo = paras["WelderNo"].ToString();
            
                string startTime = paras["StartTime"].ToString();
                string endTime = paras["EndTime"].ToString();
           
                //写入数据的日志
                var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "WcappFiles");
                if (!dirInfo.Exists)
                    dirInfo.Create();

                var name = AppDomain.CurrentDomain.BaseDirectory + "WcappFiles/" + "DeleteTask" + new Random().Next() + ".txt";

                using (var fs = new FileStream(name, FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write(xml);
                    }
                }
                DateTime StartTime = Convert.ToDateTime(startTime);
                DateTime EndTime= Convert.ToDateTime(endTime);
           
                //DateTime StartTime = DateTime.ParseExact(startTime, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            
                //DateTime EndTime= DateTime.ParseExact(endTime, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            
                //返回内容即文档中 输出参数描述 中 <![CDATA[ 以内的内容

                var db = new Context();
                //
                var dispatch = db.DispatchMessages.SingleOrDefault(x => x.TaskNo == TaskNo);
                var processes = from p in db.Processes
                                group p by p.No
                into g
                                select g.FirstOrDefault(x => x.Version == g.Max(y => y.Version));
                foreach (var process in processes.ToList())
                {
                    if (process.PartNo!=null)
                    {
                        foreach (var proceduce in process.Procedures)
                        {
                            if (PartNo == process.PartNo&&SeqNo==proceduce.No)
                            {
                                if (dispatch == null)
                                {
                                    dispatch = new DispatchMessage
                                    {
                                        TaskNo = TaskNo,
                                        PartNo = PartNo,
                                        BatchNo = BatchNo,
                                        SeqNo = SeqNo,
                                        Count = Count,
                                        FacCode = FacCode,
                                        WeldNo = WeldNo,
                                        WelderNo = WelderNo,
                                        StartTime = StartTime,
                                        EndTime = EndTime,
                                        State = false,
                                        exportTime = DateTime.Now,
                                        showState = false
                                    };
                                    db.DispatchMessages.Add(dispatch);
                                    db.SaveChanges();
                                    return GetResult(true, "添加派工信息成功", "00001");
                                }
                                else
                                {
                                    return GetResult(false, "任务单号已存在", "00000");
                                }
                            }
                        }                                               
                    }                                     
                }
                dispatch.showState = true;
                db.SaveChanges();
                return GetResult(true, "添加派工信息成功", "00001");

            }
            catch (Exception)
            {
                return GetResult(false, "新增派工信息失败！", "00000");
            }
        }

        /// <summary>
        /// 更新任务单
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod]
        public string UpdateTask(string xml)
        {
            try
            {
                //写入数据的日志
                var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "WcappFiles");
                if (!dirInfo.Exists)
                    dirInfo.Create();

                var name = AppDomain.CurrentDomain.BaseDirectory + "WcappFiles/" + "DeleteTask" + new Random().Next() + ".txt";

                using (var fs = new FileStream(name, FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write(xml);
                    }
                }
                string UpdateTask = "UpdateTask";
                // TODO 此处解析XML数据并处理
                Hashtable paras = GetParametersFromXML(xml, UpdateTask);

                //检查参数是否存在
                string msg = this.CheckParas("TaskNo,PartNo,BatchNo,SeqNo,Count,FacCode,WeldNo,WelderNo,StartTime,EndTime", paras);

                int code = 00000;//00000代表数据出错

                if (msg != "")
                {
                    return GetResult(false, msg, code.ToString());
                }

                string TaskNo = paras["TaskNo"].ToString();
                string PartNo = paras["PartNo"].ToString();
                string BatchNo = paras["BatchNo"].ToString();
                string SeqNo = paras["SeqNo"].ToString();
                string Count = paras["Count"].ToString();
                string FacCode = paras["FacCode"].ToString();
                string WeldNo = paras["WeldNo"].ToString();
                string WelderNo = paras["WelderNo"].ToString();

                string startTime = paras["StartTime"].ToString();
                string endTime = paras["EndTime"].ToString();
                
                DateTime StartTime = Convert.ToDateTime(startTime);
                DateTime EndTime = Convert.ToDateTime(endTime);                

                //返回内容即文档中 输出参数描述 中 <![CDATA[ 以内的内容

                var db = new Context();
                var dispatch = db.DispatchMessages.SingleOrDefault(x => x.TaskNo == TaskNo);

                if (dispatch == null)
                {
                    return GetResult(false, "任务单号不存在！", code.ToString());
                }

                dispatch.PartNo = PartNo;
                dispatch.BatchNo = BatchNo;
                dispatch.SeqNo = SeqNo;
                dispatch.Count = Count;
                dispatch.FacCode = FacCode;
                dispatch.WeldNo = WeldNo;
                dispatch.WelderNo = WelderNo;
                dispatch.StartTime = StartTime;
                dispatch.EndTime = EndTime;
                dispatch.State = false;
               
                db.SaveChanges();
                return GetResult(true, "更新派工信息成功！", "00001");

            }
            catch (Exception)
            {
                return GetResult(false, "更新派工信息失败！", "00000");
            }
        }

        [WebMethod]
        public string DeleteTask(string xml)
        {
            try
            {//写入数据的日志
                var dirInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "WcappFiles");
                if (!dirInfo.Exists)
                    dirInfo.Create();

                var name = AppDomain.CurrentDomain.BaseDirectory + "WcappFiles/" + "DeleteTask" +  new Random().Next() + ".txt";
                
                using (var fs = new FileStream(name, FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.Write(xml);
                    }
                }
                string DeleteTask = "DeleteTask";
                // TODO 此处解析XML数据并处理
                Hashtable paras = GetParametersFromXML(xml, DeleteTask);
                //检查参数是否存在
                string msg = this.CheckParas("TaskNo", paras);

                int code = 00000;//00000代表数据出错

                if (msg != "")
                {
                    return GetResult(false, msg, code.ToString());
                }

                string TaskNo = paras["TaskNo"].ToString();
                var db = new Context();
                var dispatch = db.DispatchMessages.SingleOrDefault(x => x.TaskNo == TaskNo);

                if (dispatch == null)
                {
                    return GetResult(false, "任务单号不存在！", code.ToString());
                }
                db.DispatchMessages.Remove(dispatch);
                db.SaveChanges();
                return GetResult(true, "删除派工信息成功", "00001");

            }
            catch (Exception)
            {
                return GetResult(false, "删除派工信息失败", "00000");
            }            
        }        
    }
}