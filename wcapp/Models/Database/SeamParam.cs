using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WCAPP.Models.Database
{
    [Serializable]
    public class SeamParam
    {
        public int Id { get; set; }
        public WeldParam Enum { get; set; }

        public string Value { get; set; }

      

        [Required]
        [ScriptIgnore]
        public Seam Seam { get; set; }
    }
    
    [Serializable]
    public class SeamParam1 : SeamParam
    {
       
    }
    
    [Serializable]
    public class SeamParam2 : SeamParam
    {
    }
   
}