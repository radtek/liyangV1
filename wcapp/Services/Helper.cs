using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCAPP.Sevices
{
    public class Helper
    {
        public string[] GetAllMaterials()
        {
            return new Context().Materials.Select(x=>x.Grade).ToArray();
        }
    }
}