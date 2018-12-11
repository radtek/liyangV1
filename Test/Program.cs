using System;
using WCAPP.Libs;

namespace Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                WcappExcel.Import(@"C:\Users\yedan\Desktop\WCAPP测试数据.xls");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}