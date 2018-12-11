using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WCAPP.Models.Database
{
    public class ReportFile
    {
        [MaxLength(255)]
        [Key]
        public string No { get; set; }
        public byte[] Bytes { get; set; }
    }
}