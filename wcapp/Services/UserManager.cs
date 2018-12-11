using WCAPP.Models;
using WCAPP.Types;
using WCAPP.Utils;
using System;
using System.Linq;
using System.Data.Entity;
using WCAPP.Models.Database;

namespace WCAPP.Sevices
{
    public class UserManager
    {
        public User[] GetUsers()
        {
            return new Context().Users.Include(x => x.AuthorityRs).ToArray();
        }

        public void CreateUser(User user)
        {
            var db = new Context();
            var u = db.Users.Find(user.Id);
            if (u != null)
                throw new Exception("该用户已存在");

            if (string.IsNullOrEmpty(user.Password))
                user.Password = "111111".Md532();
            db.Users.Add(user);
            db.SaveChanges();
        }

        public void DeleteUser(string userId)
        {
            var db = new Context();
            var user = db.Users.Find(userId);
            if (user == null)
                throw new Exception("该用户不存在");

            db.Users.Remove(user);
            db.SaveChanges();
        }

        public string PrepareResetPassword(string userId)
        {
            var db = new Context();
            var user = db.Users.Find(userId);
            if (user == null)
                throw new Exception("该用户不存在");

            user.Signature = new Random().Next(100000, 999999).ToString();
            db.SaveChanges();
            return user.Signature;
        }

        public void InsureResetPassword(string userId, string signature)
        {
            var db = new Context();
            var user = db.Users.Find(userId);
            if (user == null)
                throw new Exception("该用户不存在");

            bool correct = signature == user.Signature;

            if (correct)
                user.Password = "111111".Md532();

            user.Signature = null;
            db.SaveChanges();

            if (!correct)
                throw new Exception("验证码错误");
        }

        public void ModifyPassword(string userId, string oldPassword, string newPassword)
        {
            var db = new Context();
            var user = db.Users.Find(userId);
            if (user == null)
                throw new Exception("该用户不存在");

            if (user.Password != oldPassword)
                throw new Exception("旧密码不正确");

            user.Password = newPassword;
            db.SaveChanges();
        }

        public void ModifyAuthority(string userId, Authority[] authorities)
        {
            var db = new Context();
            var user = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == userId);
            if (user == null)
                throw new Exception("该用户不存在");

            if (user.AuthorityRs != null)
                db.Authorities.RemoveRange(user.AuthorityRs);
            user.AuthorityRs = authorities.ToAuthorityRs();
            db.SaveChanges();
        }

        public void ModifyUserInfo(string userId, User u)
        {
            var db = new Context();
            var user = db.Users.Find(userId);
            if (user == null)
                throw new Exception("该用户不存在");

            user.Name = u.Name;
            user.Position = u.Position;
            db.SaveChanges();
        }

        /// <summary>
        /// 获取具有审核权限的用户，该API无需管理员权限即可调用
        /// </summary>
        public User[] GetApprovers()
        {
            var db = new Context();

            var usrs = from u in db.Users
                       join a in db.Authorities
                       on u.Id equals a.UserId
                       where a.Authority == Authority.审核
                       select u;

            return usrs.ToArray();
        }
        
        public User[] PdmGetApprovers(string pdmToken)
        {
            if (pdmToken != GlobalData.Token)
                throw new Exception("没有PDM权限");

            return GetApprovers();
        }
    }
}