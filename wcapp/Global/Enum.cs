using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WCAPP
{
    public class Enum<T>
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public Enum(int val)
        {
            Index = val;
            Name = Enum.GetName(typeof(T), val);
        }

        public static List<Enum<T>> GetValues()
        {
            if (!typeof(T).IsEnum)
                throw new Exception("泛型参数类型不是枚举");

            var arr = Enum.GetValues(typeof(T));
            var opts = new List<Enum<T>>();

            for (var i = 0; i < arr.Length; i++)
            {
                opts.Add(new Enum<T>(i));
            }

            return opts;
        }

        public static SelectList GetSelectList()
        {
            return new SelectList(Enum.GetValues(typeof(T)));
        }
    }
}