using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.DAL
{
    public class User
    {
        public string UserName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string FullName
        {
            get
            {
                return Firstname + " " + Lastname;
            }
        }

        public Role Role { get; set; }
        public string Password { get; set; }
        public string HashedPassword { get; set; }
        public DateTime ValidTill { get; set; }

    }

    public enum Role
    {
        Admin,
        User
    }



}
