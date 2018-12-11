using WCAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WCAPP.Models.Database;

namespace WCAPP.Types
{
    public static class AuthorityConvertor
    {
        public static Authority[] ToAuthorities(this List<AuthorityR> list)
        {
            if (list == null || list.Count == 0)
                return null;

            var authes = new List<Authority>();
            foreach (var it in list)
            {
                authes.Add(it.Authority);
            }

            return authes.ToArray();
        }
        public static List<AuthorityR> ToAuthorityRs(this Authority[] authes)
        {
            if (authes == null || authes.Length == 0)
                return null;

            var list = new List<AuthorityR>();
            foreach (var it in authes)
            {
                list.Add(new AuthorityR { Authority = it });
            }

            return list;
        }
    }
}