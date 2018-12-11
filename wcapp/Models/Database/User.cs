using WCAPP.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace WCAPP.Models.Database
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Sex Sex { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }
        public WorkPosition Position { get; set; }
        public string Signature { get; set; }
        

        [ScriptIgnore] public List<AuthorityR> AuthorityRs { get; set; }

        [NotMapped]
        public Authority[] Authorities => AuthorityRs?.ToAuthorities();

        public override string ToString()
        {
            return Name + "(" + Id + ")";
        }
    }

    public class AuthorityR
    {
        public int Id { get; set; }
        public Authority Authority { get; set; }

        [Required] [ScriptIgnore] public User User { get; set; }

        [ScriptIgnore] public string UserId { get; set; }
    }
}