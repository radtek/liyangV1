using WCAPP.Models;
using WCAPP.Types;
using WCAPP.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Script.Serialization;
using WCAPP.Models.Database;

namespace WCAPP.Sevices
{
    public class HomeManager
    {
        Sessions ss = new Sessions();

        public List<Entrance> Login(string userId, string password, bool isAdmin)
        {
            var db = new Context();
            List<Entrance> ret = new List<Entrance>();
            User user = db.Users.Include(x => x.AuthorityRs)
                .SingleOrDefault(u => u.Id == userId && u.Password == password);
            if (user == null)
                throw new Exception("用户名或密码错误");
            user.Password = null;

            var authes = user.Authorities;

            if (authes == null)
                throw new Exception("用户权限不够");

            if (isAdmin)
            {
                if (authes.Exists(a => a == Authority.用户管理))
                    ret.Add(Entrance.用户管理);
            }
            else
            {
                if (authes.Exists(a => a == Authority.编制))
                {
                    ret.Add(Entrance.工艺规程管理);
                    ret.Add(Entrance.试焊管理);
                }

                if (authes.Exists(a => a == Authority.审核))
                {
                    ret.Add(Entrance.审核管理);
                }
            }

            if (ret.Count == 0)
                throw new Exception("用户权限不够");

            HttpContext.Current.Response.SetCookie(new HttpCookie("session", ss.CreateSession(user)));
            return ret;
        }

        public void Logout()
        {
            var s = HttpContext.Current.Request.Cookies.Get("session");
            var token = s != null ? s.Value : null;
            if (token != null)
                ss.DeleteSession(token);
        }


        public bool IsLogin()
        {
            var s = HttpContext.Current.Request.Cookies.Get("session");
            var token = s != null ? s.Value : null;
            return ss.CheckSession(token).Succeed;
        }

        /// <summary>
        /// 空操作，用于维持会话
        /// </summary>
        public void Nop()
        {
        }

        public User GetCurrentUser(string userId)
        {
            var db = new Context();
            var user = db.Users.Find(userId);
            return user;
        }
    }
}