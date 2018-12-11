using WCAPP.Models.Database;
using WCAPP.Types;
using WCAPP.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;

namespace WCAPP
{
    public class Sessions
    {
        static Dictionary<string, Session> dictSeesion = new Dictionary<string, Session>();
        static TaskQueue taskQueue = new TaskQueue();

        const int MAX_TICK = 12;

        static int first = 1;

        public Sessions()
        {
            if (Interlocked.Exchange(ref first, 0) == 1)
            {
                try
                {
                    var users = new Context().Users
                        .Where(x => x.Id == "cj" || x.Id == "sh" || x.Id == "cjsh" || x.Id == "admin").ToList();
                    foreach (var user in users)
                    {
                        var token = user.Id;
                        var session = new Session(token, user);

                        taskQueue.PostTask(() => { dictSeesion.Add(token, session); });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public string CreateSession(User user, string t = null)
        {
            var token = t ?? Guid.NewGuid().ToString("N");
            var session = new Session(token, user);

            session.SetTimerCallback((object sender, ElapsedEventArgs e) =>
            {
                taskQueue.PostTask(() =>
                {
                    session.Tick++;
                    if (session.Tick > MAX_TICK)
                    {
                        dictSeesion.Remove(token);
                        session.StopTimer();
                    }
                });
            });

            return taskQueue.RunTask(() =>
            {
                dictSeesion.Add(token, session);
                session.StartTimer();
                return token;
            });
        }

        public SessionCkeckResult CheckSession(string token, params Authority[] authes)
        {
            token = token != null ? token.Trim() : null;
            if (string.IsNullOrEmpty(token))
                return new SessionCkeckResult(AuthorityState.未登录);

            return taskQueue.RunTask(() =>
            {
                Session session;
                if (!dictSeesion.TryGetValue(token, out session))
                    return new SessionCkeckResult(AuthorityState.未登录);

                session.Tick = 0;

                if (authes == null || authes.Length == 0)
                    return new SessionCkeckResult(AuthorityState.通过, session.User.Id);

                if (session.User.AuthorityRs == null)
                    return new SessionCkeckResult(AuthorityState.权限不够);

                foreach (var auth in authes)
                {
                    if (!session.User.Authorities.Exists(o => o == auth))
                        return new SessionCkeckResult(AuthorityState.权限不够);
                }

                return new SessionCkeckResult(AuthorityState.通过, session.User.Id);
            });
        }

        public int ResetSessionTick(string token)
        {
            return taskQueue.RunTask(() =>
            {
                Session session;
                if (dictSeesion.TryGetValue(token, out session))
                    session.Tick = 0;
                return 0;
            });
        }

        public void DeleteSession(string token)
        {
            taskQueue.RunTask(() =>
            {
                dictSeesion.Remove(token);
                dictSeesion.Remove(token);
            });
        }
    }

    class Session
    {
        private string token;
        private User user;
        private System.Timers.Timer timer;

        const int TICK_MILISECONDS = 30000;

        public Session(string token, User user)
        {
            this.token = token;
            this.user = user;
            timer = new System.Timers.Timer(TICK_MILISECONDS)
            {
                AutoReset = true,
                Enabled = false
            };
            Tick = 0;
        }

        public void SetTimerCallback(ElapsedEventHandler timerCallback)
        {
            timer.Elapsed += timerCallback;
        }

        public string Token
        {
            get { return token; }
        }

        public User User
        {
            get { return user; }
        }

        public int Tick { get; set; }

        public void StopTimer()
        {
            timer.Stop();
        }

        public void StartTimer()
        {
            timer.Start();
        }
    }

    public class SessionCkeckResult
    {
        public bool Succeed { get; private set; }
        private string ErrorMessage { get; set; }
        private string UserId { get; set; }
        private AuthorityState Value { get; set; }

        public SessionCkeckResult(AuthorityState value, string username = null)
        {
            Debug.Assert(value != AuthorityState.通过 || username != null);
            UserId = username;
            Value = value;

            if (value != AuthorityState.通过)
            {
                Succeed = false;
                ErrorMessage = value.ToString();
            }
            else
            {
                Succeed = true;
            }
        }
    }
}