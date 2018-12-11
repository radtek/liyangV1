using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using WCAPP.Types;
using WCAPP.Utils;

namespace WCAPP.Models.Database
{
    /// <summary>
    /// 工艺规程
    /// </summary>
    public class Process
    {
        public int Id { get; set; }
        public string PdmId { get; set; }

        [MaxLength(255)]
        [Index("process_index", IsUnique = true, Order = 0)]
        public string No { get; set; }

        [Index("process_index", IsUnique = true, Order = 1)]
        public int Version { get; set; }

        public string PartNo { get; set; }
        public string PartName { get; set; }
        public bool Published { get; set; }

        public bool Establish { get; set; }

        [NotMapped]
        public string VersionString => "V" + Version + "00";

        public User Author { get; set; }

        public ApprovalState ApprovalState { get; set; }

        public ProgramTestState TestState { get; set; }

        public List<Procedure> Procedures { get; set; }

        public DateTime? importTime { get; set; }//导出时间
        public bool importState { get; set; }//导入导出状态

        public DateTime? excelImportTime { get; set; }//导入excel时间
    }
}