using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM
{
    [Table("userinfo")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class User
    {

        [Column("Id")]
        public int Id { get; set; }

        [Column("UserName")]
        public string UserName { get; set; }

        [Column("Password")]
        public string Password { get; set; }

        [Column("CreateTime")]
        public DateTime CreateTime { get; set; }

        [Column("Status")]
        public bool Status { get; set; }

        [Column("RoleType")]
        public RoleType RoleType { get; set; }

    }

    public enum RoleType : int
    {
        Common = 1,
        Admin = 2
    }
}  
