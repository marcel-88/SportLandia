using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TW_WebSite.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public decimal Password { get; set; }
        public decimal Salt { get; set; }
        public string Role { get; set; } //0 - admin // 1 - reg user
    }
}