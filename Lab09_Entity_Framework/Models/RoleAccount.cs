using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab09_Entity_Framework.Models
{
    [Table("RoleAccount")]
    public class RoleAccount
    {
        [Key] // Khóa chính 1
        [Column(Order = 0)] // Khóa chính ghép
        [StringLength(100)]
        public string AccountName { get; set; }

        [Key] // Khóa chính 2
        [Column(Order = 1)] // Khóa chính ghép
        public int RoleID { get; set; }

        public bool Actived { get; set; }
        public string Notes { get; set; }

        // Khai báo khóa ngoại
        [ForeignKey("AccountName")]
        public virtual Account Account { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}