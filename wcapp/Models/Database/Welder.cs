using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCAPP.Models.Database
{
    public class Welder
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public WelderQualification WelderQualification { get; set; }
    }
}