using System;
using WCAPP.Types;

namespace WCAPP.Models.Home
{
    [Serializable]
    public class SessionUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Authority[] Authorities;
    }
}