using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab09_Entity_Framework.Models
{
    [Table("Account")]
    public class Account
    {
        [Key] // Khóa chính
        [StringLength(100)]
        public string AccountName { get; set; }

        [Required]
        [StringLength(200)]
        public string Password { get; set; }

        [Required]
        [StringLength(1000)]
        public string FullName { get; set; }

        public string Email { get; set; }
        public string Tell { get; set; } // Dùng Tell thay vì Phone
        public DateTime? DateCreated { get; set; } // Dùng DateTime? (nullable)

        // Khai báo mối quan hệ: Một Account có nhiều RoleAccount
        public virtual ICollection<RoleAccount> RoleAccounts { get; set; }
    }
}