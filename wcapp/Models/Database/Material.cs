using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WCAPP.Models.Database
{
    public class Material
    {
        [Key]
        public string Grade { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var m = obj as Material;

            return Grade == m.Grade;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Material m1, Material m2)
        {
            if (!Equals(m1, null))
                return m1.Equals(m2);

            return Equals(m2, null);
        }

        public static bool operator !=(Material m1, Material m2)
        {
            return !(m1 == m2);
        }
    }
}