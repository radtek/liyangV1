using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WCAPP.Models.DispatchMessage
{
    public class UpdateDispatchMessageModel
    {
        [Required]
        public string TaskNo { get; set; }//任务ID

        public string PartNo { get; set; }//零件编号

        public string BatchNo { get; set; }//批次号

        public string SeqNo { get; set; }//工序号

        public string Count { get; set; }//派工数量

        public string FacCode { get; set; }//分厂代码

        public string WeldNo { get; set; }//设备编号

        public string WelderNo { get; set; }//员工编号

        public DateTime StartTime { get; set; }//起始时间

        public DateTime EndTime { get; set; }//终止时间

        public bool State { get; set; }//导入导出状态

        public DateTime? importTime { get; set; }//导出时间
        public DateTime? exportTime { get; set; }//导入时间
    }
}