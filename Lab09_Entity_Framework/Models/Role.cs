using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab09_Entity_Framework.Models
{
    // Chỉ định bảng CSDL tên là "Role"
    [Table("Role")]
    public class Role
    {
        [Key] // Khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Tự động tăng
        public int ID { get; set; }

        [Required] // Không được null
        [StringLength(1000)]
        public string RoleName { get; set; }

        public string Path { get; set; }
        public string Notes { get; set; }

        // Khai báo mối quan hệ: Một Role có nhiều RoleAccount
        public virtual ICollection<RoleAccount> RoleAccounts { get; set; }
    }
}