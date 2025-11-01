using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab09_Entity_Framework.Models
{
    // Tên bảng trong CSDL là "Table"
    [Table("Table")]
    public class Table
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(1000)]
        public string Name { get; set; }

        public int Status { get; set; }
        public int? Capacity { get; set; } // Cho phép NULL

        // Khai báo mối quan hệ: Một bàn có nhiều hóa đơn
        [ForeignKey("TableID")]
        public virtual ICollection<Bills> Bills { get; set; }
    }
}