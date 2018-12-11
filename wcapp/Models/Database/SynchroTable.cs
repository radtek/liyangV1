using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WCAPP.Models.Database
{
    public class SynchroTable
    {
        [Key]
        public string taskNo { get; set; }//任务ID

        public string partNo { get; set; }//零件编号

        public string batchNo { get; set; }//批次号

        [MaxLength(4)]
        public string seqNo { get; set; }//工序号

        public int countNum { get; set; }//派工数量

        public string facCode { get; set; }//分厂代码

        public string weldNo { get; set; }//设备编号

        [MaxLength(8)]
        public string welderNo { get; set; }//员工编号

        public string status { get; set; }//状态

        public DateTime startTime { get; set; }//计划起始时间

        public DateTime endTime { get; set; }//计划终止时间

        public DateTime realStartTime { get; set; }//实际起始时间

        public DateTime realEndTime { get; set; }//实际终止时间

        public DateTime weldTime { get; set; }//实际焊接时间

        public DateTime? importTime { get; set; }//导出时间

        public bool importState { get; set; }//导入导出状态
    }
}