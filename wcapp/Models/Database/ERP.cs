using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WCAPP.Models.Database
{
    public class ERP
    {
        [Key]
        public string ID { get; set; }//设备编号

        public string Desce { get; set; }//设备描述
        public string Type { get; set; }//设备型号
        public string Kostl { get; set; }//成本中心（使用部门编码）
        public string KoDesc { get; set; }//成本中心描述（使用部门描述）
        public string TypeDesc { get; set; }//设备种类描述
        public string Eartx { get; set; }//设备大小细类描述
        public string State { get; set; }//设备状态（检修、报废等）
        public string Eqart { get; set; }//设备状态（检修、报废等）
        public bool? importState { get; set; }//导入导出状态
        public DateTime? importTime { get; set; }//导出时间
       
    }
}