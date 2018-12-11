using System;
using System.Web;
using System.Web.SessionState;
using WCAPP.Utils;
using System.Linq;
using System.Data.Entity;
using WCAPP.Models.Home;

namespace WCAPP
{
    public class WcappModule : IHttpModule, IRequiresSessionState
    {
        /// <summary>
        /// 您将需要在网站的 Web.config 文件中配置此模块
        /// 并向 IIS 注册它，然后才能使用它。有关详细信息，
        /// 请参阅以下链接: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        public void Dispose()
        {
            //此处放置清除代码。
        }

        public void Init(HttpApplication context)
        {
            // 下面是如何处理 LogRequest 事件并为其 
            // 提供自定义日志记录实现的示例
            context.LogRequest += (source, e) => { };
            context.BeginRequest += (sender, e) => { };
            context.AcquireRequestState += (sender, e) => {
                var application = (HttpApplication)sender;
                var ctx = application.Context;
                Log.Info(ctx.Request.HttpMethod + " " + ctx.Request.Url.ToString());

                try
                {
                    var path = ctx.Request.Path.ToLower();
                    if (path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".ico") || path.EndsWith(".task") || path.EndsWith(".sync") ||
                        path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".ttf") || path.EndsWith(".wcapp"))
                        return;
                    if (path == "/" || path == "/home/index" || path.Contains(".asmx"))
                        return;

                    if (ctx.Session.GetSessionUser() != null)
                        return;

                    if (!path.Contains("pdm"))
                    {
                        ctx.Response.Redirect("~/");
                        return;
                    }

                    var usrid = ctx.Request.QueryString["authorId"];
                    if (usrid.IsEmpty())
                    {
                        ctx.Response.Redirect("~/");
                        return;
                    }

                    var db = new Context();
                    var usr = db.Users.Include(x => x.AuthorityRs).SingleOrDefault(x => x.Id == usrid);
                    if (usr == null)
                    {
                        ctx.Response.Redirect("~/");
                        return;
                    }

                    ctx.Session.SetSessionUser(new SessionUser {
                        Id = usr.Id,
                        Name = usr.Name,
                        Authorities = usr.Authorities
                    });
                }
                catch (Exception)
                {
                    ctx.Response.Write("系统出现错误，请稍后访问......");
                }
            };
            context.PostResolveRequestCache += (sender, e) => {
                var ctx = new HttpContextWrapper(((HttpApplication)sender).Context);

                IHttpHandler handler = null;
                var path = context.Request.Path.ToLower();

                if (handler != null)
                    ctx.RemapHandler(handler);
            };

            context.ReleaseRequestState += (sender, e) => {
            };
        }
    }
}