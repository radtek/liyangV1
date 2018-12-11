using WCAPP.Types;
using WCAPP.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WCAPP.Models.Database
{
    /// <summary>
    /// 工序
    /// </summary>
    public class Procedure
    {
        public int Id { get; set; }
        public string PdmId { get; set; }

        [MaxLength(255)]
        [Index("procedure_index", IsUnique = true, Order = 1)]
        public string No { get; set; }

        public string Name { get; set; }
        public WeldMethod WeldMethod { get; set; }
        public AutoLevel? AutoLevel { get; set; }
        public WeldType WeldType { get; set; }
        public ResistType? ResistType { get; set; }
        public CurrentType CurrentType { get; set; }

        [Required][ScriptIgnore] public Process Process { get; set; }

        [Index("procedure_index", IsUnique = true, Order = 0)]
        public int ProcessId { get; set; }

        public List<Seam> Seams { get; set; }
        public ProgramTestState TestState { get; set; }
        public ApprovalState ApprovalState { get; set; }
    }
}