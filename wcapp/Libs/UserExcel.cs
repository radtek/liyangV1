using WCAPP.Models.Database;
using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using WCAPP.Utils;

namespace WCAPP.Libs
{
    public static class UserExcel
    {
        private const int ParamOffset = 18;

        public static IEnumerable<User> Import(string path)
        {
            OdbcConnection conn = null;
            try
            {
                string strConn;
                if (GlobalData.Excel32)
                    strConn = "Driver={Microsoft Excel Driver (*.xls)};Dbq=" + path + ";";
                else
                    strConn = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};Dbq=" + path + ";";

                conn = new OdbcConnection(strConn);
                conn.Open();

                var userDict = new Dictionary<string, User>(); 
                
                var ds = new DataSet();
                var cmd = new OdbcDataAdapter("select * from [" + "员工信息表" + "$]", strConn);
                cmd.Fill(ds);              

                var line = 2;

                try
                {
                   foreach (DataRow dr in ds.Tables[0].Rows)
                   {
                       User user;                       
                       var userNo = dr["企业代码"].GetDbString();
                        if (userNo=="")
                        {
                            continue;
                        }
                        else
                        {
                            if (!userDict.TryGetValue(userNo, out user))
                            {
                                //Authority auths = dr["权限"].GetDbEnum<Authority>();
                                //foreach (Authority authw in Enum.GetValues(typeof(Authority)))
                                //{
                                //}
                               //string auth = dr["权限"].GetDbString();
                               //string[] authArray = auth.Split(',');
                                //AuthorityRs = model.Authorities.ToAuthorityRs()
                               user = new User
                                {
                                    Id = userNo,
                                    Department = dr["所在部门"].GetDbString(),
                                    Name = dr["姓名"].GetDbString(),
                                    Sex = dr["性别"].GetDbEnum<Sex>(),
                                    Position = dr["岗位"].GetDbEnum< WorkPosition>(),
                                    Password = "123456".Md532(),                                  
                                    AuthorityRs = new[] { Authority.编制 }.ToAuthorityRs()
                                };
                                userDict.Add(userNo, user);
                            }
                        }                                                                   
                   }
                }
                catch (Exception e)
                {
                    throw new Exception("第" + line + "行，" + e.Message);
                }                

                return userDict.Values;
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }       
    }
}